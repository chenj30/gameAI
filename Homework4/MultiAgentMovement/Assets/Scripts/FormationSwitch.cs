using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FormationSwitch : MonoBehaviour {

	[SerializeField]
	private GameObject birdLeader;

	private Text _txtObj;

	// Use this for initialization
	void Start () {
		_txtObj = this.gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoToScalable()
	{
		_txtObj.text = "Scalable Formation";
		// change formation to scalable
		SceneManager.LoadScene("Johnny_workbench");
	}

	public void GoToEmerged()
	{
		_txtObj.text = "Emerged Formation";
		// change formation to emgered 
		SceneManager.LoadScene("Andy_workbench");
	}

	public void GoToTwoLevel()
	{
		_txtObj.text = "Two-Level Formation";
		// change formation to two-level
	}

}
