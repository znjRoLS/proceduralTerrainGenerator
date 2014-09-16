using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphVoronoi {

	private int p_boundaryOffset = 10;

	private List<Vector2> p_points;
	public List<Vector2> points{ get { return p_points; } }

	private List<Center> p_centers;
	public List<Center> centers{ get { return p_centers; } }
	
	private List<Edge> p_edges;
	public List<Edge> edges{ get { return p_edges; } }

	private List<Corner> p_corners;
	public List<Corner> corners{ get { return p_corners; } }

	private Vector2 p_voronoiMapSize;
	public Vector2 voronoiMapSize{ set { p_voronoiMapSize = value; } }

	private PointGenerator p_pointGenerator;
	public PointGenerator pointGenerator{ set { p_pointGenerator = value; } }

	private Delaunay.Voronoi p_voronoi;
	private Center[,] p_nearestCenter;
	private int p_numPoints;
	private int p_heightMapSize;
	private Dictionary<int,List<Corner>> _cornerMap = new Dictionary<int,List<Corner>>();
	private float waterLimit = 0.12f;

	public GraphVoronoi(int numPoints, int mapSize){
		p_numPoints = numPoints;
		p_heightMapSize = mapSize;
		init ();
	}

	private void init(){
		p_points = new List<Vector2> ();
		p_centers = new List<Center> ();
		p_edges = new List<Edge> ();
		p_corners = new List<Corner> ();
		p_nearestCenter = new Center[p_heightMapSize,p_heightMapSize];

	}





	public void buildGraph(){

		assignOceanCoastAndLand ();

		foreach (Corner q in p_corners) {
			if (q.ocean || q.coast) {
				q.elevation = 0f;
			}
		}	

		assignPolygonElevations ();
		calculateDownslopes ();
		calculateWatersheds ();
		createRivers ();
		assignCornerMoisture ();
		redistributeMoisture (landCorners (p_corners));
		assignPolygonMoisture ();

	}

	public void fillNearestCenters(){

		for (int i=0; i <p_heightMapSize; i++)
			for (int j=0; j<p_heightMapSize; j++)
				p_nearestCenter [i, j] = centers [0];

		foreach (Center center in centers) {
			
			foreach(Vector2 point in GetPoints(p_voronoi.Region(center.point))){
				
				p_nearestCenter[(int) point.x, (int) point.y] = center;
				
				
			}
			
		}

	}

	public void assignCornerElevations(HeightMap heightMap){

		
		foreach (Corner p in p_corners) {
			
			int elevX = (int) (p.point.x);
			int elevY = (int) (p.point.y);
			
			if (elevX == heightMap.mapSize) elevX--;
			if (elevY == heightMap.mapSize) elevY--;
			
			p.elevation = heightMap.getHeight(elevX,elevY) ;
			
			p.water = p.elevation < waterLimit; //* (heightMaximum - heightMinimum) + heightMinimum;
			
		}
		
	}


	private void setPoints(){
		p_points = p_pointGenerator.generate (p_numPoints,p_voronoiMapSize, p_boundaryOffset);
	}



	

	public void createGraph() {
		setPoints ();
		p_voronoi = new Delaunay.Voronoi (p_points, null, new Rect (0, 0, p_voronoiMapSize.x, p_voronoiMapSize.y));


		Center p;
		Corner q; 
		Vector2 point;
		Vector2 other;
		List<Delaunay.Edge> libedges= p_voronoi.Edges();
		Dictionary<System.Nullable<Vector2>,Center> centerLookup = new Dictionary<System.Nullable<Vector2>,Center>();
		
		// Build Center objects for each of the p_points, and a lookup map
		// to find those Center objects again as we build the graph
		foreach ( Vector2 ppp in p_points) {
			System.Nullable<Vector2> pp = (System.Nullable<Vector2>) ppp;
			p = new Center();
			p.index = p_centers.Count;
			p.point = (Vector2) pp;
			p.neighbors = new List<Center>();
			p.borders = new List<Edge>();
			p.corners = new List<Corner>();
			centers.Add(p);
			centerLookup[pp] = p;
		}
		foreach ( Center po in p_centers) {
			p_voronoi.Region(po.point);
		}
		
		
		
		foreach (Delaunay.Edge libedge in libedges) {
			Delaunay.Geo.LineSegment dedge = libedge.DelaunayLine();
			Delaunay.Geo.LineSegment vedge = libedge.VoronoiEdge();
			
			// Fill the graph data. Make an Edge object corresponding to
			// the edge from the p_voronoi library.
			Edge edge = new Edge();
			edge.index = p_edges.Count;
			edge.river = 0;
			p_edges.Add(edge);
			edge.midpoint = null;
			if (vedge.p0!= null && vedge.p1 != null)
				edge.midpoint = Vector2.Lerp( (Vector2) vedge.p0, (Vector2) vedge.p1, 0.5f);
			//edge.midpoint = vedge.p0!= null && vedge.p1 != null && new Vector2(vedge.p0.x + (vedge.p1.x - vedge.p0.x) * 0.5,vedge.p0.y + (vedge.p1.y - vedge.p0.y) * 0.5);
			//edge.midpoint = new Vector2(vedge.p0 + (vedge.p1.x - vedge.p0.x) * 0.5,vedge.p0.y + (vedge.p1.y - vedge.p0.y) * 0.5);
			
			// p_edges point to p_corners. p_edges point to p_centers. 
			edge.v0 = makeCorner(vedge.p0);
			edge.v1 = makeCorner(vedge.p1);
			edge.d0 = centerLookup[dedge.p0];
			edge.d1 = centerLookup[dedge.p1];
			
			// p_centers point to p_edges. p_corners point to p_edges.
			if (edge.d0 != null) { edge.d0.borders.Add(edge); }
			if (edge.d1 != null) { edge.d1.borders.Add(edge); }
			if (edge.v0 != null) { edge.v0.protrudes.Add(edge); }
			if (edge.v1 != null) { edge.v1.protrudes.Add(edge); }
			
			// p_centers point to p_centers.
			if (edge.d0 != null && edge.d1 != null) {
				addToCenterList(edge.d0.neighbors, edge.d1);
				addToCenterList(edge.d1.neighbors, edge.d0);
			}
			
			// p_corners point to p_corners
			if (edge.v0 != null && edge.v1 != null) {
				addToCornerList(edge.v0.adjacent, edge.v1);
				addToCornerList(edge.v1.adjacent, edge.v0);
			}
			
			// p_centers point to p_corners
			if (edge.d0 != null) {
				addToCornerList(edge.d0.corners, edge.v0);
				addToCornerList(edge.d0.corners, edge.v1);
			}
			if (edge.d1 != null) {
				addToCornerList(edge.d1.corners, edge.v0);
				addToCornerList(edge.d1.corners, edge.v1);
			}
			
			// p_corners point to p_centers
			if (edge.v0 != null) {
				addToCenterList(edge.v0.touches, edge.d0);
				addToCenterList(edge.v0.touches, edge.d1);
			}
			if (edge.v1 != null) {
				addToCenterList(edge.v1.touches, edge.d0);
				addToCenterList(edge.v1.touches, edge.d1);
			}
		}
	}
	
	public Corner makeCorner(System.Nullable<Vector2> npoint) {
		Corner q;
		int bucket;
		if (npoint == null) return null;
		Vector2 point = (Vector2) npoint;
		for (bucket = (int)(point.x)-1; bucket <= (int)(point.x)+1; bucket++) {
			if (_cornerMap.ContainsKey(bucket))
			{
				foreach (Corner qq in _cornerMap[bucket]) {
					float dx = point.x - qq.point.x;
					float dy = point.y - qq.point.y;
					if (dx*dx + dy*dy < 1e-6) {
						return qq;
					}
				}
			}
		}
		bucket = (int)(point.x);
		
		if (! _cornerMap.ContainsKey(bucket)) _cornerMap[bucket] = new List<Corner>();
		q = new Corner();
		q.index = p_corners.Count;
		corners.Add(q);
		q.point = point;
		q.border = (point.x == 0 || point.x == p_heightMapSize
		            || point.y == 0 || point.y == p_heightMapSize);
		q.touches = new List<Center>();
		q.protrudes = new List<Edge>();
		q.adjacent = new List<Corner>();
		_cornerMap[bucket].Add(q);
		return q;
	}
	private void addToCornerList(List<Corner> v,Corner x) {
		if (x != null && v.IndexOf(x) < 0) { v.Add(x); }
	}
	private void addToCenterList(List<Center> v,Center x) {
		if (x != null && v.IndexOf(x) < 0) { v.Add(x); }
	}
	
	private void assignOceanCoastAndLand()
	{
		
		
		Queue<Center> queue = new Queue<Center> ();
		//Center p, r;
		//Corner q;
		int numWater;
		
		foreach (Center p in p_centers) {
			numWater = 0;
			foreach (Corner q in p.corners) {
				if (q.border) {
					p.border = true;
					p.ocean = true;
					q.water = true;
					queue.Enqueue(p);
				}
				if (q.water) {
					numWater += 1;
				}
			}
			p.water = (p.ocean || numWater >= p.corners.Count * 0.3f);
		}
		while (queue.Count > 0) {
			Center p = queue.Dequeue();
			foreach (Center r in p.neighbors) {
				if (r.water && !r.ocean) {
					r.ocean = true;
					queue.Enqueue(r);
				}
			}
		}
		
		foreach (Center p in p_centers) {
			int  numOcean = 0;
			int  numLand = 0;
			foreach (Center r in p.neighbors) {
				numOcean += (r.ocean)?1:0;
				numLand += (!r.water)?1:0;
			}
			p.coast = (numOcean > 0) && (numLand > 0);
		}
		
		
		foreach (Corner q in p_corners) {
			int numOcean = 0;
			int numLand = 0;
			foreach (Center p in q.touches) {
				numOcean += (p.ocean)?1:0;
				numLand += (!p.water)?1:0;
			}
			q.ocean = (numOcean == q.touches.Count);
			q.coast = (numOcean > 0) && (numLand > 0);
			q.water = q.border || ((numLand != q.touches.Count) && !q.coast);
		}
	}
	
	
	public void assignPolygonElevations(){
		float sumElevation;

		foreach (Center p in p_centers) {
			sumElevation = 0f;
			foreach (Corner q in p.corners) {
				sumElevation += q.elevation;
			}
			p.elevation = sumElevation / p.corners.Count;

		}
	}
	
	
	
	public List<Corner> landCorners(List<Corner> p_corners){
		List<Corner> locations = new List<Corner> ();
		foreach (Corner q in p_corners) {
			if (!q.ocean && !q.coast) {
				locations.Add(q);
			}
		}
		return locations;
	}
	
	public void calculateDownslopes() {
		
		
		foreach (Corner q in p_corners) {
			Corner r = q;
			foreach (Corner s in q.adjacent) {
				if (s.elevation < r.elevation) {
					r = s;
				}
			}
			q.downslope = r;
		}
	}
	
	public void calculateWatersheds() {
		//var q:Corner, r:Corner, i:int, changed:Boolean;
		bool changed;
		int i;
		// Initially the watershed pointer p_points downslope one step.      
		foreach ( Corner q in p_corners) {
			q.watershed = q;
			if (!q.ocean && !q.coast) {
				q.watershed = q.downslope;
			}
		}
		// Follow the downslope pointers to the coast. Limit to 100
		// iterations although most of the time with numPoints==2000 it
		// only takes 20 iterations because most p_points are not far from
		// a coast.  TODO: can run faster by looking at
		// p.watershed.watershed instead of p.downslope.watershed.
		for (i = 0; i < 10000; i++) {
			changed = false;
			foreach (Corner q in p_corners) {
				if (!q.ocean && !q.coast && !q.watershed.coast) {
					Corner r = q.downslope.watershed;
					if (!r.ocean) q.watershed = r;
					changed = true;
				}
			}
			if (!changed) break;
		}
		// How big is each watershed?
		foreach (Corner q in p_corners) {
			Corner r = q.watershed;
			r.watershed_size+=1;
		}
		
	}
	
	public void createRivers() {
		//var i:int, q:Corner, edge:Edge;
		int k=0;
		for (int i = 0; i <corners.Count/2; i++) {
			Corner q = p_corners[Random.Range(0, p_corners.Count-1)];
			if (q.ocean || q.elevation<waterLimit || q.elevation > 0.9f) continue;
			// Bias rivers to go west: if (q.downslope.x > q.x) continue;
			while (!q.coast ) {
				if (q == q.downslope) {
					break;
				}
				Edge edge = lookupEdgeFromCorner(q, q.downslope);
				edge.river = edge.river + 1;
				q.river+=1;
				q.downslope.river+= 1; 
				q = q.downslope;
			}
			//			k++;
			
		}
	}
	public Edge lookupEdgeFromCorner(Corner q, Corner s) {
		foreach (Edge edge in q.protrudes) {
			if (edge.v0 == s || edge.v1 == s) return edge;
		}
		return null;
	}
	
	public void assignCornerMoisture() {
		//Corner q, r;
		float	  newMoisture;
		Queue<Corner> queue=new Queue<Corner>();
		foreach (Corner q in p_corners) {
			if ((q.water || q.river > 0) && !q.ocean) {
				q.moisture = q.river > 0? Mathf.Min(3.0f, (0.2f * q.river)) : 1.0f;
				queue.Enqueue(q);
			} else {
				q.moisture = 0.0f;
			}
		}
		while (queue.Count > 0) {
			Corner q = queue.Dequeue();
			
			foreach (Corner r in q.adjacent) {
				newMoisture = q.moisture * 0.9f;
				if (newMoisture > r.moisture) {
					r.moisture = newMoisture;
					queue.Enqueue(r);
				}
			}
		}
		foreach (Corner q in p_corners) {
			if (q.ocean || q.coast) {
				q.moisture = 1.0f;
			}
			Debug.Log (q.moisture);
		}
	}
	
	
	
	public void assignPolygonMoisture() {
		//Center p, q;
		float	 sumMoisture;
		foreach (Center p in p_centers) {
			sumMoisture = 0.0f;
			foreach (Corner q in p.corners) {
				if (q.moisture > 1.0f) q.moisture = 1.0f;
				sumMoisture += q.moisture;
			}
			p.moisture = sumMoisture / p.corners.Count;
			
			//			if (p.elevation>0.7f) Debug.Log (p.moisture);
		}
		
	}
	
	public void redistributeMoisture(List<Corner> locations) {
		
		
		locations.Sort(delegate(Corner x, Corner y)	{
			if( x.moisture < y.moisture) return -1;
			else if( x.moisture > y.moisture) return 1;
			else return 0;
		});
		for (int i = 0; i < locations.Count; i++) {
			locations[i].moisture = (float)i/(float)(locations.Count-1);
		}
		
	}

	public static List<Vector2> GetPoints(List<Vector2> p_points)
	{
		if (p_points.Count == 0)
			return new List<Vector2>();
		List<Vector2> pPoints = new List<Vector2>();
		float highestx = p_points[0].x;
		float highesty = p_points[0].y;
		float lowestx = p_points[0].x;
		float lowesty = p_points[0].y;
		for (int i = 0; i < p_points.Count; i++)
		{
			if (p_points[i].x > highestx)
				highestx = p_points[i].x;
			if (p_points[i].y > highesty)
				highesty = p_points[i].y;
			if (p_points[i].x < lowestx)
				lowestx = p_points[i].x;
			if (p_points[i].y < lowesty)
				lowesty = p_points[i].y;
		}
		for (int x = (int)lowestx  ; x < Mathf.CeilToInt(highestx); x++)
		{
			for (int y = (int)lowesty; y < Mathf.CeilToInt( highesty ); y++)
			{
				if (IsPointInPolygon( p_points, new Vector2(x, y)))
				{
					pPoints.Add(new Vector2(x,y));
				}
			}
		}
		return pPoints;
	}
	
	static private bool IsPointInPolygon( List<Vector2> polygon, Vector2 point)
	{
		float epsilon = 0.01f;
		bool isInside = false;
		for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
		{
			if (((polygon[i].y > point.y + epsilon) != (polygon[j].y > point.y + epsilon)) &&
			    (point.x +epsilon < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
			{
				isInside = !isInside;
			}
		}
		return isInside;
	}

}
