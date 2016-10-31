using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// small struct for nodes
public struct TileAStarNode
{
	public int x;
	public int y;
	public float distance;
	public float heuristic;
	public int parent;
}

// comparer class to use with queue
public class TileAStarComparer : IComparer<int>
{
	private TileAStar _tileAStar;
	private float _heuristicWeight;

	public TileAStarComparer(TileAStar tileAStar, float heuristicWeight)
	{
		_tileAStar = tileAStar;
		_heuristicWeight = heuristicWeight;
	}

	public int Compare(int a, int b)
	{
		float tileA = (_tileAStar.Get(a).distance + _tileAStar.Get(a).heuristic * _heuristicWeight); 
		float tileB = (_tileAStar.Get(b).distance + _tileAStar.Get(b).heuristic * _heuristicWeight);
		if (tileA < tileB) { return -1; }
		else if (tileA > tileB) { return 1; }
		return 0;
	}
}

// A Star class to hold graph information and run A Star algorithm
public class TileAStar {

	const int INFINITY = 99999;

	private int _width;
	public int Width { get { return _width; } }
	private int _height;
	public int Height { get { return _height; } }
	private TileAStarNode[] _nodes;
	private int _numNodes;
	public int NumNodes { get { return _width * _height; } }

	// Clears graph by emptying list of nodes
	public void Clear()
	{
		_nodes = null;
		_width = 0;
		_height = 0;
	}

	// Gets node at index i
	public TileAStarNode Get(int i)
	{
		return _nodes[i];
	}

	// Checks if specific tile is passable
	public static bool IsPassable(TileMap tileMap, int minX, int minY, int width, int height)
	{
		if (minX + width >= tileMap.width || minY + height >= tileMap.height)
			return false;

		for (int y = minY; y < Mathf.Min(tileMap.height, minY + height); ++y)
		{
			for (int x = minX; x < Mathf.Min(tileMap.width, minX + width); ++x)
			{
				if (tileMap.Get(x, y).z != 0)
					return false;
			}
		}

		return true;
	}

	// Load graph data from tile map
	public void LoadFromTileMap(TileMap tileMap, int nodeSizeX, int nodeSizeY)
	{
		_width = (tileMap.width + nodeSizeX - 1) / nodeSizeX;
		_height = (tileMap.height + nodeSizeY - 1) / nodeSizeY;
		_nodes = new TileAStarNode[_width * _height];

		int index = 0;
		for (int y = 0; y < _height; ++y)
		{
			for (int x = 0; x < _width; ++x, ++index)
			{
				_nodes[index].x = x;
				_nodes[index].y = y;

				if (!IsPassable(tileMap, x * nodeSizeX, y * nodeSizeY, nodeSizeX, nodeSizeY))
					_nodes[index].distance = -1.0f;
				else
					_nodes[index].distance = INFINITY;

				_nodes[index].heuristic = 0.0f;
				_nodes[index].parent = -1;
			}
		}
	}

	// Perform A Star algorithm
	public void RunAStar(int startX, int startY, int endX, int endY, AStarHeuristic  heuristic, float heuristicWeight)
	{
		int startIndex = startX + startY * _width;
		int endIndex = endX + endY * _width;

		int index = 0;
		for (int y = 0; y < _height; ++y)
		{
			for (int x = 0; x < _width; ++x, ++index)
			{
				switch (heuristic)
				{
					case AStarHeuristic.None:
						_nodes[index].heuristic = 0.0f;
						break;
					case AStarHeuristic.Manhattan:
						_nodes[index].heuristic = Mathf.Abs(endX - x) + Mathf.Abs(endY - y);
						break;
					case AStarHeuristic.Euclidean:
						_nodes[index].heuristic = Mathf.Sqrt(Mathf.Pow(endX - x, 2) + Mathf.Pow(endY - y, 2));
						break;
					default:
						break;
				}
			}
		}
		_nodes[startIndex].distance = 0.0f;

		if (startIndex == endIndex)
			return;

		PriorityQueue<int> queue = new PriorityQueue<int>(new TileAStarComparer(this, heuristicWeight));
		queue.Enqueue(startIndex);

		while (queue.Count > 0 && _nodes[endIndex].parent == -1)
		{
			index = queue.Dequeue();
			int curX = index % _width;
			int curY = index / _width;

			AStarHelper(curX, curY, curX + 1, curY, queue);
			AStarHelper(curX, curY, curX, curY + 1, queue);
			AStarHelper(curX, curY, curX - 1, curY, queue);
			AStarHelper(curX, curY, curX, curY - 1, queue);
		}
	}

	// Healper function to check if distance is shorter
	private void AStarHelper(int fromX, int fromY, int toX, int toY, PriorityQueue<int> queue)
	{
		if (toX < 0 || toX >= _width || toY < 0 || toY >= _height)
			return;

		int fromIndex = fromX + fromY * _width;
		int toIndex = toX + toY * _width;
		float fromDistance = _nodes[fromIndex].distance;
		if (_nodes[toIndex].distance > fromDistance + 1.0f)
		{
			_nodes[toIndex].distance = fromDistance + 1.0f;
			_nodes[toIndex].parent = fromIndex;
			queue.Enqueue(toIndex);
		}
	}
}
