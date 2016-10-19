using UnityEngine;
using System.Collections;

public class Emergence_Script : Movement {

	// PUBLIC VARIABLES
	#region Emergence Variables
	public GameObject mainLeader;
	public bool followMainLeader = true;
	public float xOffset;  // x offset from the leader's position
	public float yOffset;  // y offset from the leader's position
	#endregion

	#region Separation Variables
	public float separationWeight;
	public float groupRadius;
	#endregion

	#region Raycast Variables
	public float avoidDistance;
	#endregion

	// PRIVATE FUNCTIONS
	private GameObject personalLeader;
	private bool neg = false;
	private bool willHit = false;

	// FUNCTIONS

	#region Unity functions
	// Use this for initialization
	void Start () {
		personalLeader = null;

		children = new GameObject[numChildren];
		for(int i = 0; i < numChildren; i++) {
			children[i] = null;
		}

		StartCoroutine(move());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.tag == "Blackbird") {
			this.gameObject.tag = "none";

			if(children[0] != null && followMainLeader){
				children[0].GetComponent<Emergence_Script>().findLeader(true);

				if(children[1] != null) {
					children[1].GetComponent<Emergence_Script>().findLeader(false);
				}
			}
			else if(!followMainLeader) {
				if(children[0] != null) {
					children[0].GetComponent<Emergence_Script>().findLeader(false);
				}

				if(children[1] != null) {
					children[1].GetComponent<Emergence_Script>().findLeader(false);
				}
			}

			Destroy(this.gameObject);
		}
	}
	#endregion

	private IEnumerator move() {
		yield return new WaitForSeconds(1);

		findLeader(true);

		while(true) {

			Vector3 target = Vector3.zero;

			target = rayCastDir();

			if(!willHit) {
				target = findPosition() + separate() * separationWeight;
			}

			// get linear acceleration and if more than max clamp to max
			linearAcceleration = seek(target);
			if(linearAcceleration.magnitude > maxAcceleration) {
				linearAcceleration = linearAcceleration.normalized * maxAcceleration;
			}

			// get linear velocity and if more than max clamp to max
			linearVelocity += linearAcceleration;
			if(linearVelocity.magnitude > maxVelocity) {
				linearVelocity = linearVelocity.normalized * maxVelocity;
			}

			// turn in direction
			Vector3 direction = (target - transform.position).normalized;
			float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, angle);

			// move the game object
			transform.position += linearVelocity;

			yield return new WaitForEndOfFrame(); 
		}
	}

	#region Emergence functions
	// find gameobject's place behind its leader
	private Vector3 findPosition() {
		Vector3 pos = Vector3.zero;
		float leaderX = 0;

		if(followMainLeader) {
			pos = mainLeader.transform.position - (mainLeader.transform.forward * yOffset);
			leaderX = mainLeader.transform.position.x;
		}
		else {
			pos = personalLeader.transform.position - (personalLeader.transform.forward * yOffset);
			leaderX = personalLeader.transform.position.x;
		
		}

		pos.z = 0;

		// get an x position that's offset from the leader
		if(!neg) {
			pos.x = Random.Range(leaderX-xOffset, leaderX);
		}
		else {
			pos.x = Random.Range(leaderX, leaderX + xOffset);
		}

		return pos;
	}

	// set the child of a leader
	private bool setchild(GameObject child) {
		for(int i = 0; i < numChildren; i++) {
			if(children[i] == null) {
				children[i] = child;

				if(i == 0) {
					neg = true;
				}
				else {
					neg = false;
				}

				return true;
			}
		}

		return false;
	}

	private void findLeader(bool leaderFollower) {

		// if leaderFollower is true, set this gameobject to follow the leader
		if(leaderFollower) {
			followMainLeader = true;
			personalLeader = null;
			return;
		}

		GameObject[] group = GameObject.FindGameObjectsWithTag(groupTag);
		foreach(GameObject member in group) {
			if(member != this.gameObject) {
				personalLeader = member;
				member.GetComponent<Emergence_Script>().setchild(this.gameObject);
			}
		}
	}
	#endregion

	#region Separation functions
	// calculate separating from rest of group members
	private Vector3 separate() {
		Vector3 separation = Vector3.zero;
		GameObject[] groupMembers = GameObject.FindGameObjectsWithTag (groupTag);

		foreach (GameObject member in groupMembers) {

			// if group member is not this game object
			if(member != this.gameObject) {
				if(Vector3.Distance(member.transform.position, transform.position) < groupRadius) {

					// linear decay
					Vector3 distance = member.transform.position - transform.position;
					separation += ((groupRadius * distance / distance.magnitude) - distance) / groupRadius;
				}
			}
		}

		separation *= -1;

		return separation;
	}
	#endregion

	#region Raycast Functions
	// use raycast
	public Vector3 rayCastDir() {

		Vector3 target = Vector3.zero;

		// raycast to see if there is an object that is not part of the fomration that's in the way
		RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.zero, 10f);
		if(hit.collider != null && hit.collider.gameObject.tag != groupTag) {
			//Debug.Log ( this.gameObject.name + " hit");

			Vector3 hitPosition = hit.collider.transform.position;
			target = hitPosition + hitPosition.normalized * avoidDistance;
			willHit = true;
		}
		else {
			willHit = false;
		}

		return target;
	}
	#endregion
}
