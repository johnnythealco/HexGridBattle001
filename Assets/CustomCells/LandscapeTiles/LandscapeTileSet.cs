using UnityEngine;

public class LandscapeTileSet : ScriptableObject
{
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Gamelogic/Examples/Create Landscape Tile Set")]
	public static void CreateTileSet()
	{
		var tileSet = CreateInstance<LandscapeTileSet>();
		UnityEditor.AssetDatabase.CreateAsset(tileSet, "Assets/CustomCells/LandscapeTiles.asset");
		UnityEditor.AssetDatabase.SaveAssets();
	}
#endif

	public LandscapeTileInfo[] tiles;
}
