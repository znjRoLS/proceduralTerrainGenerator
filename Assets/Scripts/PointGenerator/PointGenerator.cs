using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface PointGenerator {

	List<Vector2> generate(int numPoints, Vector2 size, int boundaryOffset);

}
