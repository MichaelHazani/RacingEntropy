using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TO DO: Implement disaster cascades.

public class CascadeManager {

	// So for each cascading element, I'm going to need a whole array of affected tiles because I don't want to hit the same tile twice.
	// So I'll need to check if the tile in question is already on the affected list.
	// If !Contains(currentTile) Add(currentTile)

	// Then at the end, apply the element to every affected tile.
	// Foreach (Tile current in xxxx)

	// So the only differece is the way that the cascade spreads, which is based on the element.

	public TileManager manager;

	private int width;
	private int height;

	private int[] elevations = new int[]{1,0,1,0,3,1,2,1};
	private int[] telements = new int[]{3,2,2,2,0,1,0,1};

	private List<Tile> affectedTiles;
	private Queue<Tile> toDo;

	public CascadeManager (TileManager m) {
		manager = m;
	}

	public void OnElement (Tile currentTile, int currentElement) {

		// I can't guarentee that the map size will remain the same from call to call on this function.
		width = manager.getTile.GetLength(0);
		height = manager.getTile.GetLength(1);
		
		// So, first things first, I need to know the coordinate of the current tile.  And that's not currently stored.
		// I'd rather that each tile store this on creation, but this will work for now.

		// So now I need a bunch of cases for each element, and then I need a loop function for each of them
		affectedTiles = new List<Tile>();
		toDo = new Queue<Tile>();

		// Don't want to hit ourselves with air.
		if (currentElement != (int)TileType.element.AIR)
			affectedTiles.Add(currentTile);
		toDo.Enqueue(currentTile);
		// Now we can assume in each iteration that we've already added the tile on toDo.
		// (reasonable, since it'll have been done by the prior iteration.)

		// loops until toDo is done.  This isn't dangerous at all...
		while (toDo.Count > 0) 
		{
			switch (currentElement) 
			{
				case (int)TileType.element.WATER:
					OnWater(toDo.Dequeue());
					break;
				case (int)TileType.element.FIRE:
					if(currentTile.flammability > 0)manager.AddFire(currentTile.x,currentTile.y);
					OnFire(toDo.Dequeue());
					break;
				case (int)TileType.element.EARTH:
					OnEarth(toDo.Dequeue());
					break;
				case (int)TileType.element.AIR:
					OnAir(toDo.Dequeue());
					break;
			}
		}

		// If air, here we need to change the current element according to the tile we're hitting with air.
		if (currentElement == (int)TileType.element.AIR)
			currentElement = telements[(int)currentTile.type];

		// Then apply the element effect to every tile in affectedTiles.  I don't currently know what to call for that.
		foreach (Tile t in affectedTiles) {
			t.Change(currentElement);
			manager.Change(manager.objectFromTile[t],t);
		}
	}

	private void OnWater (Tile currentTile) {
		// Four checks, comparing elevation of each adjacent tile with elevation of this tile.
		// Can short-circut if this tile is already elevation 1 or less.
		if (elevations[(int)currentTile.type] == 0)
			return;

		int coordX = currentTile.x;
		int coordY = currentTile.y;

		Tile currentCheck;

		if (coordY != 0) {
			currentCheck = manager.getTile[coordX, coordY-1];
			if (!affectedTiles.Contains(currentCheck) && (elevations[currentCheck.type] < elevations[currentTile.type])) {
				affectedTiles.Add(currentCheck);
				toDo.Enqueue(currentCheck);
			}
		}

		if (coordX != 0) {
			currentCheck = manager.getTile[coordX-1, coordY];
			if (!affectedTiles.Contains(currentCheck) && (elevations[currentCheck.type] < elevations[currentTile.type])) {
				affectedTiles.Add(currentCheck);
				toDo.Enqueue(currentCheck);
			}
		}

		if (coordX != width-1) { 
			currentCheck = manager.getTile[coordX+1, coordY];
			if (!affectedTiles.Contains(currentCheck) && (elevations[currentCheck.type] < elevations[currentTile.type])) {
				affectedTiles.Add(currentCheck);
				toDo.Enqueue(currentCheck);
			}
		}

		if (coordY != height-1) { 
			currentCheck = manager.getTile[coordX, coordY+1];
			if (!affectedTiles.Contains(currentCheck) && (elevations[currentCheck.type] < elevations[currentTile.type])) {
				affectedTiles.Add(currentCheck);
				toDo.Enqueue(currentCheck);
			}
		}
	}

	private void OnFire (Tile currentTile) {
		// Four checks to see if the selected tile is flammable.
		if (currentTile.type != (int)TileType.tile.FOREST && currentTile.type != (int)TileType.tile.PLAIN)
			return;
		
		
		int coordX = currentTile.x;
		int coordY = currentTile.y;
		
		// Four checks
		Tile currentCheck = manager.getTile[coordX,coordY];
		manager.AddFire(currentCheck.x,currentCheck.y);
	}

	private void OnEarth (Tile currentTile) {
		// Make sure we're a crag or mountain
		if (currentTile.type != (int)TileType.tile.MOUNTAIN && currentTile.type != (int)TileType.tile.CRAGS)
			return;


		int coordX = currentTile.x;
		int coordY = currentTile.y;

		// Four checks
		Tile currentCheck;

		if (coordY != 0) {
			currentCheck = manager.getTile[coordX, coordY-1];
			if (elevations[currentCheck.type] < elevations[currentTile.type]) {
				affectedTiles.Add(currentCheck);
			}
		}

		if (coordX != 0) {
			currentCheck = manager.getTile[coordX-1, coordY];
			if (elevations[currentCheck.type] < elevations[currentTile.type]) {
				affectedTiles.Add(currentCheck);
			}
		}

		if (coordX != width-1) { 
			currentCheck = manager.getTile[coordX+1, coordY];
			if (elevations[currentCheck.type] < elevations[currentTile.type]) {
				affectedTiles.Add(currentCheck);
			}
		}

		if (coordY != height-1) { 
			currentCheck = manager.getTile[coordX, coordY+1];
			if (elevations[currentCheck.type] < elevations[currentTile.type]) {
				affectedTiles.Add(currentCheck);
			}
		}

	}

	private void OnAir (Tile currentTile) {
		// Four checks
		int coordX = currentTile.x;
		int coordY = currentTile.y;

		// Four checks
		Tile currentCheck;

		if (coordY != 0) {
			currentCheck = manager.getTile[coordX, coordY-1];
			if (currentCheck.type != 6 && currentCheck.type != 4) {
				affectedTiles.Add(currentCheck);
			}
		}

		if (coordX != 0) {
			currentCheck = manager.getTile[coordX-1, coordY];
			if (currentCheck.type != 6 && currentCheck.type != 4) {
				affectedTiles.Add(currentCheck);
			}
		}

		if (coordX != width-1) { 
			currentCheck = manager.getTile[coordX+1, coordY];
			if (currentCheck.type != 6 && currentCheck.type != 4) {
				affectedTiles.Add(currentCheck);
			}
		}

		if (coordY != height-1) { 
			currentCheck = manager.getTile[coordX, coordY+1];
			if (currentCheck.type != 6 && currentCheck.type != 4) {
				affectedTiles.Add(currentCheck);
			}
		}
		// Currently air doesn't do anything anyway, so... take a moment.
	}

	// Spread air, and if it's spread to an existing plains or desert then spread the storm further.
	public void OnStorm (Tile currentTile) {

	}

	// Spread fire.
	public void OnEruption (Tile currentTile) {

	}

	// This puppy should cascade, complete with toDo, until it finds an appropriate tile to spread water to.
	public void OnFlood (Tile currentTile) {

	}
}