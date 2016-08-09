using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mainMaze;
	//public AppState app;
	public GameObject player;
	public GameObject finishTrigger;
	public GameObject chest;

	public int mazeLength;
	public int mazeWidth;

	void Start () {		
		mainMaze.Init (mazeLength, mazeWidth);

		mainMaze.spawnPoint = new Vector3 (1.0f, 1.0f, 1.0f);
		mainMaze.finishPoint = new Vector3 (2 * mazeLength - 1, 1.0f, 2 * mazeWidth + 2);

		Instantiate (player, mainMaze.spawnPoint, new Quaternion());
		Instantiate (finishTrigger, mainMaze.finishPoint, new Quaternion ());

		mainMaze.Setup ();
	}
}
