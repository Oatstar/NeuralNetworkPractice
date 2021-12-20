using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
	[SerializeField] GameObject edgeTop;
	[SerializeField] GameObject edgeRight;
	[SerializeField] GameObject edgeBot;
	[SerializeField] GameObject edgeLeft;

	public Vector2 GetEdgePosition(int posDirection)
	{
		GameObject comparableEdge = edgeTop;

		switch (posDirection)
		{
			case 0:
				comparableEdge = edgeTop;
				break;
			case 1:
				comparableEdge = edgeRight;
				break;
			case 2:
				comparableEdge = edgeBot;
				break;
			case 3:
				comparableEdge = edgeLeft;
				break;
		}

		return comparableEdge.transform.position;
	}
}
