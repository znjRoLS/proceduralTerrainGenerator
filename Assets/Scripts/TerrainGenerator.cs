using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private Terrain p_terrain;
	private HeightMap p_heightMap;
	private Vector2 p_terrainSize;
	private AlphaMap p_alphaMap;
	private DetailMap p_detailMap;
	
	public int detailMapSize = 512;
	public int alphaMapSize = 500;
	public int heightMapSize = 513;
	public int terrainSizeX = 2048;
	public int terrainSizeY = 2048;
	public int terrainHeight = 512;
	public float waterLevel = 0.12f;


	//GRAPH
	private GraphVoronoi p_graphVoronoi;

	public int voronoiPoints= 5000;


	//textures and trees
	private List<SplatPrototype> texturePrototypes;
	private List<TreePrototype> treePrototypes;
	private List<DetailPrototype> detailPrototypes;
	private int[,,] detailMap;

	public Texture2D[] textures;
	public GameObject waterTexture;
	public GameObject[] trees;
	public Texture2D[] details;


	//@todo refactor
	public int m_treeSeed = 2;
	public float  m_treeFrq = 400.0f;
	//Tree settings
	public int[] m_treeSpacing; //spacing between trees
	public float m_treeDistance = 2000.0f; //The distance at which trees will no longer be drawn
	public float m_treeBillboardDistance = 400.0f; //The distance at which trees meshes will turn into tree billboards
	public float m_treeCrossFadeLength = 20.0f; //As trees turn to billboards there transform is rotated to match the meshes, a higher number will make this transition smoother
	public int m_treeMaximumFullLODCount = 400; //The maximum number of trees that will be drawn in a certain area. 


	//Detail settings
	public DetailRenderMode detailMode;
	public int m_detailObjectDistance = 400; //The distance at which details will no longer be drawn
	public float m_detailObjectDensity = 4.0f; //Creates more dense details within patch
	public int m_detailResolutionPerPatch = 32; //The size of detail patch. A higher number may reduce draw calls as details will be batch in larger patches
	public float m_wavingGrassStrength = 0.4f;
	public float m_wavingGrassAmount = 0.2f;
	public float m_wavingGrassSpeed = 0.4f;
	public Color m_wavingGrassTint = Color.white;
	public Color m_grassHealthyColor = Color.white;
	public Color m_grassDryColor = Color.white;
	



	//da houses
	public GameObject object1, object2, object3;


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
		texturePrototypes = new List<SplatPrototype> ();
		treePrototypes = new List<TreePrototype> ();
		detailPrototypes = new List<DetailPrototype> ();
	}


	private void generateTerrain(){

		setupGroundNoise();
		setupMountainNoise ();

		fillHeightMap ();

		buildGraph ();

		assignBiomes ();

		fillAlphaMap ();

		createWater ();

		fillPrototypes ();

		fillDetailMap ();

		createTerrain ();

		fillTreeInstances ();


		Field.start (p_terrain, heightMapSize, p_heightMap.map,object1, object2, object3, waterLevel);

		foreach (GameObject house in GameObject.FindGameObjectsWithTag("house"))
						house.transform.position += new Vector3 (-terrainSizeX / 2, 0, -terrainSizeY/2);

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

		p_heightMap = new HeightMap (heightMapSize);
		p_heightMap.terrainSize = p_terrainSize;

		HeightMapGenerator heightMapGenerator = new HeightMapGeneratorIsland ();

		heightMapGenerator.groundNoise = p_groundNoise;
		heightMapGenerator.mountainNoise = p_mountainNoise;
		heightMapGenerator.generate (p_heightMap);

	}

	private void fillDetailMap(){

		p_detailMap = new DetailMap(detailMapSize, details.Length);
		p_detailMap.heightMapSize = heightMapSize;

		DetailMapGenerator detailMapGenerator = new DetailMapGenerator3 ();

		detailMapGenerator.generate (p_detailMap, p_graphVoronoi);		
		
	}

	private void fillAlphaMap(){
		Debug.Log (textures.Length);
		p_alphaMap = new AlphaMap (alphaMapSize, textures.Length);
		p_alphaMap.terrainSize = p_terrainSize;
		
		AlphaMapGenerator alphaMapGenerator = new AlphaMapGeneratorBiomes ();
		
		alphaMapGenerator.generate (p_alphaMap, p_graphVoronoi);
		
		
	}

	private void buildGraph(){

		p_graphVoronoi = new GraphVoronoi(voronoiPoints, heightMapSize);
		p_graphVoronoi.pointGenerator = new RandomPointGenerator ();
		p_graphVoronoi.createVoronoi ();
		p_graphVoronoi.assignCornerElevations (p_heightMap);
		p_graphVoronoi.buildGraph ();
		p_graphVoronoi.fillNearestCenters ();


	}

	private void assignBiomes(){
		foreach (Center p in p_graphVoronoi.centers) {
			p.biome = p.getBiome();
		}
	}



	private void createWater(){
		int sizeX = (int) (terrainSizeX /2 * 1.41);
		int sizeY = (int) (terrainSizeY /2 * 1.41);
		float waterHeight = terrainHeight * waterLevel;
		GameObject water = (GameObject)Instantiate (waterTexture, new Vector3 (0, waterHeight, 0), Quaternion.identity);
		Vector3 v = water.transform.localScale;
		water.transform.localScale = v + new Vector3 (sizeX, 0, sizeY);
	}

	private void fillPrototypes(){
		
		for (int i= 0; i < textures.Length ; i++) {
			
			SplatPrototype splatPrototype = new SplatPrototype();
			splatPrototype.texture = textures[i];
			splatPrototype.tileSize = new Vector2 (2, 2);
			
			texturePrototypes.Add( splatPrototype);
			
		}

		for (int i= 0; i < trees.Length ; i++) {
			
			TreePrototype treePrototype = new TreePrototype();
			treePrototype.prefab = trees[i];
			
			treePrototypes.Add( treePrototype);
			
		}

		for (int i=0; i< details.Length; i++) {
		
			DetailPrototype detailPrototype = new DetailPrototype();
			detailPrototype.prototypeTexture = details[i];
			detailPrototype.renderMode = detailMode;
			detailPrototype.healthyColor = m_grassHealthyColor;
			detailPrototype.dryColor = m_grassDryColor;

			detailPrototypes.Add (detailPrototype);
		}

	}

	private void createTerrain(){

		TerrainData terrainData = new TerrainData();
		terrainData.heightmapResolution = heightMapSize;
		terrainData.SetHeights(0, 0, p_heightMap.map);
		terrainData.size = new Vector3((float)terrainSizeX, (float)terrainHeight, (float)terrainSizeY);
		terrainData.splatPrototypes = texturePrototypes.ToArray();
		terrainData.treePrototypes = treePrototypes.ToArray();
		terrainData.detailPrototypes = detailPrototypes.ToArray();
		terrainData.alphamapResolution = alphaMapSize;
		terrainData.SetAlphamaps(0, 0, p_alphaMap.splitMap); //pridruzi alfa mapu terenu
		
		
		p_terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
		p_terrain.transform.position = new Vector3(-terrainSizeX*0.5f, 0,-terrainSizeY*0.5f); // zasto?

		
		
		p_terrain.castShadows = false;

		p_terrain.treeDistance = m_treeDistance;
		p_terrain.treeBillboardDistance = m_treeBillboardDistance;
		p_terrain.treeCrossFadeLength = m_treeCrossFadeLength;
		p_terrain.treeMaximumFullLODCount = m_treeMaximumFullLODCount;

		p_terrain.terrainData.wavingGrassStrength = m_wavingGrassStrength;
		p_terrain.terrainData.wavingGrassAmount = m_wavingGrassAmount;
		p_terrain.terrainData.wavingGrassSpeed = m_wavingGrassSpeed;
		p_terrain.terrainData.wavingGrassTint = m_wavingGrassTint;
		p_terrain.detailObjectDensity = m_detailObjectDensity;
		p_terrain.detailObjectDistance = m_detailObjectDistance;
		p_terrain.terrainData.SetDetailResolution(detailMapSize, m_detailResolutionPerPatch);

//		int[,] dd = new int[512,512];
//		for (int i=0; i<512; i++)
//						for (int j=0; j<512; j++)
//								dd [i, j] = 0;
//
//		p_terrain.terrainData.SetDetailLayer (0, 0, 0, dd);
		for (int i=0; i< p_detailMap.map.Count ; i++)
			p_terrain.terrainData.SetDetailLayer(0,0,i,p_detailMap.map[i]);

	}

	void fillTreeInstances()
	{
		Random.seed = 0;
		
		for(int x = 0; x < terrainSizeX; x ++) 
		{
			for (int y = 0; y < terrainSizeY; y ++) 
			{
				
				float ratio = (float)(heightMapSize-1)/terrainSizeX;
				
				Center.BiomeTypes biome = p_graphVoronoi.getNearestCenter((int)(x*ratio),(int)(y*ratio)).biome;
				
				//int space=0;
				int tree = 10;
				if ((int)biome == 4) {tree = 0;}
				if ((int)biome ==9) {tree = 1;}
				if ((int)biome == 10) { tree = 2;}
				if ((int)biome == 11 ){ tree =3; }
				if( (int) biome==13) { tree =4; }
				
				float unit = 1.0f / (terrainSizeX - 1);
				
				//float offsetX = Random.value * unit * m_treeSpacing;
				//float offsetZ = Random.value * unit * m_treeSpacing;
				
				float normX = x * unit;// + offsetX;
				float normZ = y * unit;// + offsetZ;
				
				// Get the steepness value at the normalized coordinate.
				float angle = p_terrain.terrainData.GetSteepness(normX, normZ);
				
				// Steepness is given as an angle, 0..90 degrees. Divide
				// by 90 to get an alpha blending value in the range 0..1.
				float frac = angle / 90.0f;
				
				float height = p_heightMap.getHeight(y*heightMapSize/terrainSizeY,x*heightMapSize/terrainSizeX);
	
				
				if (tree<5)
				{
					
					if(frac < 0.5f && height> waterLevel+0.05f && Random.Range(0,m_treeSpacing[tree]) == 0) //make sure tree are not on steep slopes & in the sea
					{
						float worldPosX = x+(terrainSizeX-1);
						float worldPosZ = y+(terrainSizeY-1);
						
						float ht = p_terrain.terrainData.GetInterpolatedHeight(normX, normZ);
						
						if( ht < terrainHeight*0.4f )
						{
							
							TreeInstance temp = new TreeInstance();
							temp.position = new Vector3(normX,ht,normZ);
							temp.prototypeIndex = tree;
							temp.widthScale = 1;
							temp.heightScale = 1;
							temp.color = Color.white;
							temp.lightmapColor = Color.white;
							
							p_terrain.AddTreeInstance(temp);
						}
					}
				}
			}
		}
		
		
		
	}



}
