using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MinimapRenderer_Tile))]
public class MinimapController_Tile : MonoBehaviour {

	public int nodeSizeX = 2;
	public int nodeSizeY = 2;
	public AStarHeuristic heuristicType;
	public float heuristicWeight = 1;

	public TileAStar tileAStar;

	private TileMapController _controller;
	private MinimapRenderer_Tile _renderer;
	private int _startX;
	private int _startY;
	private int _endX;
	private int _endY;

	public void Awake()
	{
		tileAStar = new TileAStar();

		_controller = GetComponent<TileMapController>();
		_renderer = GetComponent<MinimapRenderer_Tile>();
	}

	// Clears the tile data and renderer data
	public void Clear()
	{
		tileAStar.Clear();
		_renderer.Clear();
	}

	// Performs A* Algorithm
	public void RunAStar()
	{
		int startX = _startX / nodeSizeX;
		int startY = _startY / nodeSizeY;
		int endX = _endX / nodeSizeX;
		int endY = _endY / nodeSizeY;

		Clear();

		// A*
		tileAStar.LoadFromTileMap(_controller.tileMap, nodeSizeX, nodeSizeY);
		tileAStar.RunAStar(startX, startY, endX, endY, heuristicType, heuristicWeight);

		// Render
		_renderer.Render(startX, startY, endX, endY);
	}

	public void SetAStarStart(int x, int y)
	{
		_startX = x;
		_startY = y;
	}

	public void SetAStarEnd(int x, int y)
	{
		_endX = x;
		_endY = y;
	}

	public void SetAStarHeuristicWeight(float weight)
	{
		heuristicWeight = weight;
	}


}
