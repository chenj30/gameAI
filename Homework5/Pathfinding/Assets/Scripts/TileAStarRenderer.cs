using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileAStarRenderer : MonoBehaviour {

	public int tileSizeX = 10;
	public int tileSizeY = 10;
	public Material aStarMat;
	public Material pathMat;

	private int _numTilesX;
	public int NumTilesX { get { return (_aStarController.tileAStar.Width + tileSizeX - 1) / tileSizeX; } }
	private int _numTilesY;
	public int NumTilesY { get { return (_aStarController.tileAStar.Height + tileSizeY - 1) / tileSizeY; } }

	private TileAStarController _aStarController;
	private GameObject _renderRoot;
	private TileAStarRenderTile[] _tiles;
	private TileAStarPathRenderer _pathRenderer;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
