using UnityEngine;

public class MovingWallScript : MonoBehaviour
{
	#region Variables
	public float forceMultiplier = 200;                                                          //the movement speed of the wall
    private Rigidbody rb;                                                                        //just rigidbody call
	#endregion
	void Start()																				 //moving the wall method
    {
        rb = GetComponent<Rigidbody>();                                                          //instantiating the rigidbody
        rb.velocity = transform.forward * Time.deltaTime * forceMultiplier;                      //adding the velocity to the object

        Destroy(gameObject, 10);                                                                 //destroying object after "xf" seconds
    }
}