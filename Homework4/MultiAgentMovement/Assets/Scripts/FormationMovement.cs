using UnityEngine;
using System.Collections;

public class FormationMovement : MonoBehaviour {

	public float maxSpeed = 1;
	public float maxAcceleration = 1;
	public float arriveRadius = .5f;

	private Vector3 _velocity = new Vector3(1, 0);
	public Vector3 Velocity { get { return _velocity; } }
	private Vector3 _acceleration = Vector3.zero;
	public Vector3 Acceleration { get { return _acceleration; } }

	private Vector3 _targetLoc;
	private float _targetArriveRadius = 0.5f;

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

	Vector3 CheckCollision()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(this.transform.position, _velocity, out hitInfo, 2))
		{
			return Seek(_velocity + hitInfo.normal * arriveRadius, arriveRadius);
		}
		else
		{
			return Vector3.zero;
		}
	}

	public void DoSeek(Vector3 targetLoc, float targetArriveRadius)
	{
		_targetLoc = targetLoc;
		_targetArriveRadius = targetArriveRadius;	
	}

	// Use this for initialization
	void Start () {
		_targetLoc = this.gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		_acceleration = Seek(_targetLoc, _targetArriveRadius);
		_acceleration += CheckCollision();
		_acceleration = Clip(_acceleration, maxAcceleration);
		_velocity = Clip(_velocity + _acceleration, maxSpeed);
		this.transform.position += _velocity;

		float angle = ((Mathf.Atan2(_velocity.y, _velocity.x) * 180) / Mathf.PI) % 360;
		this.transform.eulerAngles = new Vector3(0, 0, angle);
	}
}
