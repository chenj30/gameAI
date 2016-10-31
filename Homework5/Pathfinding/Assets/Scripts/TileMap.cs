using UnityEngine;
using System.Collections;
using System.IO;

public class TileMap {

	private Vector3[,] _data;
	private int _width;
	public int Width { get { return _width; } }
	private int _height;
	public int Height {  get { return _height; } }

	public Vector3 Get(int x, int y)
	{
		return _data[x, y];
	}

	public void Set(int x, int y, int value)
	{
		_data[x, y] = new Vector3(x, y, value);
	}

	public void Clear()
	{
		_data = null;
		_width = 0;
		_height = 0;
	}

	public void Resize(int newWidth, int newHeight)
	{
		Clear();
		_width = newWidth;
		_height = newHeight;
		_data = new Vector3[_width, _height];
	}

	public int LoadMap(string filepath)
	{
		TextAsset mapText = Resources.Load<TextAsset>(filepath);
		if (mapText == null)
		{
			Debug.LogError("Unable to load text asset \"" + filepath + "\"");
			return 1;
		}

		MemoryStream stream = new MemoryStream(mapText.bytes);
		StreamReader reader = new StreamReader(stream);

		string typeLine = reader.ReadLine();
		string heightLine = reader.ReadLine();
		string widthLine = reader.ReadLine();
		string mapLine = reader.ReadLine();

		if (typeLine != "type octile")
		{
			Debug.LogError("Invalid tile map type line, expected \"type octile\", got \"" + typeLine + "\"");
			return 2;
		}
		if (mapLine != "map")
		{
			Debug.LogError("Invalid tile map map line, expected \"map\", got \"" + mapLine + "\"");
			return 3;
		}

		Resize(int.Parse(widthLine.Split(' ')[1]), int.Parse(heightLine.Split(' ')[1]));

		for (int y = _height - 1; y >= 0; --y)
		{
			string line = reader.ReadLine();
			for (int x = 0; x < _width; ++x)
			{
				switch (line[x])
				{
					case '@':
						Set(x, y, 2);
						break;
					case 'T':
						Set(x, y, 1);
						break;
					case '.':
						Set(x, y, 0);
						break;
					default:
						Debug.LogError("Invalid tile map tile, expected '@', 'T', or '.', got '" + line[x] + "'");
						return 4;
				}
			}
		}

		return 0;
	}
}
