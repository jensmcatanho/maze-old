using UnityEngine;
using System.Collections;

public class Maze {
	public int length; // x
	public int width; // z

	private GameObject floor;

	private int[,,] maze;

	public Maze (int l, int w) {
		length = l;
		width = w;
	}

	void Start () {
		Setup();
	}

	public void Setup() {
		DFSMaze ();

		CreateMaze ();
	}

	void CreateMaze() {
		CreateFloor ();

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < width; j++) {
				if (maze [i, j, 0] == 0) 
					CreateWall (new Vector3(2 * i + 1, 2.0f, 2 * j), new Vector3(90.0f, 0.0f, 0.0f));

				if (maze [i, j, 1] == 0)
					CreateWall (new Vector3(2 * i, 2.0f, 2 * j + 1), new Vector3(90.0f, 90.0f, 0.0f));

				if (maze [i, j, 2] == 0)
					CreateWall (new Vector3(2 * i + 1, 2.0f, 2 * j + 2), new Vector3(90.0f, 0.0f, 180.0f));

				if (maze [i, j, 3] == 0)
					CreateWall (new Vector3(2 * i + 2, 2.0f, 2 * j + 1), new Vector3(90.0f, -90.0f, 0.0f));
			}
		}

	}

	void CreateWall(Vector3 position, Vector3 rotation) {
		GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Plane);
		wall.transform.localScale = new Vector3 (0.2f, 1.0f, 0.4f);
		wall.transform.Rotate(rotation);
		wall.transform.position = position;

	}

	void CreateFloor() {
		GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
		floor.transform.localScale = new Vector3 (length / 5, 1, width / 5);
		floor.transform.position = new Vector3 (length, 0, width);

	}

	void DFSMaze() {
		ArrayList history = new ArrayList (), 
		check = new ArrayList ();

		int row = 0,
		col = 0;

		// The array maze is going to hold the information for each cell.
		// The first four coordinates tell if walls exist on those sides and the fifth indicates if the cell has benn visited in the search.
		// maze(left, up, right, down, check_if_visited)
		maze = new int[length, width, 5];

		history.Add (new Vector2 (row, col));

		while (history.Count > 0) {
			// Set this cell as visited.
			maze [row, col, 4] = 1;

			// Clear the check array.
			check.Clear();

			// Check if the adjacent cells are valid for moving to.
			if (col > 0 && maze [row, col - 1, 4] == 0)
				check.Add ('L');

			if (row > 0 && maze [row - 1, col, 4] == 0)
				check.Add ('U');

			if (col < length - 1 && maze [row, col + 1, 4] == 0)
				check.Add ('R');

			if (row < width - 1 && maze [row + 1, col, 4] == 0)
				check.Add ('D');

			// If there is a valid cell to move to.
			if (check.Count > 0) {
				history.Add (new Vector2 (row, col));
				char direction = System.Convert.ToChar (check [Random.Range (0, check.Count)]);

				// Mark the walls between cells as open if we move.
				switch (direction) {
				case 'L':
					maze [row, col, 0] = 1;
					col--;
					maze [row, col, 2] = 1;
					break;

				case 'U':
					maze [row, col, 1] = 1;
					row--;
					maze [row, col, 3] = 1;
					break;

				case 'R':
					maze [row, col, 2] = 1;
					col = col + 1;
					maze [row, col, 0] = 1;
					break;

				case 'D':
					maze [row, col, 3] = 1;
					row++;
					maze [row, col, 1] = 1;
					break;

				}

			} else {
				// Retrace one step back in history if no move is possible.
				Vector2 retrace = (Vector2)history [history.Count - 1];
				row = (int)retrace.x;
				col = (int)retrace.y;

				history.RemoveAt (history.Count - 1);

			}

			maze [0, 0, 0] = 1;
			maze [length - 1, width - 1, 2] = 1; 

		}
	}
}
