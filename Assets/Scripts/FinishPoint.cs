using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinishPoint : MonoBehaviour {

	void Start () {
		
	}

	void Update () {
	
	}

	void OnTriggerEnter() {
		SceneManager.LoadScene("endscene");

	}
}
