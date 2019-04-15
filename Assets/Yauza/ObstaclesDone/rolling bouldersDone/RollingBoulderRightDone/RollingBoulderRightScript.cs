using UnityEngine; 

public class RollingBoulderRightScript : MonoBehaviour
{
	#region Private Variables
	private Rigidbody rb;
	private float despawntime;
	private float forcemult;
	#endregion

	private void Awake()															// Getting variables and preparing the despawn
	{
		rb = GetComponent<Rigidbody>();
		despawntime = RollingBoulderRightSpawnPointScript.despawnTime;
		forcemult = RollingBoulderRightSpawnPointScript.forceMult;
		Destroy(gameObject, despawntime);
	}

	void FixedUpdate()																// Moving in a direction 
	{
		Vector3 movement = new Vector3(1, 0, 0.0f);

		rb.AddForce(movement * forcemult * Time.deltaTime);
	}
}