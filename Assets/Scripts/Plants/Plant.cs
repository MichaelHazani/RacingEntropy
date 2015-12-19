using UnityEngine;
using System.Collections;

public class Plant
{
	public Tile tile;
	public GameObject instance;
	public TileType.growFunction function;
	public int type;
	
	public Plant(TileType.growFunction newFunction, GameObject newInstance, Tile newTile, int newType)
	{
		instance = newInstance;
		function = newFunction;
		tile = newTile;
		type = newType;
	}

	public void Kill()
	{
		GameObject.DestroyImmediate (instance);
	}
}
