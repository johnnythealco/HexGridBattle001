using UnityEngine;
using System.Collections;
using Gamelogic.Grids;
using Gamelogic;
using System.Collections.Generic;
using Gamelogic.Grids.Examples;
using System.Linq;

public class TerrainBehaviour : GridBehaviour<FlatHexPoint>
{
	#region Public Variables

	public LandscapeTileSet tiles;
	public GameObject unitPrefab;
	public GameObject enemyPrefab;
	public TurnManager turn;

	#endregion

	#region Private Variables

	private IGrid<TerrainCell, FlatHexPoint> terrainGrid;
	private bool somethingSelected;
	private Unit unitSelected;
	private FlatHexPoint selectedPoint;
	private Dictionary<FlatHexPoint, float> AvailableMoves;
	public List<FlatHexPoint> validTargets;
	private FlatHexPoint enemyPosition; 

	#endregion

	#region Map Initialisation

	/*
		Cast the basic Grid to a grid of Terrain Cells & Flat Hex Point
		Iitialise the grid as grassland
		Create the mountains
		Create the Unist
	*/
	override public void InitGrid ()
	{
		somethingSelected = false;
		terrainGrid = Grid.CastValues<TerrainCell, FlatHexPoint> ();
		validTargets = new List<FlatHexPoint> ();


		foreach (var point in Grid)
		{
			CreateGrassLand (point);
			terrainGrid [point].contents = CellContents.Empty;
		}

		var randomMountains = Grid.SampleRandom (10);
		foreach (var point in randomMountains)
		{
			CreateMountains (point);
			var neighbours = terrainGrid.GetNeighbors (point);
			foreach (var neighbour in neighbours)
			{
				CreateForrestedMountain (neighbour);
			}
		}

		var rndUnits = Grid.SampleRandom (3);

		foreach (var point in rndUnits)
		{
			CreateUnit (point);
		}

		var rndEnemies = Grid.SampleRandom (3);
		foreach (var point in rndEnemies)
		{
			CreateEnemy (point);
		}
	}

	#endregion

	#region Terrain Creation Functions

	private void CreateGrassLand (FlatHexPoint point)
	{
		
		var cell = terrainGrid [point];

		cell.Color = tiles.tiles [0].color;
		cell.foreground.sprite = tiles.tiles [0].image;
		cell.IsAccessible = true;
		cell.Cost = tiles.tiles [0].Cost;

	}

	private void CreateMountains (FlatHexPoint point)
	{

		var cell = terrainGrid [point];

		cell.Color = tiles.tiles [4].color;
		cell.foreground.sprite = tiles.tiles [4].image;
		cell.IsAccessible = true;
		cell.Cost = tiles.tiles [4].Cost;

	}

	private void CreateForrestedMountain (FlatHexPoint point)
	{

		var cell = terrainGrid [point];

		cell.Color = tiles.tiles [3].color;
		cell.foreground.sprite = tiles.tiles [3].image;
		cell.IsAccessible = true;
		cell.Cost = tiles.tiles [3].Cost;

	}

	#endregion

	#region Unit & Enemy Creation

	private void CreateUnit (FlatHexPoint point)
	{

		GameObject newUnit = Instantiate (unitPrefab, Map [point], Quaternion.identity) as GameObject;
		Unit unit = newUnit.GetComponent<Unit> ();
		terrainGrid [point].contents = CellContents.Player;

		terrainGrid [point].unit = unit;
		terrainGrid [point].IsAccessible = false;
		unit.turnmanager = turn;
	}

	private void CreateEnemy (FlatHexPoint point)
	{

		GameObject newEnemy = Instantiate (enemyPrefab, Map [point], Quaternion.identity) as GameObject;
		Unit unit = newEnemy.GetComponent<Unit> ();
		terrainGrid [point].contents = CellContents.Enemy;

		terrainGrid [point].unit = unit;
		terrainGrid [point].IsAccessible = false;
		unit.turnmanager = turn;

	}

	public Unit GetUnitAtPoint(FlatHexPoint point)
	{
		return terrainGrid [point].unit;
	}

	#endregion

	#region User Interaction

