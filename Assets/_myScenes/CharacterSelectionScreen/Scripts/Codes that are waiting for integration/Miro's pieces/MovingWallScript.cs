using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWallScript : MonoBehaviour
{
    public float forceMultiplier = 200;                                                          //the movement speed of the wall
    private Rigidbody rb;                                                                        //just rigidbody call

    void Start()
    {
        rb = GetComponent<Rigidbody>();                                                          //instantiating the rigidbody
        rb.velocity = transform.forward * Time.deltaTime * forceMultiplier;                      //adding the velocity to the object

        Destroy(gameObject, 5f);                                                                 //destroying object after "xf" seconds
    }
}
