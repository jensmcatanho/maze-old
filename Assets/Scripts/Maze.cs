using UnityEngine;
using System.Collections;

[System.Serializable]
public class Maze {
	int m_Length;
	int m_Width;

	Cell[,] m_Cells;

	// Maze constructor.
	public Maze(int length, int width, int cellSize) {
		InitializeMaze(length, width, cellSize);
	}

	// Helper function to initialize the matrix.
	void InitializeMaze(int length, int width, int cellSize) {
		m_Length = length;
		m_Width = width;

		m_Cells = new Cell[length, width];

		for (int row = 0; row < length; row++)
			for (int col = 0; col < width; col++)
				m_Cells [row, col] = new Cell (cellSize);
	}

	// Indexer for the cells in the maze.
	public Cell this[int row, int col] {
		get { return m_Cells [row, col]; }

		set { m_Cells[row, col] = value; }
	}

	// Read-only accessors to the maze properties.
	public int Length { get { return m_Length; } }
	public int Width { get { return m_Width; } }
	public int cellSize { get { return m_Cells[0, 0].Size; } }

}

/*
[System.Serializable]
public class Maze {
	public int length;
	public int width;
	public int cellSize = 1;

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
			DFSMaze ();
			break;

		case 1:
			pDeadEnd = 0.36f;
			PrimMaze ();
			break;
		}
			
		ChestProbabilitySetup ();
		CreateMaze ();
	}

	void CreateMaze() {
		CreateFloor ();

		// Create walls in the diagonal part of the maze.
		for (int i = 0; i < length; i++) {
			if (maze [i, i, (int)Direction.LEFT] == 0)
				CreateWall (new Vector3 ((2 * i + 1) * cellSize, 2.0f, 2 * i * cellSize), new Vector3 (90.0f, 90.0f, 0.0f));

			if (maze [i, i, (int)Direction.DOWN] == 0)
				CreateWall (new Vector3 ((2 * i + 2) * cellSize, 2.0f, (2 * i + 1) * cellSize), new Vector3 (90.0f, 0.0f, 0.0f));

			if (maze [i, i, (int)Direction.UP] == 0)
				CreateWall (new Vector3 (2 * i * cellSize, 2.0f, (2 * i + 1) * cellSize), new Vector3 (90.0f, 0.0f, 180.0f));

			if (maze [i, i, (int)Direction.RIGHT] == 0)
				CreateWall (new Vector3 ((2 * i + 1) * cellSize, 2.0f, (2 * i + 2) * cellSize), new Vector3 (90.0f, -90.0f, 0.0f));
		}
		

		for (int i = 0; i < length; i++) {
			for (int j = i + 1; j < width; j++) {
				// Create walls and chests in the lower triangular part of the maze.
				if (maze [j, i, (int)Direction.LEFT] == 0)
					CreateWall (new Vector3 ((2 * j + 1) * cellSize, 2.0f, 2 * i * cellSize), new Vector3(90.0f, 90.0f, 0.0f));

				if (maze [j, i, (int)Direction.DOWN] == 0)
					CreateWall (new Vector3((2 * j + 2) * cellSize, 2.0f, (2 * i + 1) * cellSize), new Vector3(90.0f, 0.0f, 0.0f));

				if (CheckDeadEnd (j, i) && Random.value < pChest)
					CreateChest (j, i);

				// Create walls in the upper triangular part of the maze.
				if (maze [i, j, (int)Direction.UP] == 0)
					CreateWall (new Vector3(2 * i * cellSize, 2.0f, (2 * j + 1) * cellSize), new Vector3(90.0f, 0.0f, 180.0f));

				if (maze [i, j, (int)Direction.RIGHT] == 0)
					CreateWall (new Vector3((2 * i + 1) * cellSize, 2.0f, (2 * j + 2) * cellSize), new Vector3(90.0f, -90.0f, 0.0f));

				if (CheckDeadEnd (i, j) && Random.value < pChest)
					CreateChest (i, j);

			}
		}

	}

	bool CheckDeadEnd(int row, int col) {
		int numWalls = 0;

		for (int i = 0; i < 4; i++)
			if (maze [row, col, i] == 0)
				numWalls++;

		return numWalls == 3 ? true : false;
	}

	void ChestProbabilitySetup() {
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
		 *

		pChest = 0.05f / pDeadEnd;
	}

	void CreateWall(Vector3 position, Vector3 rotation) {
		Quaternion r = Quaternion.identity;
		r.eulerAngles = rotation;

		GameObject localWall = MonoBehaviour.Instantiate (wall, position, r) as GameObject;
		localWall.transform.localScale *= cellSize;
	}

	void CreateFloor() {
		GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
		floor.transform.localScale = new Vector3 (length / 5 * cellSize, 1, width / 5 * cellSize);
		floor.transform.position = new Vector3 (length * cellSize, 0, width * cellSize);

	}

	void CreateChest(int row, int col) {
		Quaternion r = Quaternion.identity;
		Vector3 chestPosition = new Vector3 ((2 * row + 1) * cellSize, 0.0f, (2 * col + 1) * cellSize);
		Vector3 chestRotation = new Vector3();
		Direction dir = Direction.DOWN;

		// Check which direction the chest is facing (which direction of the cell is open).
		for (int k = 0; k < 4; k++)
			if (maze[row, col, k] == 1) {
				dir = (Direction)k;
				break;
			}

		switch (dir) {
		case Direction.LEFT:
			chestRotation = new Vector3 (0.0f, 180.0f, 0.0f);
			break;

		case Direction.UP:
			chestRotation = new Vector3 (0.0f, -90.0f, 0.0f);
			break;

		case Direction.RIGHT:
			chestRotation = new Vector3 (0.0f, 0.0f, 0.0f);
			break;

		case Direction.DOWN:
			chestRotation = new Vector3 (0.0f, 90.0f, 0.0f);
			break;

		}

		r.eulerAngles = chestRotation;
		MonoBehaviour.Instantiate (chest, chestPosition, r);
		nChests++;
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
*/