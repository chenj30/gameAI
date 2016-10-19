using UnityEngine;
using System.Collections;

public class PathFollow_Script : Movement {

	// PUBLIC VARIABLES
	#region Path Following Variables
	public GameObject path;
	public float distanceToNode;
	#endregion

	// PRIVATE VARIABLES

	private int index = 0; 
	private Transform[] pathNodes;

	// FUNCTIONS

	#region Unity Functions
	// Use this for initialization
	void Start () {
		if(path != null) {
			pathNodes = path.GetComponentsInChildren<Transform> ();
		}

		StartCoroutine(move());
	}
	
	// Update is called once per frame
	void Update () {
		if(index == pathNodes.Length-1) {
			StopCoroutine(move());
		}
	}
	#endregion

	private IEnumerator move() {

		yield return new WaitForSeconds (1);

		while(true) {

			Vector3 target = Vector3.zero;

			target = pathFollow();

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
