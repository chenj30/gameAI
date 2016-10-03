using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIMovement : MonoBehaviour {

	// Enum to define which type of movement
	public enum MovementType {
		Stop,
		Pursue,
		Evade,
		Wander,
		PathFollowing,
		Mouse
	}

	// PUBLIC VARIABLES //
	public MovementType moveType; // Defines AI Movement type
	public Transform target = null; // Defines target of movement

	public float maxSpeed = 0.2f; // Defines maximum speed
	public float maxAcceleration = 0.2f; // Defines maximum acceleration 
	public float arriveRadius = 2; // Defines radius for slow-down radius

	[Header("For Evade and Wander")]
	public float evadeRadius = 2; // Defines radius of when to evade the target
	public float circleDist = 0.2f; 
	public float wanderRadius = 0.2f;
	public float wanderAngle = 0;
	public float angleChange = 1;

	[Header("For Path Following")]
	public Transform[] path;

	// PRIVATE VARIABLES //
	private Vector3 _velocity = new Vector3(-1, -2);
	public Vector3 Velocity { get { return _velocity; } }
	private Vector3 _acceleration = Vector3.zero;
	public Vector3 Acceleration { get { return _acceleration; } }

	private float targetArriveRadius = 0;
	private int currentNode = 0;
	
	// FUNCTIONS //
	Vector3 Clip(Vector3 vector, float max) {
		float i;
		i = max / vector.magnitude;
		if (i > 1.0f) { i = 1.0f; }
		return vector *= i;
	}

	Vector3 Seek(Vector3 targetLoc) {
		Vector3 desiredVelocity = targetLoc - this.transform.position;
		if (desiredVelocity.magnitude < targetArriveRadius)
		{
			desiredVelocity = desiredVelocity.normalized * maxSpeed * (desiredVelocity.magnitude / targetArriveRadius);
		}
		else { desiredVelocity = desiredVelocity.normalized * maxSpeed; }
		Vector3 force = desiredVelocity - _velocity;
		return force;
	}
	
	Vector3 Flee(Vector3 targetLoc) {
		Vector3 desiredVelocity = this.transform.position - targetLoc;
		desiredVelocity = desiredVelocity.normalized * maxSpeed;
		Vector3 force = desiredVelocity - _velocity;
		return force;
	}

	Vector3 Pursue() {
		Vector3 distance = target.position - this.transform.position;
		int updates = (int)(distance.magnitude / maxSpeed);
		Vector3 futurePos = target.position + target.GetComponent<AIMovement>().Velocity * updates;
		return Seek(futurePos);
	}

	Vector3 Evade() {
		Vector3 distance = target.position - this.transform.position;
		int updates = (int)(distance.magnitude / maxSpeed);
		Vector3 futurePos = target.position + target.GetComponent<AIMovement>().Velocity * updates;
		return Flee(futurePos);
	}

	Vector3 Wander() {
		Vector3 circleCenter = _velocity.normalized * (circleDist);
		Vector3 displacement = new Vector3(0, -1) * (wanderRadius);
		displacement.x = Mathf.Cos(wanderAngle) * displacement.magnitude;
		displacement.y = Mathf.Sin(wanderAngle) * displacement.magnitude;
		wanderAngle += (Random.value * angleChange) - (angleChange * .5f) ;
		Vector3 force = circleCenter + displacement;
		return force;
	}

	float calcDistance(Vector3 a, Vector3 b)
	{
		return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
	}

	Vector3 PathFollowing() {
		if (path.Length > 0)
		{
			target = path[currentNode];
			if (calcDistance(this.transform.position, target.position) <= target.GetComponent<AIMovement>().arriveRadius)
			{
				currentNode++;
				if (currentNode >= path.Length)
				{
					currentNode = 0;
				}
			}
		}
		if (target != null) { return Seek(target.position); }
		else { return Vector3.zero; }
	}

	// Use this for initialization
	void Start () {
		if ((moveType == MovementType.Pursue || moveType == MovementType.Evade) && target == null)
		{
			moveType = MovementType.Wander;
		}
		if (target != null)
		{
			AIMovement script = target.GetComponent<AIMovement>();
			if (script != null) { targetArriveRadius = script.arriveRadius; }
		}
	}

	
	// Update is called once per frame
	void Update () {

		switch (moveType)
		{
			case MovementType.Stop:
				_velocity = Vector3.zero;
				_acceleration = Vector3.zero;
				break;
			case MovementType.Pursue:
				_acceleration = Pursue();
				break;
			case MovementType.Evade:
				if (calcDistance(this.transform.position, target.position) <= evadeRadius)
				{
					_acceleration = Evade();
				}
				else
				{
					_acceleration = Wander();
				}
				break;
			case MovementType.Wander:
				_acceleration = Wander();
				break;
			case MovementType.PathFollowing:
				_acceleration = PathFollowing();
				break;
			case MovementType.Mouse:
				_acceleration = Seek(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10));
				break;
			default:
				Debug.Log("Unknown MoveType");
				break;
		}

		_acceleration = Clip(_acceleration, maxAcceleration);
		_velocity = Clip(_velocity + _acceleration, maxSpeed);
		this.transform.position += _velocity;

		float angle = (Mathf.Atan2(_velocity.y, _velocity.x) * 180) / Mathf.PI;
		this.transform.eulerAngles = new Vector3(0, 0, angle);

		Vector2 cameraPos = Camera.main.transform.position;
		float upperBound = cameraPos.y + Camera.main.orthographicSize;
		float lowerBound = cameraPos.y - Camera.main.orthographicSize;
		float leftBound = cameraPos.x - Camera.main.orthographicSize;
		float rightBound = cameraPos.x + Camera.main.orthographicSize;
		if (this.transform.position.x > rightBound)
		{
			this.transform.position = new Vector3(leftBound, this.transform.position.y, this.transform.position.z);
		}
		else if (this.transform.position.x < leftBound)
		{
			this.transform.position = new Vector3(rightBound, this.transform.position.y, this.transform.position.z);
		}
		if (this.transform.position.y > upperBound)
		{
			this.transform.position = new Vector3(this.transform.position.x, lowerBound, this.transform.position.z);
		}
		else if (this.transform.position.y < lowerBound)
		{
			this.transform.position = new Vector3(this.transform.position.x, upperBound, this.transform.position.z);
		}
	}
}
