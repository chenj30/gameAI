using UnityEngine;
using System.Collections;

public class TileMapRenderer : MonoBehaviour {

	public int tileSizeX = 10;
	public int tileSizeY = 10;
	public Material renderMat;

	private int _numTilesX;
	public int NumTilesX { get { return (_controller.tileMap.Width + tileSizeX - 1) / tileSizeX; } }
	private int _numTilesY;
	public int NumTilesY { get { return (_controller.tileMap.Height + tileSizeY - 1) / tileSizeY; } }

	private TileMapController _controller;
	private GameObject _renderRoot;
	private TileMapRenderTile[,] _tiles;

	public void Awake()
	{
		_controller = GetComponent<TileMapController>();
		_renderRoot = new GameObject();
		_renderRoot.name = "Render Root";
		_renderRoot.transform.SetParent(transform);
		_renderRoot.transform.localPosition = new Vector3(0, 0, 200);
		_tiles = null;
	}

	public void Clear()
	{
		if (_tiles != null)
		{
			foreach (TileMapRenderTile tile in _tiles)
			{
				GameObject.Destroy(tile.gameObject);
			}
			_tiles = null;
		}
	}

	public void Render()
	{
		_tiles = new TileMapRenderTile[_numTilesX, _numTilesY];
		int index = 0;
		for (int y = 0; y < _numTilesY; ++y)
		{
			for (int x = 0; x < _numTilesX; ++x)
			{
				GameObject tileGO = new GameObject();
				tileGO.name = "Tile " + x + "," + y;
				TileMapRenderTile tile = tileGO.AddComponent<TileMapRenderTile>();
				tile.Render(_controller.tileMap, x * tileSizeX, y * tileSizeY, tileSizeX, tileSizeY, renderMat);
				_tiles[x, y] = tile;
				tileGO.transform.SetParent(_renderRoot.transform);
				tileGO.transform.localPosition = Vector3.zero;
			}
		}
	}

	public void OnChanged(int x, int y)
	{
		int tileX = x / tileSizeX;
		int tileY = y / tileSizeY;
		_tiles[tileX, tileY].Render(_controller.tileMap, tileX * tileSizeX, tileY * tileSizeY, tileSizeX, tileSizeY, renderMat);
	}
}
