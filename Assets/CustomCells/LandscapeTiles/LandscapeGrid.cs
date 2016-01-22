using Gamelogic;
using Gamelogic.Grids;
using UnityEngine;

public class LandscapeGrid : GridBehaviour<FlatHexPoint>
{
	public LandscapeTileSet tiles;

	public override void InitGrid()
	{
		var subset = ChooseSubset(7);

		foreach (var point in Grid)
		{
			int randomIndex = subset.RandomItem();

			var cell = (LandscapeCell) Grid[point];

			cell.Color = tiles.tiles[randomIndex].color;
			cell.foreground.sprite = tiles.tiles[randomIndex].image;
		}
	}

	private int[] ChooseSubset(int count)
	{
		int[] subset = new int[count];

		for (int i = 0; i < subset.Length; i++)
		{
			subset[i] = Random.Range(0, tiles.tiles.Length);
		}

		return subset;
	}
}
