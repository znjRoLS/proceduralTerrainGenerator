using UnityEngine;
using System.Collections;

public class HeightMapGeneratorIsland : HeightMapGenerator {

	private Noise2D p_groundNoise;
	public Noise2D groundNoise {set{p_groundNoise= value;}}

	private Noise2D p_mountainNoise;
	public Noise2D mountainNoise{ set { p_mountainNoise = value; } }




	public void generate(HeightMap heightMap){

		int mapSize = heightMap.mapSize;


	}
}
