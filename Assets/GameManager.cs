using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mainMaze;
	public GameObject teste;

	public int length;
	public int width;

	private int[,,] maze;

	// Use this for initialization
	void Start () {
		mainMaze = new Maze (length, width);

		mainMaze.Setup ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
}
