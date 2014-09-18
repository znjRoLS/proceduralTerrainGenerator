using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DetailMap {

	private List<int[,]> p_map;
	public List<int[,]> map{get{return p_map;}}
	
	private int p_mapSize;
	public int mapSize{ get { return p_mapSize; } set{ p_mapSize = value; } }

	private int p_numLayers;
	public int numLayers{ get { return p_numLayers; } set { p_numLayers = value; } }

	private int p_heightMapSize;
	public int heightMapSize{ get { return p_heightMapSize; } set { p_heightMapSize = value; } }
	
	public DetailMap(int mapSize, int num){
		p_mapSize = mapSize;
		p_numLayers = num;
		p_map = new List<int[,]> ();
		for (int i=0;i<num;i++)
			p_map.Add(new int[p_mapSize, p_mapSize]);
	}
	
	
	
	public void setDetail(int h, int x, int y, int val){
		p_map[h][x,y] = val;
	}
	
	public float getDetail(int h, int x, int y){
		return p_map[h][x,y];
	}
	
//	public void scale(int min, int max){
//		
//		float mini = 1000000.0f;
//		float maxi = -1000000.0f;
//		for (int i = 0; i< p_mapSize; i++)	for (int j=0; j< p_mapSize; j++) {
//			if(p_map[i,j] > maxi) maxi = p_map[i,j];
//			if (p_map[i,j] < mini) mini = p_map[i,j];
//		}
//		
//		for (int i = 0; i< p_mapSize; i++)	for (int j=0; j< p_mapSize; j++) 
//			p_map[i,j] = (p_map[i,j] - mini)/(maxi-mini) * (max-min) + min;
//		
//	}
//	
//	public void scale01(){
//		scale (0,1);
//	}
	
}
