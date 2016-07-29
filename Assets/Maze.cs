using UnityEngine;
using System.Collections;

public class Maze : MonoBehaviour {
	public int length = 10;
	public int width = 10;

	private GameObject floor;

	private int[,,] maze;

	void Start () {
		Setup();
	}
	
	void Update () {
	
	}

	void Setup() {
		ArrayList history = new ArrayList(), 
					check = new ArrayList();

		int row = 0,
			col = 0;

		// The array maze is going to hold the information for each cell.
		// The first four coordinates tell if walls exist on those sides and the fifth indicates if the cell has benn visited in the search.
		// maze(left, up, right, down, check_if_visited)
		maze = new int[length, width, 5];

		history.Add(new Vector2 (row, col));

		while (history.Count > 0) {
			// Set this cell as visited.
			maze [row, col, 4] = 1;

			// Check if the adjacent cells are valid for moving to.
			if (col > 0 && maze[row, col - 1, 4] == 0)
				check.Add('L');

			if (col > 0 && maze[row - 1, col, 4] == 0)
				check.Add('U');

			if (col < length - 1 && maze[row, col + 1, 4] == 0)
				check.Add('R');

			if (col < width - 1 && maze[row + 1, col, 4] == 0)
				check.Add('D');

			// If there is a valid cell to move to.
			if (check.Count > 0) {
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
					col++;
					maze [row, col, 0] = 1;
					break;

				case 'D':
					maze [row, col, 3] = 1;
					row++;
					maze [row, col, 1] = 1;
					break;

				}

			} else {
				// Retrace one step bac in history if no move is possible.
				Vector2 retrace = (Vector2)history [history.Count - 1];
				row = (int)retrace.x;
				col = (int)retrace.y;

				history.RemoveAt (history.Count - 1);
			
			}

			maze [0, 0, 0] = 1;
			maze [length - 1, width - 1, 2] = 1; 
		
		}
			
	}

	void CreateMaze() {
		

	}

	void CreateWall() {
		GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Plane);
		wall.transform.localScale = new Vector3 (0.2f, 1.0f, 0.4f);
		//wall.transform.rotation = 
	
	}

	void CreateFloor() {
		floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
		floor.transform.localScale = new Vector3 (length / 5, 1, width / 5);
		floor.transform.position = new Vector3 (length, 0, width);

	}
}
