using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TwoLevelFormation : MonoBehaviour {


	public Transform anchor;
	private List<GameObject> objs;
	public static List<Character> characters;
	private Vector2 offsetWithRotation;

	void Start()
	{
		objs = new List<GameObject>();
		characters = new List<Character>();
		objs.AddRange(GameObject.FindGameObjectsWithTag("bird"));
		for (int i = 0; i < objs.Count; i++)
		{
			characters.Add(new Character(objs[i], i, SlotAssignment.TotalSlots[i]));
		}
		foreach( Character character in characters)
		{
			character.Bird.GetComponent<Transform>().position = new Vector2(anchor.position.x, anchor.position.y) + character.Offset;
			character.Bird.GetComponent<Transform>().rotation = new Quaternion(anchor.rotation.x, anchor.rotation.y, anchor.rotation.z, anchor.rotation.w);
		}
	}

	void FixedUpdate()
	{		
		foreach (Character character in characters)
		{
			character.RayCastCollision(character);
			if (character.State_ == Character.State.follow)
			{
				character.Target = character.Bird.GetComponent<Rigidbody2D>().velocity.normalized;
                Rigidbody2D rb = character.Bird.GetComponent<Rigidbody2D>();
				character.TargetPos = new Vector2(anchor.position.x, anchor.position.y) + character.Offset;
				Vector2 targetDis = character.TargetPos - new Vector2(character.Bird.transform.position.x, character.Bird.transform.position.y);
				targetDis = targetDis.normalized * Mathf.Min(targetDis.magnitude / .5f, 10f);
				Vector2 dv = targetDis - rb.velocity;
				dv = dv.normalized * Mathf.Min(dv.magnitude * 10f, 20f);
				rb.AddForce(dv * rb.mass, ForceMode2D.Force);
				character.Bird.GetComponent<Transform>().rotation = new Quaternion(anchor.rotation.x, anchor.rotation.y, anchor.rotation.z, anchor.rotation.w);
			}
			else
			{
				Rigidbody2D rb = character.Bird.GetComponent<Rigidbody2D>();
				Vector2 dir = (anchor.position - character.Bird.transform.position).normalized;
				RaycastHit2D hit = Physics2D.Raycast(character.Bird.transform.position, character.Bird.GetComponent<Rigidbody2D>().velocity.normalized, .5f);
				Debug.DrawRay(character.Bird.transform.position, character.Bird.GetComponent<Rigidbody2D>().velocity.normalized, Color.green, 20, true);
				if(hit.collider != null && hit.collider.tag == "obstacle")
				{
					if (hit.transform != character.Bird.transform)
					{
						
						dir += hit.normal * 50;
					}
				}

				Vector2 leftCast, rightCast;
				leftCast = character.Bird.transform.position;
				rightCast = character.Bird.transform.position;
				leftCast.x -= .2f;
				rightCast.x += .2f;
				RaycastHit2D hitLeft = Physics2D.Raycast(leftCast, character.Bird.GetComponent<Rigidbody2D>().velocity.normalized, 1.5f);
				if (hitLeft.collider != null && hitLeft.collider.tag == "obstacle")
				{
					if (hitLeft.transform != character.Bird.transform)
					{
						dir += hitLeft.normal * 50;
					}

				}
				RaycastHit2D hitRight = Physics2D.Raycast(rightCast, character.Bird.GetComponent<Rigidbody2D>().velocity.normalized, 1.5f);
				if (hitRight.collider != null && hitRight.collider.tag == "obstacle")
				{
					if (hitRight.transform != character.Bird.transform)
					{
						dir += hitRight.normal * 50;
					}
				}


				character.TargetPos = dir;
				Vector2 targetDis = character.TargetPos - new Vector2(character.Bird.transform.position.x, character.Bird.transform.position.y);
				targetDis = targetDis.normalized * Mathf.Min(targetDis.magnitude / .5f, 10f);
				Vector2 dv = targetDis - rb.velocity;
				dv = dv.normalized * Mathf.Min(dv.magnitude * 10f, 20f);
				rb.AddForce(dv * rb.mass, ForceMode2D.Force);
			}
		}
	}

	public class Character
	{
		public enum State
		{
			follow,
			evade,
			prepEvade
		}
		private GameObject bird;
		private int _id;
		private Vector2 _offset;
		private Vector2 _targetPosition;
		private Vector2 target;
		private State _state;
		private Vector2 _anchorSavestate;

		public GameObject Bird
		{
			get { return bird; }
			set { bird = value; }
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public Vector2 Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}

		public State State_
		{
			get { return _state; }
			set { _state = value; }
		}

		public Vector2 Target
		{
			get { return target; }
			set { target = value; }
		}

		public Vector2 TargetPos
		{
			get { return _targetPosition; }
			set { _targetPosition = value; }
		}

		public Character(GameObject b, int id, Vector2 s)
		{
			bird = b;
			_id = id;
			_offset = s;
			_state = State.follow;
		}

		public static void ReAssignSlots()
		{
			for (int i = 0; i < characters.Count; i++ )
			{
				characters[i].Offset = SlotAssignment.TotalSlots[i];
			}
		}

		public void RayCastCollision(Character character)
		{
			
			RaycastHit2D hit = Physics2D.Raycast(character.bird.transform.position, target, .5f);
			if (hit.collider != null && hit.collider.tag == "obstacle")
			{
				_state = State.evade;
			}
			else if (hit.collider == null)
			{
				_state = State.follow;
			}
		}
	}
}
