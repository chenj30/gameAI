using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(WaypointAStarRenderer))]
public class WaypointAStarController : MonoBehaviour {

	public int nodeSizeX = 2;
	public int nodeSizeY = 2;
	public AStarHeuristic heuristicType;
	public float heuristicWeight = 1;

	public WaypointAStar waypointAStar;

	private TileMapController _controller;
	private WaypointAStarRenderer _renderer;
	private int _startX;
	private int _startY;
	private int _endX;
	private int _endY;

	public void Awake()
	{
		waypointAStar = new WaypointAStar();

		_controller = GetComponent<TileMapController>();
		_renderer = GetComponent<WaypointAStarRenderer>();
	}

	// Clears the tile data and renderer data
	public void Clear()
	{
		waypointAStar.Clear();
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
		waypointAStar.LoadFromTileMap(_controller.tileMap, nodeSizeX, nodeSizeY);
		waypointAStar.RunAStar(startX, startY, endX, endY, heuristicType, heuristicWeight);

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
