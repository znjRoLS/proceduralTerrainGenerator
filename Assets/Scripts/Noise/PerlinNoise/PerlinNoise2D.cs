using UnityEngine;
using System.Collections;

public class PerlinNoise2D : Noise2D {

	private float frequency;
	public float Frequency{ set{frequency = value;} }

	private float amplitude;
	public float Amplitude{ set { amplitude = value; } }

	private bool allowedNegative;
	public bool AllowedNegative{ set { allowedNegative = value; } } 

	private int octave;
	public int Octave{ set { octave = value; } }

	private int seed;
	public int Seed {
				set {
						seed = value;
						perlinNoise = new PerlinNoise (seed);
				}
		}

	private PerlinNoise perlinNoise;



	public PerlinNoise2D(){
		init ();
	}

	private void init(){
		octave = 8;
		frequency = 1000.0f;
		amplitude = 1.0f;
		allowedNegative = true;

	}


	

	public float getNoise(float x, float y){
		float noise = perlinNoise.FractalNoise2D(x,y,octave,frequency,amplitude);
		if (!allowedNegative && noise < 0.0f)
						noise = 0.0f;
		return noise;
	}

}
