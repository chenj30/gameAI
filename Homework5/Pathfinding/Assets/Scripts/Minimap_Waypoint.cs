using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// small struct for nodes
public struct WaypointAStarNode
{
	public int x;
	public int y;
	public int size;
	public List<int> neighbors;
	public float distance;
	public float heuristic;
	public int parent;
}

// comparer class to use with queue
public class WaypointAStarComparer : IComparer<int>
{
	private Minimap_Waypoint _waypointAStar;
	private float _heuristicWeight;

	public WaypointAStarComparer(Minimap_Waypoint waypointAStar, float heuristicWeight)
	{
		_waypointAStar = waypointAStar;
		_heuristicWeight = heuristicWeight;
	}

	public int Compare(int a, int b)
	{
		float waypointA = (_waypointAStar.Get(a).distance + _waypointAStar.Get(a).heuristic * _heuristicWeight);
		float waypointB = (_waypointAStar.Get(b).distance + _waypointAStar.Get(b).heuristic * _heuristicWeight);
		if (waypointA < waypointB) { return -1; }
		else if (waypointA > waypointB) { return 1; }
		return 0;
	}
}

public class Minimap_Waypoint {

	public const int INFINITY = 99999;

	private WaypointAStarNode[] _nodes;
	public int NumNodes { get { return _nodes.Length; } }

	public WaypointAStarNode Get(int i)
	{
		return _nodes[i];
	}

	public void Clear()
	{
		_nodes = null;
	}

	// Checks if specific waypoint is passable
	public static bool IsPassable(TileMap tileMap, int minX, int minY, int width, int height)
	{
		if (minX + width >= tileMap.Width || minY + height >= tileMap.Height)
			return false;

		for (int y = minY; y < Mathf.Min(tileMap.Height, minY + height); ++y)
		{
			for (int x = minX; x < Mathf.Min(tileMap.Width, minX + width); ++x)
			{
				if (tileMap.Get(x, y).z != 0)
					return false;
			}
		}

		return true;
	}

	public void LoadFromTileMap(TileMap tileMap, int nodeSizeX, int nodeSizeY)
	{
		List<WaypointAStarNode> genNodes = new List<WaypointAStarNode>();

		int width = (tileMap.Width + nodeSizeX - 1) / nodeSizeX;
		int height = (tileMap.Height + nodeSizeY - 1) / nodeSizeY;
		int[] cover = new int[width * height];

		int index = 0;
		for (int y = 0; y < height; ++y)
			for (int x = 0; x < width; ++x, ++index)
				cover[index] = (IsPassable(tileMap, x * nodeSizeX, y * nodeSizeY, nodeSizeX, nodeSizeY) ? 0 : -1);

		int nodeCount = 0;
		index = 0;
		while (index < width * height)
		{
			if (cover[index] == 0)
			{
				int x = index % width;
				int y = index / width;

				++nodeCount;
				cover[index] = nodeCount;
				int size = 1;
				while (x + size < width && y + size < height)
				{
					bool passable = true;

					for (int i = 0; i <= size && passable; ++i)
						if (cover[x + i + (y + size) * width] != 0 || cover[x + size + (y + i) * width] != 0)
							passable = false;

					if (passable)
					{
						for (int i = 0; i <= size; ++i)
							cover[x + i + (y + size) * width] = cover[x + size + (y + i) * width] = nodeCount;
						++size;
					}
					else
						break;
				}
				WaypointAStarNode node = new WaypointAStarNode();
				node.x = x;
				node.y = y;
				node.size = size;
				node.neighbors = new List<int>();
				genNodes.Add(node);
			}
			++index;
		}

		for (int i = 0; i < genNodes.Count; ++i)
		{
			WaypointAStarNode a = genNodes[i];
			int llax = a.x - 1;
			int llay = a.y - 1;
			int urax = a.x + a.size;
			int uray = a.y + a.size;
			for (int j = i + 1; j < genNodes.Count; ++j)
			{
				WaypointAStarNode b = genNodes[j];
				int llbx = b.x;
				int llby = b.y;
				int urbx = b.x + b.size - 1;
				int urby = b.y + b.size - 1;

				if (llax <= urbx && llay <= urby && urax >= llbx && uray >= llby)
				{
					genNodes[i].neighbors.Add(j);
					genNodes[j].neighbors.Add(i);
				}
			}
		}

		_nodes = genNodes.ToArray();
	}

	public void RunAStar(int startX, int startY, int endX, int endY, AStarHeuristic heuristic, float heuristicWeight)
	{
		int startIndex = FindContainingNode(startX, startY);
		int endIndex = FindContainingNode(endX, endY);

		for (int i = 0; i < NumNodes; ++i)
		{
			if (i == startIndex)
				_nodes[i].distance = 0.0f;
			else
				_nodes[i].distance = INFINITY;

			switch (heuristic)
			{
				case AStarHeuristic.None:
					_nodes[i].heuristic = 0.0f;
					break;
				case AStarHeuristic.Manhattan:
					_nodes[i].heuristic =
						Mathf.Abs((_nodes[i].x + _nodes[i].size / 2.0f) - (_nodes[endIndex].x + _nodes[endIndex].size / 2.0f)) +
						Mathf.Abs((_nodes[i].y + _nodes[i].size / 2.0f) - (_nodes[endIndex].y + _nodes[endIndex].size / 2.0f));
					break;
				case AStarHeuristic.Euclidean:
					_nodes[i].heuristic = Mathf.Sqrt(
						Mathf.Pow((_nodes[i].x + _nodes[i].size / 2.0f) - (_nodes[endIndex].x + _nodes[endIndex].size / 2.0f), 2.0f) +
						Mathf.Pow((_nodes[i].y + _nodes[i].size / 2.0f) - (_nodes[endIndex].y + _nodes[endIndex].size / 2.0f), 2.0f)
					);
					break;
				default:
					break;
			}

			_nodes[i].parent = -1;
		}

		if (endIndex == -1 || startIndex == endIndex)
			return;

		PriorityQueue<int> queue = new PriorityQueue<int>(new WaypointAStarComparer(this, heuristicWeight));
		queue.Enqueue(startIndex);

		while (queue.Count > 0 && _nodes[endIndex].parent == -1)
		{
			int index = queue.Dequeue();

			for (int i = 0; i < _nodes[index].neighbors.Count; ++i)
			{
				int nIndex = _nodes[index].neighbors[i];
				float distance = Mathf.Sqrt(
					Mathf.Pow((_nodes[index].x + _nodes[index].size / 2.0f) - (_nodes[nIndex].x + _nodes[nIndex].size / 2.0f), 2.0f) +
					Mathf.Pow((_nodes[index].y + _nodes[index].size / 2.0f) - (_nodes[nIndex].y + _nodes[nIndex].size / 2.0f), 2.0f)
				);
				if (_nodes[nIndex].distance > _nodes[index].distance + distance)
				{
					_nodes[nIndex].distance = _nodes[index].distance + distance;
					_nodes[nIndex].parent = index;
					queue.Enqueue(nIndex);
				}
			}
		}
	}

	public int FindContainingNode(int x, int y)
	{
		for (int i = 0; i < NumNodes; ++i)
		{
			WaypointAStarNode n = _nodes[i];
			if (x >= n.x && x < n.x + n.size && y >= n.y && y < n.y + n.size)
				return i;
		}

		return -1;
	}
}
