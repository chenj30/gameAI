using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class RadiusDrawer : MonoBehaviour {

	private int segments = 50;
	private float radius = 0;
	private LineRenderer lineRender;

	void CreatePoints()
	{
		float x, y;
		float angle = 20f;

		for (int i = 0; i < (segments+1); i++)
		{
			x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
			y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

			lineRender.SetPosition(i, new Vector3(x, y, 0));

			angle += (360f / segments);
		}
	}

	// Use this for initialization
	void Start () {
		lineRender = gameObject.GetComponent<LineRenderer>();
		lineRender.SetVertexCount(segments + 1);
		lineRender.useWorldSpace = false;
	}
	

	// Update is called once per frame
	void Update () {
		radius = gameObject.GetComponent<AIMovement>().arriveRadius;
		CreatePoints();
	}
}
