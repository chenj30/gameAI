using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileAStarRenderer))]
public class TileAStarController : MonoBehaviour {

	public int nodeSizeX;
	public int nodeSizeY;
	public AStarHeuristic heuristicType;
	public float heuristicWeight;

	public TileAStar tileAStar;

	private TileMapController _controller;
	private TileAStarRenderer _renderer;
	private int _startX;
	private int _startY;
	private int _endX;
	private int _endY;

	public void Awake()
	{
		tileAStar = new TileAStar();

		_controller = GetComponent<TileMapController>();
		_renderer = GetComponent<TileAStarRenderer>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Clear()
	{
		tileAStar.Clear();
		_renderer.Clear();
	}



}
