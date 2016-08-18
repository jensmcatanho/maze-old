using UnityEngine;
using System.Collections;

public class DFSFactory : IMazeFactory {

	public Maze CreateMaze(int length, int width, int cellSize) {
		Maze maze = new Maze (length, width, cellSize);

		System.Diagnostics.Debug.Assert(!maze[0, 0].HasWall(Wall.None));

		ArrayList history = new ArrayList ();
		ArrayList neighbors = new ArrayList ();

		// 1. Start in the first cell.
		int row = 0;
		int col = 0;

		// 2. Add it to the history stack.
		history.Add (new Vector2 (row, col));

		while (history.Count > 0) {
			// 3. Mark it as visited.
			maze[row, col].Visited = Status.True;

			// 4. Check which of its neighbors were not yet visited.
			neighbors.Clear();
			if (col > 0 && maze [row, col - 1].Visited == Status.True)
				neighbors.Add ('L');

			if (row > 0 && maze [row - 1, col].Visited == Status.True)
				neighbors.Add ('U');

			if (col < length - 1 && maze [row, col + 1].Visited == Status.True)
				neighbors.Add ('R');

			if (row < width - 1 && maze [row + 1, col].Visited == Status.True)
				neighbors.Add ('D');

			// 5a. If there is a neighbor not yet visited, choose one randomly to connect to the current cell. 
			if (neighbors.Count > 0) {
				history.Add (new Vector2 (row, col));
				char direction = System.Convert.ToChar (neighbors [Random.Range (0, neighbors.Count)]);

				switch (direction) {
				case 'L':
					maze [row, col].SetWall(Wall.Left);
					maze [row, --col].SetWall(Wall.Right);
					break;

				case 'U':
					maze [row, col].SetWall(Wall.Up);
					maze [--row, col].SetWall(Wall.Down);
					break;

				case 'R':
					maze [row, col].SetWall(Wall.Right);
					maze [row, ++col].SetWall(Wall.Left);
					break;

				case 'D':
					maze [row, col].SetWall(Wall.Down);
					maze [++row, col].SetWall(Wall.Up);
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
		maze [0, 0].SetType (Type.Entrance);
		maze [length - 1, width - 1].SetType (Type.Exit);
		maze [length - 1, width - 1].UnsetWall (Wall.Right); 

		return maze;
	}

}
