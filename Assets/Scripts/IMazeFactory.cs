using UnityEngine;
using System.Collections;

public interface IMazeFactory {
	Maze CreateMaze(int length, int width, int cellSize);
}
