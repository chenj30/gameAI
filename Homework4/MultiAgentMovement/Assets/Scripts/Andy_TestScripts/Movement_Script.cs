using UnityEngine;
using System.Collections;

public class Movement_Script : MonoBehaviour {

	// PUBLIC VARIABLES

	public bool followPath = false;

	#region Movement Variables
	public Vector3 linearAcceleration;
	public Vector3 linearVelocity;
	public float maxAcceleration;
	public float maxVelocity;
	public float slowRadius;
	#endregion

	#region Separation Variables
	public float separationWeight;
	public float groupRadius;
	#endregion

	#region Raycast Variables
	public float avoidDistance;
	#endregion

	#region Emergence Variables
	public float emergenceWeight;
	public float distance; // distance from leader of gameobject
	#endregion

	#region Path Following Variables
	public float pathFollowWeight;
	public GameObject path;
	public float distanceToNode;
	#endregion


	// PRIVATE VARIABLES
	private string groupTag;
	private int[] children;
	private int numChildren = 2;
	private GameObject leader;
	private bool haveLeader = false;

	private int index = 0; 
	private Transform[] pathNodes;

	float tempEmergenceWeight;

	// FUNCTIONS

	#region Unity Funcions
	// Use this for initialization
	void Start () {

		if(path != null) {
			pathNodes = path.GetComponentsInChildren<Transform> ();
		}

		// give gameobject 3 children and set all to -1 (-1 for slot unfilled, 1 for slot filled)
		children = new int[numChildren];
		for (int i = 0; i < numChildren; i++) {
			children [i] = -1;
		}

		groupTag = "flock";
		tempEmergenceWeight = emergenceWeight;

		StartCoroutine (move ());
	}
	
	// Update is called once per frame
	void Update () {
		if(followPath && index >= pathNodes.Length) {
			StopCoroutine (move ());
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if(!followPath && col.gameObject.tag == "BlackBird") {
			Destroy (this.gameObject);
		}
	}
	#endregion

	private IEnumerator move() {

		yield return new WaitForSeconds (1);

		while(true) {
			
			Vector3 target = Vector3.zero;

			if(followPath) {
				// have overall group leader follow the path
				target = rayCastDir() +  pathFollow () * pathFollowWeight;
			}
			else { // movement of group members
				
				// find a leader to follow
				if(leader == null) {
					findLeader ();
				}
				target = rayCastDir() + spotBehindLeader() * emergenceWeight + separate() * separationWeight;
			}

			// get acceleration and set it to max acc if above max accleration
			linearAcceleration = seek (target);
			if(linearAcceleration.magnitude > maxAcceleration) {
				linearAcceleration = linearAcceleration.normalized * maxAcceleration;
			}

			// calculate velocity and clamp to max
			linearVelocity += linearAcceleration;
			if(linearVelocity.magnitude > maxVelocity) {
				linearVelocity = linearVelocity.normalized * maxVelocity;
			}

			// turn in direction
			Vector3 direction = (target - transform.position).normalized;
			float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, angle);

			// move
			transform.position += linearVelocity;

			yield return new WaitForEndOfFrame ();
		}
	}

	#region Seek function
	// calculate seeking
	private Vector3 seek(Vector3 target) {

		// turn in direction
		Vector3 direction = (target - transform.position).normalized;

		// calculate the acceleration
		Vector3 acc = Vector3.zero;

		Vector3 velocity = target - transform.position;
		if(velocity.magnitude < slowRadius) {
			velocity = direction * maxVelocity * (velocity.magnitude / slowRadius);
		}
		else {
			velocity = direction * maxVelocity;
		}
		acc = velocity - linearVelocity;

		return acc;
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

			// if the ray has hit something ignore emergence and focus on avoiding the wall in front
			if(!followPath) {
				emergenceWeight = 0;
			}
		}
		else if(hit.collider == null) { // else if not in danger of hitting anything then go back to following leader
			emergenceWeight = tempEmergenceWeight;
		}

		return target;
	}
	#endregion

	#region Emergent functions
	// set an empty child's slot as filled; if all slots filled return null
	private GameObject setChild() {
		for (int i = 0; i < numChildren; i++) {
			if (children [i] == -1) {
				children [i] = 1;
				return this.gameObject;
			}
		}

		return null;
	}

	// find a leader in the formation to follow
	private void findLeader() {
		GameObject[] groupMembers = GameObject.FindGameObjectsWithTag (groupTag);

		// pick a member in the group to follow
		while(!haveLeader) {
			// find a random member in the group
			int index = Random.Range (0, groupMembers.Length-1);

			// exclude this gameobject
			if(groupMembers[index] != this.gameObject) {
				// find if the member has 3 followers yet or not
				GameObject temp = groupMembers [index].GetComponent<Movement_Script> ().setChild ();

				if(temp != null) {
					//Debug.Log ("follow: " + temp.name);
					leader = temp;
					haveLeader = true;
				}
			}
		}
	}

	// find a spot behind the leader
	private Vector3 spotBehindLeader() {

		Vector3 spot = Vector3.zero;

		// if the one the gameobject has been following has been killed find a new
		if(leader == null) {
			findLeader ();
		}
			
		spot = leader.transform.position - (leader.transform.forward * distance);
		spot.z = 0;

		// get a random x value in range to differ from the leader's
		float xValue = leader.transform.position.x;
		spot.x = Random.Range (xValue - 1, xValue + 1);

		return spot;
	}
	#endregion

	#region Path Following function
	// find the next node to seek
	private Vector3 pathFollow() {
		Vector3 nextNode = pathNodes [index].position;
		float distance = (transform.position - nextNode).sqrMagnitude;

		if(distance <= distanceToNode * distanceToNode) {
			index++;
		}

		if(index >= pathNodes.Length) {
			index = pathNodes.Length - 1;
		}

		return nextNode;
	}
	#endregion


}
