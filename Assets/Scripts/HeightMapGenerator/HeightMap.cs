using UnityEngine;
using System.Collections;

public class HeightMap {

	private float[,] p_map;
	public float[,] map{get{return p_map;}}

	private int p_mapSize;
	public int mapSize{ get { return p_mapSize; } }


	public HeightMap(int size){
		p_map = new float[size, size];

	}



	public void setHeight(int x, int y, float val){
		p_map[x,y] = val;
	}

	public float getHeight(int x, int y){
		return p_map[x,y];
	}

}
