using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    //Creating the variables for the prefabs
    public GameObject playerPrefab;
    public GameObject respawnPlatformPrefab;

    //Creating and storing the x-positions for the respawn platforms to be randomly chosen
    public int[] platformPositions = { -22, -15, 6, 22 };

    //Boolean to store when the player has just respawned
    public bool hasSpawned = false;

    //Gameobject used to refernece the player script
    GameObject player;

    // Update is called once per frame
    void Update()
    {
        //Finding the player within the game
        player = GameObject.FindGameObjectWithTag("Player");

        //Collecting the player script from the object found
        PlayerScript playerScript = player.GetComponent<PlayerScript>();

        //Checks whether the player has fallen off screen
        if (playerScript.belowScreen)
        {
            //Deletes the player object
            player.gameObject.SetActive(false);

            //Variable used to store the randomly generated x-position of the respawn platform and the player's respawn
            int randomPos;

            //Collects a randomly generated number that will be used for the platform and player's x-position on spawning
            randomPos = Random.Range(0, 3);

            //Instantiating the new player and respawn platform
            Instantiate(respawnPlatformPrefab, new Vector3(platformPositions[randomPos], 6, 0), Quaternion.identity);
            player.gameObject.transform.position = new Vector3(platformPositions[randomPos], 10, 0);
            player.gameObject.SetActive(true);
            

            //Sets the boolean to true to say that the player has now respawned. This is utilized in the player script
            hasSpawned = true;

            playerScript.belowScreen = false;
        }
    }
}
