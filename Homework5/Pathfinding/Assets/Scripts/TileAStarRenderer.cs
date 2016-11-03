using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileAStarRenderer : MonoBehaviour {

	public int rTileSizeX = 32;
	public int rTileSizeY = 32;
	public Material aStarMat;
	public Material pathMat;

	public int NumTilesX { get { return (_aStarController.tileAStar.Width + rTileSizeX - 1) / rTileSizeX; } }
	public int NumTilesY { get { return (_aStarController.tileAStar.Height + rTileSizeY - 1) / rTileSizeY; } }

	private TileAStarController _aStarController;
	private GameObject _renderRoot;
	private TileAStarRenderTile[] _rTiles;
	private TileAStarPathRenderer _pathRenderer;

	public void Awake()
	{
		_aStarController = GetComponent<TileAStarController>();
		_renderRoot = new GameObject();
		_renderRoot.name = "Tile A* Render Root";
		_renderRoot.transform.SetParent(this.transform);
		_renderRoot.transform.localPosition = new Vector3(0, 105, 100);
		_rTiles = null;
		_pathRenderer = null;
	}

	// Clear rendering of tile and path
	public void Clear()
	{
		if (_rTiles != null)
		{
			foreach (TileAStarRenderTile rTile in _rTiles)
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

	// Render and draws the tile and A* path
	public void Render(int startX, int startY, int endX, int endY)
	{
		int endIndex = endX + endY * _aStarController.tileAStar.Width;
		float distance = _aStarController.tileAStar.Get(endIndex).distance;

		// Tile
		_rTiles = new TileAStarRenderTile[NumTilesX * NumTilesY];
		int index = 0;
		for (int y = 0; y < NumTilesY; ++y)
		{
			for (int x = 0; x < NumTilesX; ++x, ++index)
			{
				GameObject rTileGO = new GameObject();
				rTileGO.name = "Tile A* Render tile " + x + "," + y;
				TileAStarRenderTile rTile = rTileGO.AddComponent<TileAStarRenderTile>();
				rTile.Render(_aStarController.tileAStar, x * rTileSizeX, y * rTileSizeY, rTileSizeX, rTileSizeY, _aStarController.nodeSizeX, _aStarController.nodeSizeY, distance, aStarMat);
				_rTiles[index] = rTile;
				rTileGO.transform.SetParent(_renderRoot.transform);
				rTileGO.transform.localPosition = new Vector3(0, 0, 50);
			}
		}

		// Path
		GameObject pathRendererGO = new GameObject();
		pathRendererGO.name = "Tile A* Path Renderer";
		_pathRenderer = pathRendererGO.AddComponent<TileAStarPathRenderer>();
		_pathRenderer.Render(_aStarController.tileAStar, endX, endY, _aStarController.nodeSizeX, _aStarController.nodeSizeY, pathMat);
		pathRendererGO.transform.SetParent(_renderRoot.transform);
		pathRendererGO.transform.localPosition = Vector3.zero;
	}
}
