using UnityEngine;
using System.Collections;

public interface HeightMapGenerator {

	Noise2D groundNoise{ set; }
	Noise2D mountainNoise{ set; }

	void generate(HeightMap heightMap);

}
