using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MinimapTileRenderer_Waypoint : MonoBehaviour {

	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;
	private Mesh _mesh;

	public void Awake()
	{
		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_mesh = new Mesh();
	}

	public void Render(Minimap_Waypoint waypointAStar, int startNode, int nodeCount, int nodeSizeX, int nodeSizeY, float distance, Material aStarMaterial)
	{
		nodeCount = Mathf.Min(nodeCount, waypointAStar.NumNodes - startNode);

		const int maxVerts = 65000;
		if (nodeCount * 4 > maxVerts)
		{
			Debug.LogError("Render tile dimensions too large: " + nodeCount + " * 4 = " + (nodeCount * 4) + " (> " + maxVerts + ")");
			return;
		}

		Vector3[] verts = new Vector3[nodeCount * 4];
		Vector2[] uvs = new Vector2[nodeCount * 4];
		Color32[] colors = new Color32[nodeCount * 4];
		int[] tris = new int[nodeCount * 6];

		for (int i = 0; i < nodeCount; ++i)
		{
			int index = i + startNode;

			int x0 = waypointAStar.Get(index).x * nodeSizeX;
			int x1 = x0 + waypointAStar.Get(index).size * nodeSizeX;
			int y0 = waypointAStar.Get(index).y * nodeSizeY;
			int y1 = y0 + waypointAStar.Get(index).size * nodeSizeY;

			Color32 c;
			if (waypointAStar.Get(index).distance < 0.0f)
				c = Color.clear;
			else if (waypointAStar.Get(index).distance >= Minimap_Waypoint.INFINITY)
				c = new Color(0.5f, 0.5f, 1.0f, 1.0f);
			else
			{
				float value = Mathf.Clamp01(waypointAStar.Get(index).distance / distance);
				c = new Color(1.0f, value, value, 1.0f);
			}

			verts[i * 4 + 0] = new Vector3(x0, y0, 0);
			uvs[i * 4 + 0] = new Vector2(0, 0);
			colors[i * 4 + 0] = c;

			verts[i * 4 + 1] = new Vector3(x0, y1, 0);
			uvs[i * 4 + 1] = new Vector2(0, 1);
			colors[i * 4 + 1] = c;

			verts[i * 4 + 2] = new Vector3(x1, y1, 0);
			uvs[i * 4 + 2] = new Vector2(1, 1);
			colors[i * 4 + 2] = c;

			verts[i * 4 + 3] = new Vector3(x1, y0, 0);
			uvs[i * 4 + 3] = new Vector2(1, 0);
			colors[i * 4 + 3] = c;

			tris[i * 6 + 0] = i * 4 + 0;
			tris[i * 6 + 1] = i * 4 + 1;
			tris[i * 6 + 2] = i * 4 + 2;
			tris[i * 6 + 3] = i * 4 + 2;
			tris[i * 6 + 4] = i * 4 + 3;
			tris[i * 6 + 5] = i * 4 + 0;
		}

		_mesh.Clear();
		_mesh.vertices = verts;
		_mesh.uv = uvs;
		_mesh.colors32 = colors;
		_mesh.triangles = tris;

		_meshFilter.sharedMesh = _mesh;
		_meshRenderer.sharedMaterial = aStarMaterial;
	}
}