using UnityEngine;
using System.Collections;

public class TileMapRenderer : MonoBehaviour {

	public int rTileSizeX = 32;
	public int rTileSizeY = 32;
	public Material renderMat;
	
	public int NumTilesX { get { return (_controller.tileMap.Width + rTileSizeX - 1) / rTileSizeX; } }
	public int NumTilesY { get { return (_controller.tileMap.Height + rTileSizeY - 1) / rTileSizeY; } }

	private TileMapController _controller;
	private GameObject _renderRoot;
	private TileMapRenderTile[,] _rTiles;

	public void Awake()
	{
		_controller = GetComponent<TileMapController>();
		_renderRoot = new GameObject();
		_renderRoot.name = "Render Root";
		_renderRoot.transform.SetParent(transform);
		_renderRoot.transform.localPosition = new Vector3(0, 0, 200);
		_rTiles = null;
	}

	public void Clear()
	{
		if (_rTiles != null)
		{
			foreach (TileMapRenderTile rTile in _rTiles)
			{
				GameObject.Destroy(rTile.gameObject);
			}
			_rTiles = null;
		}
	}

	public void Render()
	{
		_rTiles = new TileMapRenderTile[NumTilesX, NumTilesY];
		for (int y = 0; y < NumTilesY; ++y)
		{
			for (int x = 0; x < NumTilesX; ++x)
			{
				GameObject rTileGO = new GameObject();
				rTileGO.name = "Render tile " + x + "," + y;
				TileMapRenderTile rTile = rTileGO.AddComponent<TileMapRenderTile>();
				rTile.Render(_controller.tileMap, x * rTileSizeX, y * rTileSizeY, rTileSizeX, rTileSizeY, renderMat);
				_rTiles[x, y] = rTile;
				rTileGO.transform.SetParent(_renderRoot.transform);
				rTileGO.transform.localPosition = Vector3.zero;
			}
		}
	}

	public void OnChanged(int x, int y)
	{
		int rTileX = x / rTileSizeX;
		int rTileY = y / rTileSizeY;
		_rTiles[rTileX, rTileY].Render(_controller.tileMap, rTileX * rTileSizeX, rTileY * rTileSizeY, rTileSizeX, rTileSizeY, renderMat);
	}
}
