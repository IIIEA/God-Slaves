using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EdgeVertices
{
    public Vector3 V1, V2, V3, V4;

	public EdgeVertices(Vector3 corner1, Vector3 corner2)
	{
		V1 = corner1;
		V2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
		V3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
		V4 = corner2;
	}
}
