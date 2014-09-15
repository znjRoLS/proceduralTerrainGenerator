using UnityEngine;
using System.Collections;

interface Noise2D{

	float getNoise(float x, float y);

	void setSeed(int seed);
}
