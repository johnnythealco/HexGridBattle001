using UnityEngine;
using System.Collections;

using Gamelogic.Grids;

//using Gamelogic;

public class TerrainCell : SpriteCell
{

	public SpriteRenderer foreground;
	public Unit unit;
	public SpriteRenderer border;
//	public Enemy enemy;
	public CellContents contents;

//	public bool containsEnemy = false;

	public bool IsAccessible;

	public float Cost;

}

public enum CellContents{Empty, Player, Enemy}
