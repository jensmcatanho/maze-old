using UnityEngine;
using System.Collections;

[System.Serializable]
public class Maze {
	public int length;
	public int width;

	public Vector3 spawnPoint;
	public Vector3 finishPoint;
	enum Direction {LEFT, UP, RIGHT, DOWN};

	// Changes through the level types.
	public GameObject chest;
	public GameObject floor;
	public GameObject wall;

	// Chest spawning.
	public float pDeadEnd = 0;
	public float pChest = 0;
	public int nChests = 0;

	private int[,,] maze;

	public void Init (int l, int w) {
		length = l;
		width = w;
	}

	public void Setup() {
		int type = Random.Range (0, 2);

		switch (type) {
		case 0:
			pDeadEnd = 0.1f;
			ChestSetup ();
			DFSMaze ();
			break;

		case 1:
			pDeadEnd = 0.36f;
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
					CreateWall (new Vector3 (2 * i + 1, 2.0f, 2 * j), new Vector3(90.0f, 90.0f, 0.0f));
					numWalls++;
				}

				if (maze [i, j, (int)Direction.UP] == 0) {
					CreateWall (new Vector3(2 * i, 2.0f, 2 * j + 1), new Vector3(90.0f, 0.0f, 180.0f));
					numWalls++;
				}

				if (maze [i, j, (int)Direction.RIGHT] == 0) {
					CreateWall (new Vector3(2 * i + 1, 2.0f, 2 * j + 2), new Vector3(90.0f, -90.0f, 0.0f));
					numWalls++;
				}

				if (maze [i, j, (int)Direction.DOWN] == 0) {
					CreateWall (new Vector3(2 * i + 2, 2.0f, 2 * j + 1), new Vector3(90.0f, 0.0f, 0.0f));
					numWalls++;
				}

				if (numWalls == 3)
					Debug.Log ("Dead end");

				if (numWalls == 3 && Random.value < pChest) {
					nChests++;
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

	void ChestSetup() {
		/*  Equation 1: nC = (l * w) * dE * pC
		 *  Equation 2: nC = (l * w) * 0.05
		 * 
		 *  (l * w) * 0.05 = (l * w) * dE * pC =>
		 *  dE * pC = 0.05
		 * 
		 *  l = maze's length
		 *  w = maze's width
		 *  nC = maximum number of chests
		 *  dE = percentage of dead ends
		 *  pC = probability of chest spawning
		 * 
		 */
		
		pChest = 0.05f / pDeadEnd;
	}

	void CreateWall(Vector3 position, Vector3 rotation) {
		Quaternion r = Quaternion.identity;
		r.eulerAngles = rotation;

		MonoBehaviour.Instantiate (wall, position, r);
	}

	void CreateFloor() {
		GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
		floor.transform.localScale = new Vector3 (length / 5, 1, width / 5);
		floor.transform.position = new Vector3 (length, 0, width);

	}

	void CreateChest(Vector3 position, Vector3 rotation) {
		Quaternion r = Quaternion.identity;
		r.eulerAngles = rotation;

		MonoBehaviour.Instantiate (chest, position, r);
	}

	void DFSMaze() {
		ArrayList history = new ArrayList ();
		ArrayList neighbors = new ArrayList ();

		// 1. Start in the first cell.
		int row = 0;
		int col = 0;

		maze = new int[length, width, 5];

		// 2. Add it to the history stack.
		history.Add (new Vector2 (row, col));

		while (history.Count > 0) {
			// 3. Mark it as visited.
			maze [row, col, 4] = 1;

			// 4. Check which of its neighbors were not yet visited.
			neighbors.Clear();
			if (col > 0 && maze [row, col - 1, 4] == 0)
				neighbors.Add ('L');

			if (row > 0 && maze [row - 1, col, 4] == 0)
				neighbors.Add ('U');

			if (col < length - 1 && maze [row, col + 1, 4] == 0)
				neighbors.Add ('R');

			if (row < width - 1 && maze [row + 1, col, 4] == 0)
				neighbors.Add ('D');

			// 5a. If there is a neighbor not yet visited, choose one randomly to connect to the current cell. 
			if (neighbors.Count > 0) {
				history.Add (new Vector2 (row, col));
				char direction = System.Convert.ToChar (neighbors [Random.Range (0, neighbors.Count)]);

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

				// 5b. If there isn't a neighbor to visit, backtrack one step.
			} else {
				Vector2 retrace = (Vector2)history [history.Count - 1];
				row = (int)retrace.x;
				col = (int)retrace.y;

				history.RemoveAt (history.Count - 1);
			}

			// 6. If there are still cells in the history list, go back to step 3.
		}

		// 7. Open an entrance and a exit to the maze.
		maze [0, 0, 0] = 1;
		maze [length - 1, width - 1, 2] = 1; 
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
}