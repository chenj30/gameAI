using UnityEngine;
using System.Collections.Generic;

public class SlotAssignment : MonoBehaviour {

	public Transform anchor;

	public static List<Vector2> TotalSlots;

	private int leftBranch;
	private int rightBranch;

	// Update is called once per frame
	void Awake()
	{
		TotalSlots = new List<Vector2>();
		Debug.Log(GameObject.FindGameObjectsWithTag("bird").Length);
		SetSlots(GameObject.FindGameObjectsWithTag("bird").Length, anchor);
	}

	public void SetSlots(int slotsToSet, Transform anchor)
	{
		TotalSlots.Clear();
		//Create a circular formation around the point
		float radius = 1.5f;
		int angle = 360 / (slotsToSet+1);
		for(int i = 0; i < slotsToSet; i++)
		{
			Vector2 slot = new Vector2(radius * Mathf.Cos(angle * i), radius * Mathf.Sin(angle * i));
			TotalSlots.Add(slot);
		}
	}
}
