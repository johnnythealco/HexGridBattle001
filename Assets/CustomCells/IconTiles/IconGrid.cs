namespace Gamelogic.Grids.Examples
{
	public class IconGrid : GridBehaviour<FlatHexPoint>
	{
		public IconTileSet tiles;

		public override void InitGrid()
		{
			foreach (var point in Grid)
			{
				var cell = (LandscapeCell) Grid[point];
				cell.foreground.sprite = tiles.sprites.RandomItem();
			}
		}
	}
}