using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class IconTileSet : ScriptableObject
	{
#if UNITY_EDITOR
		[UnityEditor.MenuItem("Gamelogic/Examples/Create Icon Tile Set")]
		public static void CreateTileSet()
		{
			var tileSet = CreateInstance<IconTileSet>();
			UnityEditor.AssetDatabase.CreateAsset(tileSet, "Assets/LandscapeTiles/IconTiles.asset");
			UnityEditor.AssetDatabase.SaveAssets();
		}
#endif

		public Sprite[] sprites;
		public Color[] colors = ExampleUtils.Colors;
	}
}