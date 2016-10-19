using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class PathDrawer : MonoBehaviour {

	public Transform[] _nodes;
	public Transform[] Nodes { get { return _nodes; } }
	private LineRenderer lineRender;

	void CreateNodes()
	{
		for (int i = 0; i < _nodes.Length; i++)
		{
			Vector3 position = new Vector3(_nodes[i].position.x, _nodes[i].position.y, 0);
			lineRender.SetPosition(i, position);
		}
	}

	// Use this for initialization
	void Start () {
		Transform[] objects = gameObject.GetComponentsInChildren<Transform>();
		_nodes = new Transform[objects.Length-1];
		int c = 0;
		foreach (Transform t in objects)
		{
			if (t.name.Contains("Node"))
			{
				_nodes[c] = t;
				c++;
			}
		}

		lineRender = gameObject.GetComponent<LineRenderer>();
		lineRender.SetVertexCount(_nodes.Length);
	}
	
	// Update is called once per frame
	void Update () {
		CreateNodes();
	}
}
