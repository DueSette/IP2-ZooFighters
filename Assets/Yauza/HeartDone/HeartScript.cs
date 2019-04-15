using UnityEngine;
using System.Collections;

public class HeartScript : MonoBehaviour
{
	public Transform heartEffect;                                                             // A public prefab holder through the inspector for the particle system

	#region Methods                                                                                              
	void Start()																			  // Initiating the Despawn Coroutine
	{
		StartCoroutine(Despawn());															  
	}

	IEnumerator Despawn()															     	  // A repetitive countdown do destroy the heart to keep a maximum of 1 heart
	{
		yield return new WaitForSeconds(30);
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider info)													      // On Triggering the Collider of the Heart from the player, activate the Particle system
	{
		if (info.tag == "Player")
		{
			Transform effect = (Transform)Instantiate
								(heartEffect, transform.position, transform.rotation);
			Destroy(effect.gameObject, 3);
			Destroy(gameObject);
		}
	}
#endregion
}