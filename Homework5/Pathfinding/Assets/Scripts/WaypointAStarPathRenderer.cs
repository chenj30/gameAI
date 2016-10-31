using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WaypointAStarPathRenderer : MonoBehaviour {

	private LineRenderer _lineRenderer;

	public void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
	}

	private Vector3 GetIndexCoords(WaypointAStar waypointAStar, int index, int nodeSizeX, int nodeSizeY)
	{
		return new Vector3(
			(waypointAStar.Get(index).x + waypointAStar.Get(index).size / 2.0f) * nodeSizeX,
			(waypointAStar.Get(index).y + waypointAStar.Get(index).size / 2.0f) * nodeSizeY,
			100
		);
	}

	public void Render(WaypointAStar waypointAStar, int startX, int startY, int endX, int endY, int nodeSizeX, int nodeSizeY, Material pathMaterial)
	{
		List<Vector3> path = new List<Vector3>();
		int index = waypointAStar.FindContainingNode(endX, endY);
		if (index == -1 || waypointAStar.Get(index).parent == -1)
			return;

		path.Add(new Vector3((endX + 0.5f) * nodeSizeX, (endY + 0.5f) * nodeSizeY, 100));
		while (index != -1)
		{
			path.Add(GetIndexCoords(waypointAStar, index, nodeSizeX, nodeSizeY));
			index = waypointAStar.Get(index).parent;
		}
		path.Add(new Vector3((startX + 0.5f) * nodeSizeX, (startY + 0.5f) * nodeSizeY, 100));

		const float cornerSharpness = 0.85f;
		_lineRenderer.SetVertexCount(path.Count * 2 - 2);
		_lineRenderer.SetPosition(0, path[0]);
		for (int i = 1; i < path.Count - 1; ++i)
		{
			_lineRenderer.SetPosition(2 * i - 1, Vector3.Lerp(path[i - 1], path[i], cornerSharpness));
			_lineRenderer.SetPosition(2 * i, Vector3.Lerp(path[i + 1], path[i], cornerSharpness));
		}
		_lineRenderer.SetPosition(path.Count * 2 - 3, path[path.Count - 1]);

		_lineRenderer.sharedMaterial = pathMaterial;
	}
}