	public void OnLeftClick (FlatHexPoint point)
	{
		if (turn.PlayersTurn)
		{
			switch (terrainGrid [point].contents)
			{
			case CellContents.Player:
				if (!somethingSelected && terrainGrid [point].unit != null)
				{
					SelectUnitAtPoint (point); 
					AvailableMoves = GetAvailableMoves (point);
					validTargets = GetValidTargets ();
					HighlightMove (AvailableMoves.Keys);
					HighlightTargets (validTargets);
				}
				break;

			case CellContents.Empty:
				if (somethingSelected && AvailableMoves.ContainsKey (point))
				{
					MoveUnitFromPointToPoint (selectedPoint, point);
					EndAction ();
					turn.PlayersTurn = false;

				} 
				break;

			case CellContents.Enemy:
				if (somethingSelected && validTargets.Contains (point))
				{
					unitSelected.AttackAnnimation ();
					EndAction ();
					turn.PlayersTurn = false;
				}
//				else if (!somethingSelected)
//				{
//					terrainGrid [GetClosestPlayer (point)].Color = Color.red;
//				
//				}


				break;
			}

		}
	}

	#endregion

	#region Unit Selection & Movement

	public void MoveUnitFromPointToPoint (FlatHexPoint start, FlatHexPoint end) 
	{
		terrainGrid [end].unit = terrainGrid[start].unit;														//register the unit at the new location
		terrainGrid [end].IsAccessible = false;																	//Mark the cell as occupited
		terrainGrid[end].contents = terrainGrid[start].contents;


		terrainGrid [start].unit.Move (GetWaypoints (start, end));

		terrainGrid[start].unit = null;																					//unregister the unit from their last location
		terrainGrid[start].IsAccessible = true;																	//Make the now empty cell accessible
		terrainGrid[start].contents = CellContents.Empty;
	}



	public List<Vector3> GetWaypoints(FlatHexPoint start, FlatHexPoint end)
	{
		var path = GetGridPath(start, end); 														//Get the grid path to the target
		List<Vector3> waypoints = new List<Vector3> ();										//Create an empty of waypoints

		foreach(var waypoint in path)
		{
			waypoints.Add (Map [waypoint]);														//Add each step on the pat to the list of waypoints
		}
		return waypoints;
	}

	public void SelectUnitAtPoint (FlatHexPoint point)
	{
		validTargets.Clear ();
		unitSelected = terrainGrid [point].unit;
		somethingSelected = true;
		selectedPoint = point;


	}



	#endregion

	#region Grid Highlighting

	public void HighlightMove (IEnumerable<FlatHexPoint> cells)
	{
		//Activates the border sprite on the set of cells provided
		foreach (var point in cells)
		{
			terrainGrid [point].border.enabled = true;
//			terrainGrid [point].border.color = Color.blue;
		}

	}

	public void UnHighlightCells (IEnumerable<FlatHexPoint> cells)
	{
		//deactivates the border on the set of cells provided
		foreach (var point in cells)
		{
			terrainGrid [point].border.enabled = false;
		}

	}

	public void HighlightTargets (List<FlatHexPoint> targets)
	{

		foreach (var point in targets)
		{
			terrainGrid [point].border.enabled = true;
			terrainGrid [point].border.color = Color.red;

		}

	}

	public void EndAction ()
	{
		somethingSelected = false;								//Set somethingSelected to false
		unitSelected = null;									//Set the unitSelected to null
		UnHighlightCells (AvailableMoves.Keys);
		UnHighlightCells (validTargets);
		AvailableMoves.Clear ();
		validTargets.Clear ();
	}

	#endregion


	#region Moves & Target Algorithms

	public Dictionary<FlatHexPoint, float> GetAvailableMoves (FlatHexPoint point)
	{
		//Returns a C# Dictionary of move points available and their movement cost, 
		//Returns null if there is no unit in the selected cell

		if (terrainGrid [point].unit != null)
		{

			var AvailableMoves = Algorithms.GetPointsInRangeCost<TerrainCell, FlatHexPoint>

			(terrainGrid, point,
				cell => cell.IsAccessible,
				(p, q) => (terrainGrid [p].Cost + terrainGrid [q].Cost / 2.0f),
				terrainGrid [point].unit.movement
			);
				
			return AvailableMoves;
		}

		return null;
	}

