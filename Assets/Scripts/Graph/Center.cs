using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Center {

	public enum BiomeTypes {
		OCEAN=0, MARSH, ICE, LAKE ,
		BEACH, SNOW ,TUNDRA ,BARE,
		SCORCHED ,TAIGA, SHRUBLAND ,TEMPERATE_DESERT ,
		TEMPERATE_RAIN_FOREST, TEMPERATE_DECIDUOUS_FOREST,
		GRASSLAND, TROPICAL_RAIN_FOREST, TROPICAL_SEASONAL_FOREST, SUBTROPICAL_DESERT
	};

	public int index;
	
	public Vector2 point;  // location
	public bool water;  // lake or ocean
	public bool ocean;  // ocean
	public bool coast;  // land polygon touching an ocean
	public bool border;  // at the edge of the map
	public BiomeTypes biome;  // biome type (see article)
	public float elevation;  // 0.0-1.0
	public float moisture;  // 0.0-1.0
	
	public List<Center> neighbors;
	public List<Edge> borders;
	public List<Corner> corners;

	public BiomeTypes getBiome() {
		if (this.ocean) {
			return BiomeTypes.OCEAN;
		} else if (this.water) {
			if (this.elevation < 0.1f) return BiomeTypes.MARSH;
			if (this.elevation > 0.8f) return BiomeTypes.ICE;
			return BiomeTypes.LAKE;
		} else if (this.coast) {
			return BiomeTypes.BEACH;
		} else if (this.elevation > 0.8f) {
			if (this.moisture > 0.50f) return BiomeTypes.SNOW;
			else if (this.moisture > 0.33f) return BiomeTypes.TUNDRA;
			else if (this.moisture > 0.16f) return BiomeTypes.BARE;
			else return BiomeTypes.SCORCHED;
		} else if (this.elevation > 0.6f) {
			if (this.moisture > 0.66f) return BiomeTypes.TAIGA;
			else if (this.moisture > 0.33f) return BiomeTypes.SHRUBLAND;
			else return BiomeTypes.TEMPERATE_DESERT;
		} else if (this.elevation > 0.3f) {
			if (this.moisture > 0.83f) return BiomeTypes.TEMPERATE_RAIN_FOREST;
			else if (this.moisture > 0.50f) return BiomeTypes.TEMPERATE_DECIDUOUS_FOREST;
			else if (this.moisture > 0.16f) return BiomeTypes.GRASSLAND;
			else return BiomeTypes.TEMPERATE_DESERT;
		} else {
			if (this.moisture > 0.66f) return BiomeTypes.TROPICAL_RAIN_FOREST;
			else if (this.moisture > 0.33f) return BiomeTypes.TROPICAL_SEASONAL_FOREST;
			else if (this.moisture > 0.16f) return BiomeTypes.GRASSLAND;
			else return BiomeTypes.SUBTROPICAL_DESERT;
		}
	}
}