using UnityEngine;

public class ActivateDartsPSScript : MonoBehaviour
{
	public Transform trapEffect;

	void OnTriggerEnter(Collider info)												// Activating the Darts System when the player collides with the trap
	{
		if (info.tag == "Player")
		{
			Transform effect = (Transform)Instantiate
				(trapEffect, trapEffect.position, trapEffect.rotation);
			Destroy(effect.gameObject, 3);
		}
	}
}