	public List<FlatHexPoint> GetValidTargets ()
	{
		List<FlatHexPoint> result = new List<FlatHexPoint> ();

		if (unitSelected != null && AvailableMoves != null)
		{
			foreach (var pointInRange in AvailableMoves.Keys)
			{
				//For each point the unit can move to
				//Check what enemies and in range of that point
				var EnemiesInRangeFromPoint = Algorithms.GetPointsInRange<TerrainCell, FlatHexPoint>
												(terrainGrid,
					                            pointInRange,
					cell => (cell.contents == CellContents.Enemy),
					                            (p, q) => 1,
					                            unitSelected.attackRange);
								
				//Add the any valid target to the result
				//if the target is not already in the list
				foreach (var target in EnemiesInRangeFromPoint)
				{
					if (terrainGrid [target].contents == CellContents.Enemy && !result.Contains(target))
					{
						result.Add (target);

					}
						
				}
			}

			return result;
			
		}

		return null;

	}

	public List<FlatHexPoint> GetGridPath (FlatHexPoint start, FlatHexPoint end)
	{
		List<FlatHexPoint> result = new List<FlatHexPoint> ();

		var path = Algorithms.AStar<TerrainCell, FlatHexPoint>
			(terrainGrid, start, end
			,
			(p, q) => p.DistanceFrom(q),
			c => true,
			(p, q) =>  (terrainGrid [p].Cost + terrainGrid [q].Cost / 2) 
		);

		result = path.ToList ();
//		result.Remove (start);

		return result;
	}

	public FlatHexPoint GetMaxMove (FlatHexPoint source, FlatHexPoint target)  
	{
		//Get the best complete path to the target
		var path = GetGridPath (source, target);

		//Get a kvp of all available moves and the cost
		var range =  Algorithms.GetPointsInRangeCost<TerrainCell, FlatHexPoint>
													(terrainGrid, source,
														cell => cell.IsAccessible,
														(p, q) => (terrainGrid [p].Cost + terrainGrid [q].Cost / 2.0f),
														terrainGrid [source].unit.movement);

		//Start point
		FlatHexPoint waypoint = source;

		//itterate through every step in the path that is in range 
		//Return the point with the highest cost
	
		foreach (var step in path)
		{
			float maxMoveCost = 0.0f;
			if (range.Keys.Contains (step))
			{
				terrainGrid [step].Color = Color.blue;
				float thisMoveCost = range [step];

					if (thisMoveCost > maxMoveCost)
					{
						waypoint = step; 
						maxMoveCost = thisMoveCost;
					}
			}
		}

		terrainGrid [waypoint].Color = Color.red;
		return waypoint;
	}

	public List<FlatHexPoint> GetEnemyPositions()
	{
		List<FlatHexPoint> result = new List<FlatHexPoint> ();

		foreach (var point in Grid)
		{
			if (terrainGrid [point].contents == CellContents.Enemy)
				result.Add (point);
	
		}
		return result;
	}
  

	public List<FlatHexPoint> GetPlayerPositions()
	{
		List<FlatHexPoint> result = new List<FlatHexPoint> ();

		foreach (var point in Grid)
		{
			if (terrainGrid [point].contents == CellContents.Player)
				result.Add (point);

		}
		return result;
	}

	public FlatHexPoint GetClosestPlayer(FlatHexPoint point)
	{
		
		var playerPositions = GetPlayerPositions ();
		float lowestCost = 10000000f;
		FlatHexPoint closestPlayer = new FlatHexPoint ();

		foreach( var playerPosition in playerPositions )
		{
			var path = GetGridPath (point, playerPosition); 
				float pathCost = 0.0f;
				foreach(var step in path)
				{
				pathCost = pathCost + terrainGrid [step].Cost;
				}
			if( pathCost < lowestCost)
			{
				lowestCost = pathCost;
				closestPlayer = playerPosition;
			}

		}
		return closestPlayer;
	}


	#endregion



	
}
