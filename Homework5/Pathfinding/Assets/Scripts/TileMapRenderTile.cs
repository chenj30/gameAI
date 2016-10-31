using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileMapRenderTile : MonoBehaviour
{
	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;
	private Mesh _mesh;

	public void Awake()
	{
		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_mesh = new Mesh();
	}

	public void Render(TileMap tileMap, int minX, int minY, int width, int height, Material atlasMaterial)
	{
		width = Mathf.Min(width, tileMap.width - minX);
		height = Mathf.Min(height, tileMap.height - minY);

		const int maxVerts = 65000;
		if (width * height * 4 > maxVerts)
		{
			Debug.LogError("Chunk dimensions too large: " + width + " * " + height + " = " + (width * height * 4) + " (> " + maxVerts + ")");
			return;
		}

		Vector3[] verts = new Vector3[width * height * 4];
		Vector2[] uvs = new Vector2[width * height * 4];
		Color32[] colors = new Color32[width * height * 4];
		int[] tris = new int[width * height * 6];

		int index = 0;
		for (int y = minY; y < minY + height; ++y)
		{
			for (int x = minX; x < minX + width; ++x, ++index)
			{
				Color32 c;
				switch ((int)tileMap.Get(x, y).z)
				{
					case 0:
						c = new Color(1.0f, 1.0f, 1.0f, 1.0f);
						break;
					case 1:
						c = new Color(0.0f, 0.8f, 0.0f, 1.0f);
						break;
					case 2:
						c = new Color(0.0f, 0.4f, 0.0f, 1.0f);
						break;
					default:
						c = Color.magenta;
						break;
				}

				verts[index * 4 + 0] = new Vector3(x, y, 0);
				uvs[index * 4 + 0] = new Vector2(0, 0);
				colors[index * 4 + 0] = c;

				verts[index * 4 + 1] = new Vector3(x, y + 1, 0);
				uvs[index * 4 + 1] = new Vector2(0, 1);
				colors[index * 4 + 1] = c;

				verts[index * 4 + 2] = new Vector3(x + 1, y + 1, 0);
				uvs[index * 4 + 2] = new Vector2(1, 1);
				colors[index * 4 + 2] = c;

				verts[index * 4 + 3] = new Vector3(x + 1, y, 0);
				uvs[index * 4 + 3] = new Vector2(1, 0);
				colors[index * 4 + 3] = c;

				tris[index * 6 + 0] = index * 4 + 0;
				tris[index * 6 + 1] = index * 4 + 1;
				tris[index * 6 + 2] = index * 4 + 2;
				tris[index * 6 + 3] = index * 4 + 2;
				tris[index * 6 + 4] = index * 4 + 3;
				tris[index * 6 + 5] = index * 4 + 0;
			}
		}

		_mesh.Clear();
		_mesh.vertices = verts;
		_mesh.uv = uvs;
		_mesh.colors32 = colors;
		_mesh.triangles = tris;

		_meshFilter.sharedMesh = _mesh;
		_meshRenderer.sharedMaterial = atlasMaterial;
	}
}
