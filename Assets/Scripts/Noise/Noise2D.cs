using UnityEngine;
using System.Collections;

public interface Noise2D{

	float getNoise(float x, float y);

	void setSeed(int seed);
}
