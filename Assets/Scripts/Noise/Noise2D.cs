using UnityEngine;
using System.Collections;

public interface Noise2D{

	float Frequency{ set; }
	
	float Amplitude{ set; }
	
	bool AllowedNegative{ set; } 
	
	int Octave{ set; }

	int Seed{set;}

	float getNoise(float x, float y);

}
