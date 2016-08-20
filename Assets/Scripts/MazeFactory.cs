using UnityEngine;
using System.Collections;

public abstract class MazeFactory {
	protected Maze maze;

	protected float pDeadEnd = 0;
	protected float pChest = 0;

	// int nChests = 0;

	public Maze CreateMaze(int length, int width, int cellSize) {
		maze = new Maze (length, width, cellSize);

		CreatePath ();
		CreateChests ();

		return maze;
	}

	protected abstract void CreatePath ();

	protected abstract void ChestSetup (float pDeandEnd);

	void CreateChests () {
		for (int row = 0; row < maze.Length; row++)
			for (int col = 0; col < maze.Width; col++)
				if (CheckDeadEnd (row, col) && Random.value < pChest)
					maze [row, col].HasChest = true;
		
	}

	bool CheckDeadEnd(int row, int col) {
		int numWalls = 0;

		if (maze [row, col].HasWall (Wall.Left))
			numWalls++;
		
		if (maze [row, col].HasWall (Wall.Up))
			numWalls++;
		
		if (maze [row, col].HasWall (Wall.Right))
			numWalls++;
		
		if (maze [row, col].HasWall (Wall.Down))
			numWalls++;

		return numWalls == 3 ? true : false;
	}

}
