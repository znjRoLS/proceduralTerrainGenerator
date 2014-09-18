using UnityEngine;
using System.Collections;

public class DetailMapGenerator3 : DetailMapGenerator {





	public void generate(DetailMap detailMap, GraphVoronoi graphVoronoi ){
		//each layer is drawn separately so if you have a lot of layers your draw calls will increase 
		int detailMapSize = detailMap.mapSize;
		int numLayers = detailMap.numLayers;
		int heightMapSize = detailMap.heightMapSize;

		//	float ratio = (float)m_terrainSize/(float)m_detailMapSize;
		
		//Random.seed = 0;
		
		for(int x = 0; x <detailMapSize; x ++) 
		{
			for (int y = 0; y <detailMapSize; y ++) 
			{
				
				for (int h=0; h<numLayers; h++)
					detailMap.setDetail(h,x,y,0);
				
				float ratio1 = (float)(heightMapSize-1)/(float)detailMapSize;
				Center.BiomeTypes biome = graphVoronoi.getNearestCenter((int)(x*ratio1),(int)(y*ratio1)).biome;
				
				
				int det = 10;
				if ((int)biome == 6) {det= 0;}
				if ((int)biome ==12) {det = 1;}
				if (( int)biome==8) {det=2;}
				
				//float unit = 1.0f / (m_detailMapSize - 1);
				
				//float normX = x * unit;
				//float normZ = z * unit;
				
				// Get the steepness value at the normalized coordinate.
				//	float angle = terrain.terrainData.GetSteepness(normX, normZ);
				
				// Steepness is given as an angle, 0..90 degrees. Divide
				// by 90 to get an alpha blending value in the range 0..1.
				//float frac = angle / 90.0f;
				
				if(det<10 )
					
				{
					/*float worldPosX = (x+(m_detailMapSize-1))*ratio;
					float worldPosZ = (z+(m_detailMapSize-1))*ratio;
					
					float noise = m_detailNoise.FractalNoise2D(worldPosX, worldPosZ, 3, m_detailFrq, 1.0f);
					
					if(noise > 0.0f) 
					{*/
					float rnd = Random.value;
					//Randomly select what layer to use
					if(rnd < 0.01f)
						detailMap.setDetail(0,x,y,1);
					else if(rnd < 0.75f)
						detailMap.setDetail(1,x,y,1);
					else
						detailMap.setDetail(2,x,y,1);
					
				}
				
			}
		}
	}

}
