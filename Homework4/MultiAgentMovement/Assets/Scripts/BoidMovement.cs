using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoidMovement : MonoBehaviour {

	public float maxSpeed = 1;
	public float maxAcceleration = 1;
	[Header("Pathfinding")]
	public PathDrawer path;
	public float nodeArriveRadius = 1;
	[Header("Formation")]
	public GameObject[] followers;

	private Vector3 _velocity = new Vector3(1, 0);
	public Vector3 Velocity { get { return _velocity; } }
	private Vector3 _acceleration = Vector3.zero;
	public Vector3 Acceleration { get { return _acceleration; } }
	private Vector3[] _formationSlots;
	public Vector3[] FormationSlots { get { return _formationSlots; } }

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

	Vector3 PathFollowing()
	{
		Vector3 targetPos = Vector3.zero;
		if (path.Nodes.Length > 0 && _currentNode >= 0)
		{
			_pathTarget = path.Nodes[_currentNode];
			targetPos = _pathTarget.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(this.transform.position, _velocity, out hitInfo, 2))
			{
				Debug.Log(hitInfo.collider.name);
				targetPos = _pathTarget.position + hitInfo.normal.normalized * nodeArriveRadius;
			}
			if (calcDistance(this.transform.position, targetPos) <= nodeArriveRadius)
			{
				_currentNode++;
				if (_currentNode >= path.Nodes.Length)
				{
					_currentNode = 0;
					//_currentNode = _path.Length-1;
				}
			}
		}
		if (_pathTarget != null) { return Seek(targetPos, nodeArriveRadius); }
		else { return Vector3.zero; }
	}

	void Scalable()
	{
		int numFollowers = followers.Length;
		float formRadius = .5f + (numFollowers / 8) * 1;
		float angle = 0f;
		float x = 0, y = 0;
		for (int i = 0; i < numFollowers; i++ )
		{
			x = Mathf.Sin(Mathf.Deg2Rad * angle) * formRadius;
			y = Mathf.Cos(Mathf.Deg2Rad * angle) * formRadius;
			_formationSlots[i] = new Vector3(this.transform.position.x + x, this.transform.position.y + y, 0);
			angle += 360f / numFollowers;
		}
		for (int f = 0; f < numFollowers; f++) 
		{
			FormationMovement followerScript = followers[f].GetComponent<FormationMovement>();
			followerScript.DoSeek(_formationSlots[f], followerScript.arriveRadius);
		}
	}

	// Use this for initialization
	void Start () {
		_formationSlots = new Vector3[followers.Length];
	}
	
	// Update is called once per frame
	void Update () {

		Scalable();

		_acceleration = PathFollowing();
		_acceleration = Clip(_acceleration, maxAcceleration);
		_velocity = Clip(_velocity + _acceleration, maxSpeed);
		Debug.DrawLine(this.gameObject.transform.position, this.transform.position + _velocity, Color.red);
		this.transform.position += _velocity;

		float angle = ((Mathf.Atan2(_velocity.y, _velocity.x) * 180) / Mathf.PI) % 360;
		this.transform.eulerAngles = new Vector3(0, 0, angle);
	}
}
