using UnityEngine;
using System.Collections.Generic;

public class PathFollow : MonoBehaviour {

	//================================================
	//| Sets the path following for the anchor point |
	//================================================
	[SerializeField]
	private float _movementSpeed;
	[SerializeField]
	private float _acceleration;
	[SerializeField]
	private float _maxAcceleration;


	private float _slowRadius;
	private static List<Vector2> _path;
	private static int _index;
	private Rigidbody2D _rb;

	public static List<Vector2> Path
	{
		get { return _path; }
	}
	public static int Index
	{
		get { return _index; }
	}
	void Awake()
	{
		_slowRadius = .5f;
		_index = 0;
		_rb = GetComponent<Rigidbody2D>();
		_path = new List<Vector2>();
		_path.Add(new Vector2(-5.71f, 3.29f));
		_path.Add(new Vector2(-6.34f, 2.52f));
		_path.Add(new Vector2(-7.93f, -1.54f));
		_path.Add(new Vector2(-3.84f, -0.46f));
		_path.Add(new Vector2(-1.59f, -3.54f));
		_path.Add(new Vector2(-0.36f, 0.63f));
		_path.Add(new Vector2(5.85f, 1.25f));
		_path.Add(new Vector2(7.68f, 4.11f));
	}

	void Update()
	{
		if (_index < _path.Count)
			_index = PathFollowing(_path, _index, _rb, _acceleration, _maxAcceleration, _slowRadius, _movementSpeed);
		if (_index >= _path.Count)
		{
			_rb.velocity = new Vector2(0,0);
		}
	}

	public int PathFollowing(List<Vector2> path, int index, Rigidbody2D rb, float acceleration, float maxAccel, float slowRadius, float maxVelocity)
	{
		Vector2 targetpos = path[index];
		Vector2 targetDis = targetpos - rb.position;

		float angle = Mathf.Atan2(targetDis.x, targetDis.y) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, q, Time.deltaTime * 2);

		targetDis = targetDis.normalized * Mathf.Min(targetDis.magnitude / slowRadius, maxVelocity);
		float dis = Mathf.Abs(Vector2.Distance(rb.position, targetpos));

		Vector2 dv = targetDis - rb.velocity;
		dv = dv.normalized * Mathf.Min(dv.magnitude * acceleration, maxAccel);
		rb.AddForce(dv * rb.mass, ForceMode2D.Force);

		if (dis <= .05f)
		{
			index++;
		}
		return index;
	}
}
