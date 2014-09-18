using UnityEngine;
using System.Collections;

public class AlphaMapGeneratorBiomes : AlphaMapGenerator {





	public void generate(AlphaMap alphaMap, GraphVoronoi graphVoronoi){

		int alphaMapSize = alphaMap.mapSize;
		int heightMapSize = 512;

		float ratio = (float)heightMapSize / alphaMapSize;
			
			
		for(int x = 0; x < alphaMapSize; x++) 
		{
			for (int y = 0; y < alphaMapSize; y++) 
			{
				
				Center.BiomeTypes biome = graphVoronoi.getNearestCenter((int)(ratio* x), (int)(ratio*y)).biome;

				alphaMap.setAlpha(x,y,(int)biome);
			}
		}

		alphaMap.fillSplitMap ();
			
	}


}