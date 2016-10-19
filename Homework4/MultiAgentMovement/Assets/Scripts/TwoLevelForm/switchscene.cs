using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class switchscene : MonoBehaviour {

	public void click(string scene)
	{
		SceneManager.LoadScene(scene);
	}
}
