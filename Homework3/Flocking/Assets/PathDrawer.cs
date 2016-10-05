using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class PathDrawer : MonoBehaviour {

	public Transform[] nodes;
	private LineRenderer lineRender;

	void CreateNodes()
	{
		for (int i = 0; i < nodes.Length; i++)
		{
			Vector3 position = new Vector3(nodes[i].position.x, nodes[i].position.y, 0);
			lineRender.SetPosition(i, position);
		}
	}

	// Use this for initialization
	void Start () {
		lineRender = gameObject.GetComponent<LineRenderer>();
		lineRender.SetVertexCount(nodes.Length);
	}
	
	// Update is called once per frame
	void Update () {
		CreateNodes();
	}
}
