using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class MinimapPathRenderer_Tile : MonoBehaviour {

	private LineRenderer _lineRenderer;

	public void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
	}

	private Vector3 GetIndexCoords(TileAStar tileAStar, int index, int nodeSizeX, int nodeSizeY)
	{
		return new Vector3((index % tileAStar.Width + 0.5f) * nodeSizeX, (index / tileAStar.Width + 0.5f) * nodeSizeY, 100);
	}

	public void Render(TileAStar tileAStar, int endX, int endY, int nodeSizeX, int nodeSizeY, Material pathMaterial)
	{
		List<Vector3> path = new List<Vector3>();
		int index = endX + endY * tileAStar.Width;
		while (index != -1)
		{
			path.Add(GetIndexCoords(tileAStar, index, nodeSizeX, nodeSizeY));
			index = tileAStar.Get(index).parent;
		}

		if (path.Count < 2)
			return;

		const float cornerSharpness = 0.75f;
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
