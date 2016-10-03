using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneNav : MonoBehaviour {

	public void GoToConeCheck()
	{
		SceneManager.LoadScene("main");
	}

	public void GoToCollisionPrediction()
	{
		SceneManager.LoadScene("collisionPrediction");
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
