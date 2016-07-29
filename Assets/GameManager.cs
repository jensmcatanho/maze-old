using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mainMaze;
	public GameObject teste;

	// Use this for initialization
	void Start () {
		teste = GameObject.CreatePrimitive(PrimitiveType.Plane);
		teste.transform.localScale = new Vector3 (0.2f, 1, 0.4f);
		teste.transform.Rotate(new Vector3 (90.0f, 0.0f, 0.0f));
		teste.transform.position = new Vector3 (1, 2, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
