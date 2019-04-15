using UnityEngine;

public class SpawningMovingWallScript : MonoBehaviour
{
	#region Public Variables
	public GameObject Spawnee;                                                      //public window to add the object we want ot spawn
	public bool stopSpawning = false;                                               //adding
	public float spawnTime;                                                         //          some
	public float spawnDelay;                                                        //                    variables
	#endregion
	#region Methods
	void OnEnable()
	{
		InvokeRepeating("SpawnMovingWall", spawnTime, spawnDelay);                  //this is basically something like an update method but in the start
	}

	public void SpawnMovingWall()                                                   //this is our simple spawning script
	{
		Instantiate(Spawnee, transform.position, transform.rotation);
		if (stopSpawning)
		{
			CancelInvoke("SpawnMovingWall");
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))                                             //Stop the spawning if we press the 'O' key/character 
		{
			if (stopSpawning == false)
			{
				stopSpawning = true;
			}
		}
	}
	#endregion
}