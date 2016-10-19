using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	// PUBLIC VARIABLES
	#region Movement Variables
	public Vector3 linearAcceleration;
	public Vector3 linearVelocity;
	public float maxAcceleration;
	public float maxVelocity;
	public float slowRadius;
	#endregion

	public GameObject[] children;
	public string groupTag;
	public int numChildren = 2;

	// FUNCTIONS
	void Start() {

	}

	#region Seek function
	// calculate seeking
	public Vector3 seek(Vector3 target) {

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
}
