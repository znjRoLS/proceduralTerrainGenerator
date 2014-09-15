using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphVoronoi {

	private int p_boundaryOffset = 10;

	private List<Vector2> p_points;
	public List<Vector2> Points{ get { return p_points; } }

	private List<Center> p_centers;
	public List<Center> Centers{ get { return p_centers; } }
	
	private List<Edge> p_edges;
	public List<Edge> Edges{ get { return p_edges; } }

	private List<Corner> p_corners;
	public List<Corner> corners{ get { return p_corners; } }

	private Vector2 p_voronoiMapSize;
	public Vector2 voronoiMapSize{ set { p_voronoiMapSize = value; } }

	private PointGenerator p_pointGenerator;
	public PointGenerator pointGenerator{ set { p_pointGenerator = value; } }

	private Delaunay.Voronoi p_voronoi;



	public GraphVoronoi(){
		init ();
	}

	private void init(){
		p_points = new List<Vector2> ();
		p_centers = new List<Center> ();
		p_edges = new List<Edge> ();
		p_corners = new List<Corner> ();
	}





	public void buildGraph(int numPoints){
		setPoints (numPoints);
		p_voronoi = new Delaunay.Voronoi (p_points, null, new Rect (0, 0, p_voronoiMapSize.x, p_voronoiMapSize.y));
	
	
	}



	private void setPoints(int numPoints){
		p_points = p_pointGenerator.generate (numPoints,p_voronoiMapSize, p_boundaryOffset);
	}


}
