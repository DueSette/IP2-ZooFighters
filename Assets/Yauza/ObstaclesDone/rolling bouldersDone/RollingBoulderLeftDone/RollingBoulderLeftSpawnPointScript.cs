using UnityEngine;

public class RollingBoulderLeftSpawnPointScript : MonoBehaviour
{
	#region Public Variables
	public GameObject Spawnee;                                                              //public window to add the object we want ot spawn
	public bool stopSpawning = false;                                                       //adding
	[Header("Time Accessories")]
	public float spawnTime;                                                                 //          some
	public float spawnDelay;                                                                //                    variables
	public static float despawnTime = 10;
	public static float forceMult = 5000;
	#endregion
	#region Methods
	void OnEnable()
	{
		InvokeRepeating("SpawnRollingBoulderLeft", spawnTime, spawnDelay);                  //this is basically something like an update method but in the start
	}

	public void SpawnRollingBoulderLeft()                                                   //this is our simple spawning script
	{
		Instantiate(Spawnee, transform.position, transform.rotation);

		if (stopSpawning)
		{
			CancelInvoke("SpawnRollingBoulderLeft");
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))                                                    //Stop the spawning if we press the 'O' key/character 
		{
			if (stopSpawning == false)
			{
				stopSpawning = true;
			}
		}
	}
	#endregion
} 