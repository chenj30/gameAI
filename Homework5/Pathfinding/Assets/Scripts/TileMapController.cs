using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum GUIState
{
	Normal,
	PickStartPoint,
	PickEndPoint,
	AddObstacles,
	RemoveObstacles
}

public enum WorldRepresenation
{
	Tile,
	Waypoint
}

public enum AStarHeuristic
{
	None,
	Manhattan,
	Euclidean
}

[RequireComponent(typeof(TileMapRenderer))]
[RequireComponent(typeof(TileAStarController))]
[RequireComponent(typeof(WaypointAStarController))]
public class TileMapController : MonoBehaviour
{

	public string mapFilePath;
	public Text heuristicTextDisplay;
	public Text methodTextDisplay;
	public InputField mapPathInput;
	public GameObject startPointPrefab;
	public GameObject endPointPrefab;

	public TileMap tileMap;

	private TileMapRenderer _renderer;
	private TileAStarController _aStarControllerTile;
	private WaypointAStarController _aStarControllerWaypoint;
	private GameObject _startPointGO;
	private GameObject _endPointGO;
	private GUIState _guiState;
	private WorldRepresenation _worldRep;

	public void Awake()
	{
		tileMap = new TileMap();

		_renderer = GetComponent<TileMapRenderer>();
		_aStarControllerTile = GetComponent<TileAStarController>();
		_aStarControllerWaypoint = GetComponent<WaypointAStarController>();

		_startPointGO = GameObject.Instantiate(startPointPrefab);
		_startPointGO.transform.SetParent(transform);
		_endPointGO = GameObject.Instantiate(endPointPrefab);
		_endPointGO.transform.SetParent(transform);

		_guiState = GUIState.Normal;
		_worldRep = WorldRepresenation.Tile;

	}

	// Use this for initialization
	void Start()
	{
		LoadTileMap();
	}

	void Update()
	{
		if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			Vector3 pos = Input.mousePosition;
			pos.z = 0;
			pos = Camera.main.ScreenToWorldPoint(pos);

			int tileX = Mathf.FloorToInt(pos.x);
			int tileY = Mathf.FloorToInt(pos.y);

			switch (_guiState)
			{
				case GUIState.Normal:
					break;
				case GUIState.PickStartPoint:
					SetStartPoint(tileX, tileY);
					break;
				case GUIState.PickEndPoint:
					SetEndPoint(tileX, tileY);
					break;
				case GUIState.AddObstacles:
					SetTile(tileX, tileY, 2);
					break;
				case GUIState.RemoveObstacles:
					SetTile(tileX, tileY, 0);
					break;
				default:
					break;
			}
		}
	}

	public void SetMapPath(string path)
	{
		mapFilePath = path;
	}

	public void LoadTileMap()
	{
		if (tileMap.LoadMap(mapFilePath) == 0)
		{
			_renderer.Clear();
			_renderer.Render();

			_aStarControllerTile.Clear();
			_aStarControllerWaypoint.Clear();

			Camera.main.transform.position = new Vector3(tileMap.width / 2.0f, tileMap.height / 2.0f, -10.0f);
		}
	}

	public void PerformAStar()
	{
		switch (_worldRep)
		{
			case WorldRepresenation.Tile:
				_aStarControllerTile.PerformAStar();
				break;
			case WorldRepresenation.Waypoint:
				_aStarControllerWaypoint.PerformAStar();
				break;
			default:
				break;
		}
	}

	public void SetStartPoint(int x, int y)
	{
		_aStarControllerTile.SetAStarStart(x, y);
		_aStarControllerWaypoint.SetAStarStart(x, y);
		_startPointGO.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, 50);
		FinishSelection();
	}

	public void SetEndPoint(int x, int y)
	{
		_aStarControllerTile.SetAStarEnd(x, y);
		_aStarControllerWaypoint.SetAStarEnd(x, y);
		_endPointGO.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, 50);
		FinishSelection();
	}

	public void SetNodeWidth(float width)
	{
		_aStarControllerTile.nodeSizeX = Mathf.RoundToInt(width);
		_aStarControllerWaypoint.nodeSizeX = Mathf.RoundToInt(width);
	}

	public void SetNodeHeight(float height)
	{
		_aStarControllerTile.nodeSizeY = Mathf.RoundToInt(height);
		_aStarControllerWaypoint.nodeSizeY = Mathf.RoundToInt(height);
	}

	public void SetTile(int x, int y, int value)
	{
		if (x < 0 || y < 0 || x >= tileMap.width || y >= tileMap.height) { return; }

		tileMap.Set(x, y, value);

		_renderer.OnChanged(x, y);
	}

	public void SetHeuristicWeight(float weight)
	{
		_aStarControllerTile.SetAStarHeuristicWeight(weight);
		_aStarControllerWaypoint.SetAStarHeuristicWeight(weight);
	}

	public void SetAStarHeuristicType(float type)
	{
		switch (Mathf.RoundToInt(type * 2.0f))
		{
			case 0:
				_aStarControllerTile.Heuristic = AStarHeuristic.None;
				_aStarControllerWaypoint.heuristic = AStarHeuristic.None;
				heuristicTextDisplay.text = "None";
				break;
			case 1:
				_aStarControllerTile.heuristic = AStarHeuristic.Manhattan;
				_aStarControllerWaypoint.heuristic = AStarHeuristic.Manhattan;
				heuristicTextDisplay.text = "Manhattan";
				break;
			case 2:
				_aStarControllerTile.heuristic = AStarHeuristic.Euclidean;
				_aStarControllerWaypoint.heuristic = AStarHeuristic.Euclidean;
				heuristicTextDisplay.text = "Euclidean";
				break;
			default:
				break;
		}
	}

	

	public void FinishSelection()
	{
		_guiState = GUIState.Normal;
	}

	public void StartPointSelection()
	{
		_guiState = GUIState.PickStartPoint;
	}

	public void EndPointSelection()
	{
		_guiState = GUIState.PickEndPoint;
	}

	public void AddObtaclesSelection()
	{
		_guiState = GUIState.AddObstacles;
	}

	public void RemoveObstaclesSelection()
	{
		_guiState = GUIState.RemoveObstacles;
	}
}
