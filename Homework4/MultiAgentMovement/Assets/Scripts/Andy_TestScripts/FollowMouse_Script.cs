using UnityEngine;
using System.Collections;

public class FollowMouse_Script : MonoBehaviour {

	// PUBLIC VARIABLES

	#region Unity functions
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)) {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePosition.z = 5f;

			float angle = Mathf.Atan2 (mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;

			transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			transform.position = Vector3.MoveTowards(transform.position, mousePosition, 4 * Time.deltaTime);
		}
	}
	#endregion
}
