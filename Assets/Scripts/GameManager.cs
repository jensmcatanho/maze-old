using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mainMaze;
	public GameObject player;
	public GameObject finishTrigger;
	public GameObject chest;
		
	public int mazeLength;
	public int mazeWidth;

	Vector3 spawnPoint;
	Vector3 finishPoint;

	// Use this for initialization
	void Start () {
		mainMaze = ScriptableObject.CreateInstance("Maze") as Maze;
		mainMaze.Init (mazeLength, mazeWidth);

		spawnPoint = new Vector3 (1.0f, 1.0f, 1.0f);
		finishPoint = new Vector3 (2 * mazeLength - 1, 1.0f, 2 * mazeWidth + 2);

		Instantiate (player, spawnPoint, new Quaternion());
		Instantiate (finishTrigger, finishPoint, new Quaternion ());

		mainMaze.Setup ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
}
