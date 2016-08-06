using UnityEngine;
using System.Collections;

public class Maze : ScriptableObject {
	enum Direction {LEFT, UP, RIGHT, DOWN};

	public int length; // x
	public int width; // z

	public GameObject chest;
	private GameObject floor;

	private int[,,] maze;

	public void Init (int l, int w) {
		length = l;
		width = w;
	}

	public void Setup() {
		int type = Random.Range (0, 2);

		switch (type) {
		case 0:
			DFSMaze ();
			break;
		case 1:
			PrimMaze ();
			break;
		}

		CreateMaze ();
	}

	void CreateMaze() {
		CreateFloor ();

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < width; j++) {
				int numWalls = 0;

				if (maze [i, j, (int)Direction.LEFT] == 0) {
					CreateWall (new Vector3 (2 * i + 1, 2.0f, 2 * j), new Vector3 (90.0f, 0.0f, 0.0f));
					numWalls++;
				}

				if (maze [i, j, (int)Direction.UP] == 0) {
					CreateWall (new Vector3(2 * i, 2.0f, 2 * j + 1), new Vector3(90.0f, 90.0f, 0.0f));
					numWalls++;
				}

				if (maze [i, j, (int)Direction.RIGHT] == 0) {
					CreateWall (new Vector3(2 * i + 1, 2.0f, 2 * j + 2), new Vector3(90.0f, 0.0f, 180.0f));
					numWalls++;
				}

				if (maze [i, j, (int)Direction.DOWN] == 0) {
					CreateWall (new Vector3(2 * i + 2, 2.0f, 2 * j + 1), new Vector3(90.0f, -90.0f, 0.0f));
					numWalls++;
				}

				if (numWalls == 3 && Random.Range (0, 2) > 0) {
					Vector3 chestLocation = new Vector3(2 * i + 1, 0.0f, 2 * j + 1);
					Direction dir = Direction.DOWN;

					// Check which direction the chest is facing (which direction of the cell is open).
					for (int k = 0; k < 4; k++)
						if (maze[i, j, k] == 1) {
							dir = (Direction)k;
							break;
						}

					switch (dir) {
						case Direction.LEFT:
							CreateChest (chestLocation, new Vector3 (0.0f, 180.0f, 0.0f));
							break;

						case Direction.UP:
							CreateChest (chestLocation, new Vector3 (0.0f, -90.0f, 0.0f));
							break;

						case Direction.RIGHT:
							CreateChest (chestLocation, new Vector3 (0.0f, 0.0f, 0.0f));
							break;

						case Direction.DOWN:
							CreateChest (chestLocation, new Vector3 (0.0f, 90.0f, 0.0f));
							break;

					}		
				}
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

	void CreateChest(Vector3 position, Vector3 rotation) {
		GameObject go = GameObject.Find ("GameManager");
		GameManager gm = go.GetComponent (typeof(GameManager)) as GameManager;

		Quaternion r = Quaternion.identity;
		r.eulerAngles = rotation;

		Instantiate (gm.chest, position, r);
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
					maze [row, col, (int)Direction.LEFT] = 1;
					maze [row, --col, (int)Direction.RIGHT] = 1;
					break;

				case 'U':
					maze [row, col, (int)Direction.UP] = 1;
					maze [--row, col, (int)Direction.DOWN] = 1;
					break;

				case 'R':
					maze [row, col, (int)Direction.RIGHT] = 1;
					maze [row, ++col, (int)Direction.LEFT] = 1;
					break;

				case 'D':
					maze [row, col, (int)Direction.DOWN] = 1;
					maze [++row, col, (int)Direction.UP] = 1;
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

	void PrimMaze() {
		ArrayList frontier = new ArrayList ();
		ArrayList neighbors = new ArrayList ();

		maze = new int[length, width, 5];

		// 1. Pick a cell randomly.
		int row = Random.Range (0, length);
		int col = Random.Range (0, width);

		// 2. Mark it as visited.
		maze[row, col, 4] = 1;

		do {
			// 3. Add its neighbors to the frontier list and mark them as part of the frontier.
			if (col > 0 && maze [row, col - 1, 4] == 0) {
				frontier.Add (new Vector2 (row, col - 1)); // Left
				maze [row, col - 1, 4] = 2;
			}

			if (row > 0 && maze [row - 1, col, 4] == 0) {
				frontier.Add (new Vector2 (row - 1, col)); // Up
				maze [row - 1, col, 4] = 2;
			}

			if (col < length - 1 && maze [row, col + 1, 4] == 0) {
				frontier.Add (new Vector2 (row, col + 1)); // Right
				maze [row, col + 1, 4] = 2;
			}

			if (row < width - 1 && maze [row + 1, col, 4] == 0) {
				frontier.Add (new Vector2 (row + 1, col)); // Down
				maze [row + 1, col, 4] = 2;
			}

			// 4. Pick a cell in the frontier list randomly.
			Vector2 nextCell = (Vector2)frontier [Random.Range (0, frontier.Count)];
			row = (int)nextCell.x; 
			col = (int)nextCell.y;

			// 5. Mark it as visited and remove it from the frontier list.
			frontier.Remove (nextCell);
			maze[row, col, 4] = 1;

			// 6. Check which of its neighbors were already visited.
			neighbors.Clear();

			if (col > 0 && maze [row, col - 1, 4] == 1)
				neighbors.Add('L');

			if (row > 0 && maze [row - 1, col, 4] == 1)
				neighbors.Add('U');

			if (col < length - 1 && maze [row, col + 1, 4] == 1)
				neighbors.Add('R');

			if (row < width - 1 && maze [row + 1, col, 4] == 1)
				neighbors.Add('D');

			// 7. Randomly choose a neighbor to connect to the current cell.
			char direction = System.Convert.ToChar(neighbors[Random.Range(0, neighbors.Count)]);

			switch (direction) {
			case 'L':
				maze [row, col, (int)Direction.LEFT] = 1;
				maze [row, col - 1, (int)Direction.RIGHT] = 1;
				break;

			case 'U':
				maze [row, col, (int)Direction.UP] = 1;
				maze [row - 1, col, (int)Direction.DOWN] = 1;
				break;

			case 'R':
				maze [row, col, (int)Direction.RIGHT] = 1;
				maze [row, col + 1, (int)Direction.LEFT] = 1;
				break;

			case 'D':
				maze [row, col, (int)Direction.DOWN] = 1;
				maze [row + 1, col, (int)Direction.UP] = 1;
				break;

			}

		// 8. If there are still cells in the frontier list, go back to step 3.
		} while (frontier.Count > 0);
			
		// 9. Open an entrance and a exit to the maze.
		maze [0, 0, 0] = 1;
		maze [length - 1, width - 1, 2] = 1; 
	}

	void PrimsMaze() {
		ArrayList frontier = new ArrayList (),
		check = new ArrayList ();

		// The array maze is going to hold the information for each cell.
		// The first four coordinates tell if walls exist on those sides and the fifth indicates if the cell has benn visited in the search.
		// maze(left, up, right, down, check_if_visited)
		maze = new int[length, width, 5];

		// Select a random cell from the maze.
		int row = Random.Range (0, length);
		int col = Random.Range(0, width);

		// Set this cell as visited.
		maze[row, col, 4] = 1;

		// Add the adjacent cells that are not part of the maze (and within the grid size) to the frontier list.
		if (col > 0 && maze [row, col - 1, 4] == 0)
			frontier.Add (new Vector2 (row, col - 1));

		if (row > 0 && maze [row - 1, col, 4] == 0)
			frontier.Add (new Vector2 (row - 1, col));

		if (col < length - 1 && maze [row, col + 1, 4] == 0)
			frontier.Add (new Vector2 (row, col + 1));

		if (row < width - 1 && maze [row + 1, col, 4] == 0)
			frontier.Add (new Vector2 (row + 1, col));

		Debug.Log (row + " " + col);
		Debug.Log (frontier.Count);

		do {
			int index = Random.Range (0, frontier.Count);
			Vector2 nextCell = (Vector2)frontier [index];
			row = (int)nextCell.x;
			col = (int)nextCell.y;
			frontier.RemoveAt (index);

			int numCells = 0;
			for (int i = 0; i < length; i++)
				for (int j = 0; j < width; j++)
					if (maze[i, j, 4] == 1)
						numCells++;

			Debug.Log("Cells in the maze: " + numCells);

			// Set this cell as visited.
			maze[row, col, 4] = 1;

			check.Clear();

			// Add the adjacent cells that are not part of the maze (and within the grid size) to the frontier list.
			if (col > 0 && maze [row, col - 1, 4] == 0)
				frontier.Add (new Vector2 (row, col - 1));

			if (row > 0 && maze [row - 1, col, 4] == 0)
				frontier.Add (new Vector2 (row - 1, col));

			if (col < length - 1 && maze [row, col + 1, 4] == 0)
				frontier.Add (new Vector2 (row, col + 1));

			if (row < width - 1 && maze [row + 1, col, 4] == 0)
				frontier.Add (new Vector2 (row + 1, col));

			// Check which adjacent cells of the next cell are already part of the maze.
			if (col > 0 && maze [row, col - 1, 4] == 1)
				check.Add('L');

			if (row > 0 && maze [row - 1, col, 4] == 1)
				check.Add('U');

			if (col < length - 1 && maze [row, col + 1, 4] == 1)
				check.Add('R');

			if (row < width - 1 && maze [row + 1, col, 4] == 1)
				check.Add('D');

			// Choose which cell the nextCell will be conected to.
			Debug.Log(check.Count);
			char direction = System.Convert.ToChar(check[Random.Range(0, check.Count)]);

			switch (direction) {
			case 'L':
				maze [row, col, (int)Direction.LEFT] = 1;
				col--;
				maze [row, col, (int)Direction.RIGHT] = 1;
				break;

			case 'U':
				maze [row, col, (int)Direction.UP] = 1;
				row--;
				maze [row, col, (int)Direction.DOWN] = 1;
				break;

			case 'R':
				maze [row, col, (int)Direction.RIGHT] = 1;
				col = col + 1;
				maze [row, col, (int)Direction.LEFT] = 1;
				break;

			case 'D':
				maze [row, col, (int)Direction.DOWN] = 1;
				row++;
				maze [row, col, (int)Direction.UP] = 1;
				break;

			}

		} while (frontier.Count > 0);

		maze [0, 0, 0] = 1;
		maze [length - 1, width - 1, 2] = 1; 
	}
}
