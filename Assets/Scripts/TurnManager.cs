using UnityEngine;
using System.Collections;
using Gamelogic.Grids;
using Gamelogic;
using System.Collections.Generic;
using System.Linq;

public class TurnManager : MonoBehaviour {

	public bool PlayersTurn{ get; set;}
	public bool Moving{ get; set;}


	public TerrainBehaviour BattleManager;
	// Use this for initialization
	void Start () {

		PlayersTurn = true;
	
	}
	
	// Update is called once per frame
	void Update () {

		if(!PlayersTurn)
		{
			Dictionary<Unit, List<Vector3>> moves = new Dictionary<Unit, List<Vector3>> ();
			var enemies = BattleManager.GetEnemyPositions ();
			foreach(var enemy in enemies)
			{
				var player = BattleManager.GetClosestPlayer (enemy);
				var move = BattleManager.GetMaxMove (enemy, player);
				moves.Add (BattleManager.GetUnitAtPoint (enemy), BattleManager.GetWaypoints (enemy, BattleManager.GetMaxMove (enemy, player)));
			}
			StartCoroutine (MoveQueue (moves));
			PlayersTurn = true;

		}
	
	}


	protected IEnumerator MoveQueue (Dictionary<Unit, List<Vector3>> moveQueue)
	{
			
		var units = moveQueue.Keys.ToList ();

	
		while(!moveQueue.IsEmpty())
		{
				if(!Moving)
				{
				units.First().Move (moveQueue [units.First()]);
				moveQueue.Remove (units.First ());
				units.Remove (units.First ());
				}
			yield return null;
		}


	}
}
