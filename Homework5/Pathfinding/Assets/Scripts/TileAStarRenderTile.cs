using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileAStarRenderTile : MonoBehaviour {

	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;
	private Mesh _mesh;

	public void Awake()
	{
		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_mesh = new Mesh();
	}

	public void Render(TileAStar tileAStar, int minX, int minY, int width, int height, int nodeSizeX, int nodeSizeY, float distance, Material atlasMaterial)
	{
		width = Mathf.Min(width, tileAStar.Width - minX);
		height = Mathf.Min(height, tileAStar.Height - minY);

		const int maxVerts = 65000;
		if (width * height * 4 > maxVerts)
		{
			Debug.LogError("Render tile dimensions too large: " + width + " * " + height + " = " + (width * height * 4) + " (> " + maxVerts + ")");
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
				int nodeIndex = x + y * tileAStar.Width;

				Color32 c;
				if (tileAStar.Get(nodeIndex).distance < 0.0f)
					c = Color.clear;
				else if (tileAStar.Get(nodeIndex).distance >= TileAStar.INFINITY)
					c = new Color(0.5f, 0.5f, 1.0f, 1.0f);
				else
				{
					float value = Mathf.Clamp01(tileAStar.Get(nodeIndex).distance / distance);
					c = new Color(1.0f, value, value, 1.0f);
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
