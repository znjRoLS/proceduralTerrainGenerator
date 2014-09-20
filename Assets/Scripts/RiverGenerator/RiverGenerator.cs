using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverGenerator : MonoBehaviour{

	private Terrain p_terrain;
	public Terrain terrain{set{p_terrain = value;}}

	public RiverGenerator(){
	
	}

	
	
	
	
	
	public void generate(List<Vector2> p){
	
		List<Vector3> points = get3DPoints(p);
		List<Vector2> UV = assignUV(p);
		List<int> triangles = assignTriangles(p);
		
		createMesh(points.ToArray(), UV.ToArray(), triangles.ToArray());
				
	}
	
	
	
	private List<Vector3> get3DPoints(List<Vector2> p){
	
		List<Vector3> points = new List<Vector3>();
		
		foreach(Vector2 pp in p)
			points.Add(new Vector3(pp.x, getHeight(pp.x,pp.y) ,pp.y));
			
		return points;
	
	}
	
	private float getHeight(float x, float y){
		return Terrain.activeTerrain.SampleHeight(new Vector3(x,0,y));
	}
	
	private List<Vector2> assignUV(List<Vector2> points){
	
		List<Vector2> uv = new List<Vector2>();
		
		for (int i=0; i < points.Count; i++)
			uv.Add(new Vector2( (i /2) % 2, i%2));
			
		return uv;	
	}
	
	private List<int> assignTriangles(List<Vector2> points){
			
			List<int> triangles = new List<int>();
	
			for(int i=0; i< points.Count; i+=2){
				int[] arr = {i,i+1,i+2,i+1,i+2,i+3};
				triangles.AddRange(new List<int>(arr));
		}
					
			return triangles;
		
	}
	
	private void createMesh(Vector3[] newVertices, Vector2[] newUV, int[] newTriangles){
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
	}
	
}