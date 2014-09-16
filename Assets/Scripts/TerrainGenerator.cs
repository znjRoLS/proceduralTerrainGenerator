using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

	//NOISE
	private Noise2D p_groundNoise;
	private Noise2D p_mountainNoise;

	public int groundNoiseSeed = 0 ;
	public float groundNoiseFrequency = 800.0f;
	public float groundNoiseAmplitude = 0.1f;
	public int groundNoiseOctave = 6;
	public int mountainNoiseSeed = 0;
	public float mountainNoiseFrequency= 1000.0f;
	public float mountainNoiseAmplitude = 0.8f;
	public int mountainNoiseOctave = 8;


	//height ,terrain and alpha map
	private HeightMap p_heightMap;
	private Vector2 p_terrainSize;

	public int heightMapSize = 513;
	public int terrainSizeX = 2048;
	public int terrainSizeY = 2048;


	//GRAPH
	private GraphVoronoi p_graphVoronoi;

	public int voronoiPoints= 5000;

	// Use this for initialization
	void Start () {
	
		init ();
		generateTerrain ();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void init(){

		p_terrainSize = new Vector2 (terrainSizeX, terrainSizeY);
	}


	private void generateTerrain(){

		setupGroundNoise();
		setupMountainNoise ();

		fillHeightMap ();

		buildGraph ();

		assignBiomes ();

		fillAlphaMap ();

	}

	private void setupGroundNoise(){

		p_groundNoise = new PerlinNoise2D();
		p_groundNoise.Seed = groundNoiseSeed;
		p_groundNoise.Amplitude = groundNoiseAmplitude;
		p_groundNoise.Frequency = groundNoiseFrequency;
		p_groundNoise.Octave = groundNoiseOctave;
		p_groundNoise.AllowedNegative = true;

	}

	private void setupMountainNoise(){

		p_mountainNoise = new PerlinNoise2D ();
		p_mountainNoise.Seed = mountainNoiseSeed;
		p_mountainNoise.Amplitude = mountainNoiseAmplitude;
		p_mountainNoise.Frequency = mountainNoiseFrequency;
		p_mountainNoise.Octave = mountainNoiseOctave;
		p_mountainNoise.AllowedNegative = false;

	}

	private void fillHeightMap(){

		p_heightMap = new HeightMap ();
		p_heightMap.mapSize = heightMapSize;
		p_heightMap.terrainSize = p_terrainSize;

		HeightMapGenerator heightMapGenerator = new HeightMapGeneratorIsland ();

		heightMapGenerator.groundNoise = p_groundNoise;
		heightMapGenerator.mountainNoise = p_mountainNoise;
		heightMapGenerator.generate (p_heightMap);

	}

	private void buildGraph(){

		p_graphVoronoi = new GraphVoronoi(voronoiPoints, heightMapSize);
		p_graphVoronoi.pointGenerator = new RandomPointGenerator ();
		p_graphVoronoi.createGraph ();
		p_graphVoronoi.assignCornerElevations (p_heightMap);
		p_graphVoronoi.buildGraph ();
		p_graphVoronoi.fillNearestCenters ();

	}

	private void assignBiomes(){
		foreach (Center p in p_graphVoronoi.centers) {
			p.biome = p.getBiome();
		}
	}

	private void fillAlphaMap(){

		float[,,] map  = new float[m_alphaMapSize, m_alphaMapSize, 18];
		
		Random.seed = 0;
		bool [] bla = new bool[20];
		for (int i=0; i<19; i++)
			bla [i] = false;
		for(int x = 0; x < m_alphaMapSize; x++) 
		{
			for (int z = 0; z < m_alphaMapSize; z++) 
			{
				
				float ratio = (float)(m_heightMapSize-1)/m_alphaMapSize;
				
				Center.BiomeTypes biome = getCenter[(int)(x*ratio),(int)(z*ratio)].biome;
				
				for (int i=0; i< 18;i++)
					map[x,z,i]= 0;
				
				
				if (htmap[x,z]<0.15f)
				{
					map[x,z,6]=1;
					//					
				}
				else
					map[x,z,(int)biome]=1;
				bla[(int)biome]=true;
			}
		}
		for (int i=0; i<19; i++)
			if (bla [i])
				Debug.Log (i);
		terrainData.SetAlphamaps(0, 0, map); //pridruzi alfa mapu terenu

	}
}
