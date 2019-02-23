using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningMovingWallScript : MonoBehaviour
{
    public GameObject Spawnee;                                                      //public window to add the object we want ot spawn
    public bool stopSpawning = false;                                               //adding
    public float spawnTime;                                                         //          some
    public float spawnDelay;                                                        //                    variables

    void Start()
    {
        InvokeRepeating("SpawnMovingWall", spawnTime, spawnDelay);                  //this is basically something like an update method but in the start
    }

    public void SpawnMovingWall()                                                   //this is our simple spawning script
    {
        Instantiate(Spawnee, transform.position, transform.rotation); 
            if(stopSpawning)
        {
            CancelInvoke("SpawnMovingWall"); 
        }
    }

    void Update()                                                                   //Stop the spawning if we press the 'O' key/character 
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            stopSpawning = true; 
        }
    }
}
