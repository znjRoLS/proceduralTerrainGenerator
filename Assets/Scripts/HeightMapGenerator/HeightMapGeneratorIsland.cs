using UnityEngine;
using System.Collections;

public class HeightMapGeneratorIsland : HeightMapGenerator {

	private Noise2D p_groundNoise;
	public Noise2D groundNoise {set{p_groundNoise= value;}}

	private Noise2D p_mountainNoise;
	public Noise2D mountainNoise{ set { p_mountainNoise = value; } }

	private int mapSize;
	private Vector2 terrainSize;


	public void generate(HeightMap heightMap){

		mapSize = heightMap.mapSize;
		terrainSize = heightMap.terrainSize;

		float ratioX = (float)terrainSize.x/mapSize;
		float ratioY = (float)terrainSize.y/mapSize;

		for(int i = 0; i < mapSize; i++)
		{
			for(int j = 0; j < mapSize; j++)
			{
				float worldPosX = (i+mapSize-1)*ratioX;
				float worldPosY = (j+mapSize-1)*ratioY;
				
				float mountains = p_mountainNoise.getNoise(worldPosX, worldPosY);

				float ground = p_groundNoise.getNoise(worldPosX, worldPosY);

				//float dist = getRadialDistance(i,j);
				//float dist = getRadialDistanceClamped(i,j);
				float dist = getRadialDistanceClamped2(i,j);
				//dist = dist * Mathf.Pow(2,0.5f);
				//float dist2=Mathf.Min (dist,0.99f);

				float height = ground*Mathf.Pow((1.0f - dist),0.5f);
				height += mountains*Mathf.Pow((1 - dist),0.5f)/3.0f;
				height -= Mathf.Pow(dist,1.0f)/10;

				heightMap.setHeight(i,j,height);

			}
		}
		heightMap.scale01 ();

	}

	private float getRadialDistance(int i, int j){
		float distX = 2*((float)i/mapSize - 0.5f);
		float distY = 2*((float)j/mapSize - 0.5f);
		float dist = Mathf.Pow (Mathf.Pow (distX, 2) + Mathf.Pow (distY, 2), 0.5f);

		dist = dist / Mathf.Pow (2, 0.5f);

		return dist;
	}

	private float getRadialDistanceClamped(int i, int j){
		float distX = 2*((float)i/mapSize - 0.5f);
		float distY = 2*((float)j/mapSize - 0.5f);
		float dist = Mathf.Pow (Mathf.Pow (distX, 2) + Mathf.Pow (distY, 2), 0.5f);
		
		dist = Mathf.Min(1.0f, dist);
		
		return dist;
	}

	private float getRadialDistanceClamped2(int i, int j){
		float distX = 2*((float)i/mapSize - 0.5f);
		float distY = 2*((float)j/mapSize - 0.5f);
		float dist = Mathf.Pow (Mathf.Pow (distX, 2) + Mathf.Pow (distY, 2), 0.5f);
		
		dist = Mathf.Min(1.0f, dist);
		dist = Mathf.Max (0.1f, dist);

		return dist;
	}
}
