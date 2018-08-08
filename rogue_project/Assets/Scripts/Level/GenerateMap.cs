using UnityEngine;
using System.Collections;

public class GenerateMap : MonoBehaviour
{

	[Header ("Game Objects")]
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] outerWallTiles;
	public GameObject[] treasure;
	public GameObject[] enemies;
	public GameObject player;

	 
	[Header ("Generation Settings")]
	public int width = 64;
	public int height = 64;

	public float wallChance = 0.45f;

	public int birthLimit = 4;
	public int deathLimit = 3;

	public int cleanCycles = 2;

	[Header ("Treasure Settings")]
	public bool enableTreasure = true;
	public int treasureChance = 6;

	[Header ("AI Settings")]
	public GameObject aiGrid;


	private bool[,] genMap;
	private GameObject boardHolder;

	bool debugNodeChange;
	bool finishedGen = false;
	bool finishedAIGrid = false;


	void Start ()
	{
		boardHolder = new GameObject ("BoardHolder");
	
		generateMap ();

	}

	void Update ()
	{
		if (finishedGen && !finishedAIGrid) {
			aiGrid.GetComponent<AstarPath> ().Scan ();
			finishedAIGrid = true;
		}
			
	}

	private void generateMap ()
	{
		genMap = new bool[width, height];
		genMap = initializeMap ();
	

		for (int i = 0; i < cleanCycles; i++) {
			genMap = cleanUp (genMap);
		}

		InstantiateTiles ();
		InstantiateOuterWalls ();
		spawnPlayer ();
		spawnEnemy (1);

		if (enableTreasure)
			placeTreasure (genMap);

		finishedGen = true;
	}

	private bool[,] initializeMap ()
	{

		bool[,] map = new bool[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (random () < wallChance) {
					map [x, y] = true;
				}
			}
		}
		return map;
	}

	public void placeTreasure (bool[,] map)
	{

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (!map [x, y]) {
					int nbs = checkSurrounding (map, x, y);
					if (nbs >= treasureChance) {
						InstantiateTreasure (x, y);
					}
				}
			}
		}
	}

	void InstantiateTreasure (int x, int y)
	{
		InstantiateFromArray (treasure, x, y);
	}

	void InstantiateTiles ()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				InstantiateFromArray (floorTiles, x, y);

				// If the tile type is Wall...
				if (genMap [x, y] == true) {
					InstantiateFromArray (wallTiles, x, y);
				}
			}
		}
	}

	void InstantiateOuterWalls ()
	{

		float leftEdgeX = -1f;
		float rightEdgeX = height + 0f;
		float bottomEdgeY = -1f;
		float topEdgeY = width + 0f;

		InstantiateVerticalOuterWall (leftEdgeX, bottomEdgeY, topEdgeY);
		InstantiateVerticalOuterWall (rightEdgeX, bottomEdgeY, topEdgeY);

		InstantiateHorizontalOuterWall (leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
		InstantiateHorizontalOuterWall (leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
	}


	void InstantiateVerticalOuterWall (float xCoord, float startingY, float endingY)
	{

		float currentY = startingY;

		while (currentY <= endingY) {
			InstantiateFromArray (outerWallTiles, xCoord, currentY);
			currentY++;
		}
	}


	void InstantiateHorizontalOuterWall (float startingX, float endingX, float yCoord)
	{
		
		float currentX = startingX;

		while (currentX <= endingX) {
			InstantiateFromArray (outerWallTiles, currentX, yCoord);
			currentX++;
		}
	}

	void InstantiateFromArray (GameObject[] prefabs, float xCoord, float yCoord)
	{
		
		int randomIndex = Random.Range (0, prefabs.Length);

		Vector3 position = new Vector3 (xCoord, yCoord, 0f);
		GameObject tileInstance = Instantiate (prefabs [randomIndex], position, Quaternion.identity) as GameObject;

		tileInstance.transform.parent = boardHolder.transform;
	}

	private bool[,] cleanUp (bool[,] oldMap)
	{
		bool[,] newMap = new bool[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int nbs = checkSurrounding (oldMap, x, y);

				if (oldMap [x, y]) {
					if (nbs < deathLimit) {
						newMap [x, y] = false;
					} else {
						newMap [x, y] = true;
					}
				} else {
					if (nbs > birthLimit) {
						newMap [x, y] = true;
					} else {
						newMap [x, y] = false;
					}
				}
			}
		}
		return newMap;
	}

	private int checkSurrounding (bool[,] map, int x, int y)
	{

		int count = 0;

		for (int i = -1; i < 2; i++) {
			for (int j = -1; j < 2; j++) {

				int neighborX = x + i;
				int neighborY = y + j;

				if (i == 0 && j == 0) {
					
				} else if (neighborX < 0 || neighborY < 0 || neighborX >= width || neighborY >= height) {
					count = count + 1;
				} else if (map [neighborX, neighborY]) {
					count = count + 1;
				}
			}
		}

		return count;
	}

	float random ()
	{
		return Random.Range (0f, 1f);
	}

	public void spawnPlayer ()
	{
		bool hasSpawned = false;
		
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (!genMap [x, y]) {
					int nbs = checkSurrounding (genMap, x, y);
					if (!hasSpawned) {
						if (nbs >= 3) {
							Instantiate (player, new Vector2 (x, y), Quaternion.identity);
							hasSpawned = true;
						}
					}
				}
			}
		}
	}

	public void spawnEnemy (int amt)
	{
		int curAmt = 0;

		while(curAmt < amt){

			int ranX = Random.Range (0, width);
			int ranY = Random.Range (0, height);

			if (!genMap [ranX, ranY]) {
						
					InstantiateFromArray (enemies, ranX, ranY);
					curAmt++;
				
			}
		}
	}


}
