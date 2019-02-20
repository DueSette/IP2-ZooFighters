using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Integer value to determine the jump power
    public int jumpSpeed = 10;

    //Integer value that is used to determine how long the player has been on the respawn platform
    public int activeTime = 0;

    //Booleans to determine when the player is on the game platforms, if the player has an extra jump still available and if the player is out of the screen
    public bool onFloor = true;
    public bool doubleJump = false;
    public bool belowScreen = false;

    //Boolean to determine if the player has just respawned
    public bool newlySpawned = false;

    // Update is called once per frame
    void Update()
    {
        //Game object to store the respawn object in order to access the respawn script
        GameObject respawnObject = GameObject.Find("RespawnObject");

        //Storing the respawn script in order to be utilised
        RespawnScript respawnScript = respawnObject.GetComponent<RespawnScript>();

        //Checking if the player has just respawned so they will be unable to move l
        if (!respawnScript.hasSpawned)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.right * Time.deltaTime * 10);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector3.left * Time.deltaTime * 10);
            }
        }
        

        if (transform.position.y < -10)
        {
            belowScreen = true;
        }

        if (onFloor)
        {
            if (Input.GetKeyDown(KeyCode.Space) || activeTime > 100)
            {
                GetComponent<Rigidbody>().velocity += jumpSpeed * Vector3.up;
                onFloor = false;
                doubleJump = true;
                jumpSpeed = 10;

                respawnScript.hasSpawned = false;

                activeTime = 0;

                GameObject respawnFloor = GameObject.FindGameObjectWithTag("RespawnFloor");

                if (respawnFloor != null)
                {
                    Destroy(respawnFloor.gameObject);
                }
            }
        }
        else if (doubleJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<Rigidbody>().velocity += jumpSpeed * Vector3.up;
                doubleJump = false;
            }
        }

        if (respawnScript.hasSpawned)
        {
            activeTime++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            onFloor = true;
        }

        if (collision.gameObject.tag == "RespawnFloor")
        {
            jumpSpeed = 20;
        }
    }
}
