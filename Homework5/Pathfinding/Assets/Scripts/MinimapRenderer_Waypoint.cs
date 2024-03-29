﻿using UnityEngine;
using System.Collections;

public class MinimapRenderer_Waypoint : MonoBehaviour {

	public int nodesPerTile = 1024;
	public Material aStarMat;
	public Material pathMat;
	public int NumTiles {  get { return (_aStarController.waypointAStar.NumNodes + nodesPerTile - 1) / nodesPerTile; } }

	private MinimapController_Waypoint _aStarController;
	private GameObject _renderRoot;
	private MinimapTileRenderer_Waypoint[] _rTiles;
	private MinimapPathRenderer_Waypoint _pathRenderer;

	public void Awake()
	{
		_aStarController = GetComponent<MinimapController_Waypoint>();
		_renderRoot = new GameObject();
		_renderRoot.name = "Waypoint A* Render Root";
		_renderRoot.transform.SetParent(this.transform);
		_renderRoot.transform.localPosition = new Vector3(0, 0, 100);
		_rTiles = null;
		_pathRenderer = null;
	}
	
	// Clear rendering of tile and path
	public void Clear()
	{
		if (_rTiles != null)
		{
			foreach (MinimapTileRenderer_Waypoint rTile in _rTiles)
			{
				GameObject.Destroy(rTile.gameObject);
			}
			_rTiles = null;
		}

		if (_pathRenderer != null)
		{
			GameObject.Destroy(_pathRenderer.gameObject);
			_pathRenderer = null;
		}
	}

	public void Render(int startX, int startY, int endX, int endY)
	{
		int endIndex = _aStarController.waypointAStar.FindContainingNode(endX, endY);
		if (endIndex == -1)
			return;
		float distance = _aStarController.waypointAStar.Get(endIndex).distance;

		// Tile
		_rTiles = new MinimapTileRenderer_Waypoint[NumTiles];
		for (int i = 0; i < NumTiles; ++i)
		{
			GameObject rTileGO = new GameObject();
			rTileGO.name = "Waypoint A* Render tile " + i;
			MinimapTileRenderer_Waypoint rTile = rTileGO.AddComponent<MinimapTileRenderer_Waypoint>();
			rTile.Render(_aStarController.waypointAStar, i * nodesPerTile, nodesPerTile, _aStarController.nodeSizeX, _aStarController.nodeSizeY, distance, aStarMat);

			_rTiles[i] = rTile;
			rTileGO.transform.SetParent(_renderRoot.transform);
			rTileGO.transform.localPosition = new Vector3(0, 0, 50);
		}

		// Path
		GameObject pathRendererGO = new GameObject();
		pathRendererGO.name = "Tile A* Path Renderer";
		_pathRenderer = pathRendererGO.AddComponent<MinimapPathRenderer_Waypoint>();
		_pathRenderer.Render(_aStarController.waypointAStar, startX, startY, endX, endY, _aStarController.nodeSizeX, _aStarController.nodeSizeY, pathMat);
		pathRendererGO.transform.SetParent(_renderRoot.transform);
		pathRendererGO.transform.localPosition = Vector3.zero;
	}
}
