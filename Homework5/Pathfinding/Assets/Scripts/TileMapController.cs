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
		SetMapPath(mapFilePath);
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

		if (!mapPathInput.isFocused)
			MoveCamera(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
	}

	public void LoadTileMap()
	{
		SetMapPath(mapFilePath);
		if (tileMap.LoadMap(mapFilePath) == 0)
		{
			_renderer.Clear();
			_renderer.Render();

			_aStarControllerTile.Clear();
			_aStarControllerWaypoint.Clear();

			Camera.main.transform.position = new Vector3(tileMap.Width / 2.0f, tileMap.Height / 2.0f, -10.0f);
		}
	}

	public void PerformAStar()
	{
		switch (_worldRep)
		{
			case WorldRepresenation.Tile:
				_aStarControllerTile.RunAStar();
				break;
			case WorldRepresenation.Waypoint:
				_aStarControllerWaypoint.RunAStar();
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

	public void SetHeuristicWeight(float weight)
	{
		_aStarControllerTile.SetAStarHeuristicWeight(weight);
		_aStarControllerWaypoint.SetAStarHeuristicWeight(weight);
	}

	public void SetAStarHeuristicType(int type)
	{
		switch (type)
		{
			case 0:
				_aStarControllerTile.heuristicType = AStarHeuristic.None;
				_aStarControllerWaypoint.heuristicType = AStarHeuristic.None;
				break;
			case 1:
				_aStarControllerTile.heuristicType = AStarHeuristic.Manhattan;
				_aStarControllerWaypoint.heuristicType = AStarHeuristic.Manhattan;
				break;
			case 2:
				_aStarControllerTile.heuristicType = AStarHeuristic.Euclidean;
				_aStarControllerWaypoint.heuristicType = AStarHeuristic.Euclidean;
				break;
			default:
				break;
		}
	}

	public void SetWorldRep(int type)
	{
		switch (type)
		{
			case 0:
				_worldRep = WorldRepresenation.Tile;
				break;
			case 1:
				_worldRep = WorldRepresenation.Waypoint;
				break;
			default:
				break;
		}

		_aStarControllerTile.Clear();
		_aStarControllerWaypoint.Clear();
	}

	public void SetMapPath(string path)
	{
		mapFilePath = path;
		Debug.Log(mapFilePath);
	}

	public void SetTile(int x, int y, int value)
	{
		if (x < 0 || y < 0 || x >= tileMap.Width || y >= tileMap.Height) { return; }

		tileMap.Set(x, y, value);

		_renderer.OnChanged(x, y);
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

	public void MoveCamera(Vector2 delta)
	{
		if (delta.magnitude == 0.0f)
			return;

		const float cameraSpeed = 50.0f;
		delta *= cameraSpeed * Time.deltaTime / delta.magnitude;
		Camera.main.transform.position = new Vector3(
			Mathf.Clamp(Camera.main.transform.position.x + delta.x, 0, tileMap.Width),
			Mathf.Clamp(Camera.main.transform.position.y + delta.y, 0, tileMap.Height),
			-10.0f
		);
	}
}
