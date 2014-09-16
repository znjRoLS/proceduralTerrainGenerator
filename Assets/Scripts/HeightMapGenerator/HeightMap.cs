using UnityEngine;
using System.Collections;

public class HeightMap {

	private float[,] p_map;
	public float[,] map{get{return p_map;}}

	private int p_mapSize;
	public int mapSize{ get { return p_mapSize; } set{ p_mapSize = value; p_map = new float[value,value];} }

	private Vector2 p_terrainSize;
	public Vector2 terrainSize{ get { return p_terrainSize; } set { p_terrainSize = value; } }

	public HeightMap(){

	}



	public void setHeight(int x, int y, float val){
		p_map[x,y] = val;
	}

	public float getHeight(int x, int y){
		return p_map[x,y];
	}

	public void scale(int min, int max){

		float mini = 1000000.0f;
		float maxi = -1000000.0f;
		for (int i = 0; i< p_mapSize; i++)	for (int j=0; j< p_mapSize; j++) {
			if(p_map[i,j] > maxi) maxi = p_map[i,j];
			if (p_map[i,j] < mini) mini = p_map[i,j];
		}

		for (int i = 0; i< p_mapSize; i++)	for (int j=0; j< p_mapSize; j++) 
			p_map[i,j] = (p_map[i,j] - mini)/(maxi-mini) * (max-min) + min;

	}

	public void scale01(){
			scale (0,1);
		}

}
