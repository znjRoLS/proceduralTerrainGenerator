using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomPointGenerator : PointGenerator{

	public List<Vector2> generate(int numPoints, Vector2 size, int boundaryOffset){
		List<Vector2> points = new List<Vector2> ();
		int x, y;

		while (numPoints-- != 0) {
			x = (int) Random.Range(0+boundaryOffset,size.x-boundaryOffset);
			y = (int) Random.Range(0+boundaryOffset,size.y-boundaryOffset);
			points.Add(new Vector2(x,y));
		}	

		return points;
		
	}

}
