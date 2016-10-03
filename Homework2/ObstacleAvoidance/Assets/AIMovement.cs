using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIMovement : MonoBehaviour
{

	// Enum to define which type of movement
	public enum MovementType
	{
		Stop,
		Pursue,
		Evade,
		Wander,
		PathFollowing,
		Mouse,
		ConeCheck,
		CollisionPredict
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
	public GameObject pathObject;
	public float nodeArriveRadius;

	[Header("For Cone Check")]
	public GameObject groupToAvoid;
	public float coneAngle = 30;

	// PRIVATE VARIABLES //
	private Vector3 _velocity = new Vector3(1, 0);
	public Vector3 Velocity { get { return _velocity; } }
	private Vector3 _acceleration = Vector3.zero;
	public Vector3 Acceleration { get { return _acceleration; } }

	private float _targetArriveRadius = 0;
	private int _currentNode = 0;
	private Transform _pathTarget;

	private Transform[] _path;

	// FUNCTIONS //
	Vector3 Clip(Vector3 vector, float max)
	{
		float i;
		i = max / vector.magnitude;
		if (i > 1.0f) { i = 1.0f; }
		return vector *= i;
	}

	Vector3 Seek(Vector3 targetLoc)
	{
		Vector3 desiredVelocity = targetLoc - this.transform.position;
		if (desiredVelocity.magnitude < _targetArriveRadius)
		{
			desiredVelocity = desiredVelocity.normalized * maxSpeed * (desiredVelocity.magnitude / _targetArriveRadius);
		}
		else { desiredVelocity = desiredVelocity.normalized * maxSpeed; }
		Vector3 force = desiredVelocity - _velocity;
		return force;
	}

	Vector3 Flee(Vector3 targetLoc)
	{
		Vector3 desiredVelocity = this.transform.position - targetLoc;
		desiredVelocity = desiredVelocity.normalized * maxSpeed;
		Vector3 force = desiredVelocity - _velocity;
		return force;
	}

	Vector3 Pursue()
	{
		Vector3 distance = target.position - this.transform.position;
		int updates = (int)(distance.magnitude / maxSpeed);
		Vector3 futurePos = target.position + target.GetComponent<AIMovement>().Velocity * updates;
		return Seek(futurePos);
	}

	Vector3 Evade()
	{
		Vector3 distance = target.position - this.transform.position;
		int updates = (int)(distance.magnitude / maxSpeed);
		Vector3 futurePos = target.position + target.GetComponent<AIMovement>().Velocity * updates;
		return Flee(futurePos);
	}

	Vector3 Wander()
	{
		Vector3 circleCenter = _velocity.normalized * (circleDist);
		Vector3 displacement = new Vector3(0, -1) * (wanderRadius);
		displacement.x = Mathf.Cos(wanderAngle) * displacement.magnitude;
		displacement.y = Mathf.Sin(wanderAngle) * displacement.magnitude;
		wanderAngle += (Random.value * angleChange) - (angleChange * .5f);
		Vector3 force = circleCenter + displacement;
		return force;
	}

	float calcDistance(Vector3 a, Vector3 b)
	{
		return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
	}

	Vector3 PathFollowing()
	{
		if (_path.Length > 0)
		{
			_pathTarget = _path[_currentNode];
			if (calcDistance(this.transform.position, _pathTarget.position) <= nodeArriveRadius)
			{
				_currentNode++;
				if (_currentNode >= _path.Length)
				{
					//_currentNode = 0;
					//_currentNode = _path.Length-1;
					moveType = MovementType.Stop;
				}
			}
		}
		if (_pathTarget != null) { return Seek(_pathTarget.position); }
		else { return Vector3.zero; }
	}

	float dotProduct(Vector3 a, Vector3 b)
	{
		return (a.x * b.x + a.y * b.y + a.z * b.z);
	}

	Vector3 ConeCheck()
	{
		// Get closest target
		GameObject closestTarget = null;
		float closestDist = 100000f;
		Transform[] otherAgents = groupToAvoid.GetComponentsInChildren<Transform>();
		foreach (Transform agent in otherAgents)
		{
			if (calcDistance(agent.transform.position, this.transform.position) < closestDist)
			{
				closestTarget = agent.gameObject;
				closestDist = calcDistance(agent.transform.position, this.transform.position);
			}
		}
		//Debug.DrawLine(this.transform.position, closestTarget.transform.position, Color.red);
		// Calculate direction of target to character
		Vector3 direction = this.transform.position + (closestTarget.transform.position - this.transform.position);
		Debug.DrawLine(this.transform.position, direction.normalized, Color.blue);
		// Get the angle and calculate orientation vector
		float angle = ((this.transform.rotation.eulerAngles.z * Mathf.PI) / 180);
		Vector3 orientation = this.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
		Debug.DrawLine(this.transform.position, orientation.normalized, Color.white);
		// Check if target is within cone range
		float coneThreshold = Mathf.Cos((coneAngle * Mathf.PI) / 180) / 2;
		if (dotProduct(orientation, direction) > coneThreshold && calcDistance(this.transform.position, closestTarget.transform.position) < 3) 
		{
			// if so, return evade steering
			return Flee(closestTarget.transform.position)*.5f + PathFollowing();
		}
		else
		{
			// else return no steering
			return PathFollowing();
		}
	}

	Vector3 CollisionPrediction()
	{
		// Get closest target
		GameObject closestTarget = null;
		float closestDist = 100000f;
		Transform[] otherAgents = groupToAvoid.GetComponentsInChildren<Transform>();
		foreach (Transform agent in otherAgents)
		{
			if (calcDistance(agent.transform.position, this.transform.position) < closestDist)
			{
				closestTarget = agent.gameObject;
				closestDist = calcDistance(agent.transform.position, this.transform.position);
			}
		}
		// calculate delta in positions
		Vector3 dp = closestTarget.transform.position - this.transform.position;
		// calculate delta in velocities
		Vector3 dv = Vector3.zero;
		AIMovement ct_script = closestTarget.GetComponent<AIMovement>();
		if (ct_script != null) { dv = ct_script.Velocity - this.Velocity; }
		// calculate time of closest point
		float tClosest = (dotProduct(dp, dv)) / (dv.magnitude * dv.magnitude);
		// calculate the position at time of closest point
		Vector3 pT = closestTarget.transform.position;
		if (ct_script != null) { pT = closestTarget.transform.position + ct_script.Velocity * tClosest; }
		Vector3 pC = this.transform.position + this.Velocity * tClosest;
		// add steering depending on the positions of the target and character at the time of their closest point
		if (calcDistance(pT,pC) < 2)
		{
			return Flee(closestTarget.transform.position) * .5f + PathFollowing();
		}
		else
		{
			return PathFollowing();
		}
	}

	// Use this for initialization
	void Start()
	{
		if ((moveType == MovementType.Pursue || moveType == MovementType.Evade) && target == null)
		{
			moveType = MovementType.Wander;
		}
		if (target != null)
		{
			AIMovement script = target.GetComponent<AIMovement>();
			if (script != null) { _targetArriveRadius = script.arriveRadius; }
		}
		if ((moveType == MovementType.PathFollowing || moveType == MovementType.ConeCheck || moveType == MovementType.CollisionPredict) && pathObject != null)
		{
			PathDrawer pathScript = pathObject.GetComponent<PathDrawer>();
			_path = new Transform[pathScript.nodes.Length];
			for (int i = 0; i < pathScript.nodes.Length; i++)
			{
				_path[i] = pathScript.nodes[i];
			}
		}
	}


	// Update is called once per frame
	void Update()
	{

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
				_acceleration = Seek(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10));
				break;
			case MovementType.ConeCheck:
				_acceleration = ConeCheck();
				break;
			case MovementType.CollisionPredict:
				_acceleration = CollisionPrediction();
				break;
			default:
				Debug.Log("Unknown MoveType");
				break;
		}

		_acceleration = Clip(_acceleration, maxAcceleration);
		_velocity = Clip(_velocity + _acceleration, maxSpeed);
		this.transform.position += _velocity;

		float angle = ((Mathf.Atan2(_velocity.y, _velocity.x) * 180) / Mathf.PI) % 360;
		this.transform.eulerAngles = new Vector3(0, 0, angle);

		/// Camera bounds
		/*
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
		*/
	}
}
