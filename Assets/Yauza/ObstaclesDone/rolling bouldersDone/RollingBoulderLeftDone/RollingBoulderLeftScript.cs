using UnityEngine; 

public class RollingBoulderLeftScript : MonoBehaviour
{
	#region Variables
	private float forcemult;
	private Rigidbody rb;
	private float despawntime;
	#endregion
	#region Methods
	private void Awake()																		   // Getting variables and setting despawnTime
    {
		rb = GetComponent<Rigidbody>();
		despawntime = RollingBoulderLeftSpawnPointScript.despawnTime;
		forcemult = RollingBoulderLeftSpawnPointScript.forceMult; 											  
		Destroy(gameObject, despawntime); 
    }
			
    void FixedUpdate()																			   // Adding movement in a direction
    {
        Vector3 movement = new Vector3(-1, 0, 0.0f);
        rb.AddForce(movement * forcemult * Time.deltaTime);
    }
	#endregion
}