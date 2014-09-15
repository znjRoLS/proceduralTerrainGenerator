using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphVoronoi {

	private List<Vector2> points;
	public List<Vector2> Points{ get { return points; } }

	private List<Center> centers;
	public List<Center> Centers{ get { return centers; } }
	
	private List<Edge> edges;
	public List<Edge> Edges{ get { return edges; } }

	private List<Corner> corners;
	public List<Corner> Corners{ get { return corners; } }

	private Vector2 voronoiMapSize;
	public Vector2 VoronoiMapSize{ set { voronoiMapSize = value; } }
	

	private Delaunay.Voronoi voronoi;



	public GraphVoronoi(){
		init ();
	}

	private void init(){
			points = new List<Vector2> ();
			centers = new List<Center> ();
			edges = new List<Edge> ();
			corners = new List<Corner> ();
	}



	private void setPoints(){

	}

	public void buildGraph(){
		voronoi = new Delaunay.Voronoi (points, null, new Rect (0, 0, voronoiMapSize.x, voronoiMapSize.y));
	}


}
