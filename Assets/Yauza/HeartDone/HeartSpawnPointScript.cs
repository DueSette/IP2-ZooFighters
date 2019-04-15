using UnityEngine;

public class HeartSpawnPointScript : MonoBehaviour
{
	#region Variables														
																												 //Public Variables accessable in the inspector
	[Header("Spawn Object")]
	public GameObject HeartPrefab;
	[Header("Timing Variables")]
	public float initialSpawnTime;                                                                          
	public float spawnDelay;
	#endregion

	#region Methods
	public void SpawnHeart()																					 //This method will spawn the prefab, with the GameObject position and rotation
	{
		Instantiate(HeartPrefab, transform.position, transform.rotation);
	}

	void Start()																								 //The repetitive spawning of the heart, when, how often
	{
		InvokeRepeating("SpawnHeart", initialSpawnTime, spawnDelay);
	}
	#endregion
}