using UnityEngine;
using System.Collections;

public class Blackbird : MonoBehaviour
{
	public SlotAssignment slotAssignment;
	public Transform anchor;

	private Rigidbody2D rb;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	public void OnCollisionEnter2D(Collision2D other)
	{
		if(other.collider.tag == "bird")
		{
			TwoLevelFormation.characters.RemoveAt(0);
			other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			slotAssignment.SetSlots( SlotAssignment.TotalSlots.Count - 1, anchor);
			TwoLevelFormation.Character.ReAssignSlots();
		}
	}

	void Update()
	{
		Vector2 movement = new Vector2(Input.GetAxis("Horizontal") * 5f, Input.GetAxis("Vertical") * 5f);
		rb.velocity = movement;

	}
}
