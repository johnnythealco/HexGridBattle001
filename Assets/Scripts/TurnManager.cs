using UnityEngine;
using System.Collections;
using Gamelogic.Grids;
using Gamelogic;

public class TurnManager : MonoBehaviour {

	public bool PlayersTurn{ get; set;}

	public TerrainBehaviour BattleManager;
	// Use this for initialization
	void Start () {

		PlayersTurn = true;
	
	}
	
	// Update is called once per frame
	void Update () {

		if(!PlayersTurn)
		{
			var enemies = BattleManager.GetEnemyPositions ();
			foreach(var enemy in enemies)
			{
				var player = BattleManager.GetClosestPlayer (enemy);
				BattleManager.AdvanceOnTarget (enemy, player);
			}
			PlayersTurn = true;

		}
	
	}
}
