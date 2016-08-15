using UnityEngine;
using System.Collections;

[System.Flags]
public enum Wall {
	None  =      0,
	Left  = 1 << 0,
	Up    = 1 << 1,
	Right = 1 << 2,
	Down  = 1 << 3
		
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                

[System.Flags]
public enum Type {
	None     =      0,
	Entrance = 1 << 0,
	Exit     = 1 << 1

}

public class Cell {
	public int m_Size;
	Wall m_Walls;
	Type m_Type;

	public void SetWall(Wall target) {
		m_Walls = m_Walls | target;
	}

	public void UnsetWall(Wall target) {
		m_Walls = m_Walls & (~target);
	}

	public void ToogleWall(Wall target) {
		m_Walls = m_Walls ^ target;
	}

	public bool HasWall(Wall target) {
		return (m_Walls & target) == target; 
	}

	public void SetType(Type target) {
		m_Type = m_Type | target;
	}

	public void UnsetType(Type target) {
		m_Type = m_Type & (~target);
	}

	public void ToogleType(Type target) {
		m_Type = m_Type ^ target;
	}

	public bool HasType(Type target) {
		return (m_Type & target) == target; 
	}
}