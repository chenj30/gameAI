using UnityEngine;
using System.Collections;

public class FormationMovement : MonoBehaviour {

	public float maxSpeed = 1;
	public float maxAcceleration = 1;
	[Header("Pathfinding")]
	public PathDrawer path;
	public float nodeArriveRadius = 1;

	private Vector3 _velocity = new Vector3(1, 0);
	public Vector3 Velocity { get { return _velocity; } }
	private Vector3 _acceleration = Vector3.zero;
	public Vector3 Acceleration { get { return _acceleration; } }

	private int _currentNode = 0;
	private Transform _pathTarget;

	Vector3 Clip(Vector3 vector, float max)
	{
		float i;
		i = max / vector.magnitude;
		if (i > 1.0f) { i = 1.0f; }
		return vector *= i;
	}

	float calcDistance(Vector3 a, Vector3 b)
	{
		return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
	}

	Vector3 Seek(Vector3 targetLoc, float targetArriveRadius)
	{
		Vector3 desiredVelocity = targetLoc - this.transform.position;
		if (desiredVelocity.magnitude < targetArriveRadius)
		{
			desiredVelocity = desiredVelocity.normalized * maxSpeed * (desiredVelocity.magnitude / targetArriveRadius);
		}
		else { desiredVelocity = desiredVelocity.normalized * maxSpeed; }
		Vector3 force = desiredVelocity - _velocity;
		return force;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
