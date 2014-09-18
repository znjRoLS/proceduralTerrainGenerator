using UnityEngine;
using System.Collections;

public class AlphaMap {
	
	private int[,] p_map;
	public int[,] map{get{return p_map;}}

	private float[,,] p_splitMap;
	public float[,,] splitMap{ get { return p_splitMap; } }
	
	private int p_mapSize;
	public int mapSize{ get { return p_mapSize; } set{ p_mapSize = value; } }

	private int p_numTextures;
	public int numTextures{ set { p_numTextures = value; } }

	private Vector2 p_terrainSize;
	public Vector2 terrainSize{ get { return p_terrainSize; } set { p_terrainSize = value; } }
	
	public AlphaMap(int mapSize, int numTextures){
		p_mapSize = mapSize;
		p_numTextures = numTextures;

		p_map = new int[p_mapSize, p_mapSize];
		p_splitMap = new float[p_mapSize, p_mapSize, p_numTextures];
	}
	
	
	
	public void setAlpha(int x, int y, int val){
		p_map[x,y] = val;
	}
	
	public float getAlpha(int x, int y){
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
			p_map[i,j] = (int)((p_map[i,j] - mini)/(maxi-mini) * (max-min) + min);
		
	}
	
	public void scale01(){
		scale (0,1);
	}

	public void fillSplitMap(){
				for (int i = 0; i< p_mapSize; i++)
						for (int j=0; j< p_mapSize; j++) {
								for (int h=0; h<p_numTextures; h++)
										p_splitMap [i, j, h] = 0;
			if (p_map[i,j] >17) Debug.Log(i + " " + j + " " + p_map[i,j]);
								p_splitMap [i, j, p_map [i, j]] = 1;

						}
		}
}
