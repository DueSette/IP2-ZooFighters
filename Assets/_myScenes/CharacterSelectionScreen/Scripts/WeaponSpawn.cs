using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawn : MonoBehaviour
{
    public GameObject[] weaponsToSpawn = new GameObject[1];
    public float timeBetweenSpawns = 3;
    private Vector3 transf;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(Spawn(weaponsToSpawn[Random.Range(0, weaponsToSpawn.Length)]));
    }

    private void Update()
    {
        
    }

   private IEnumerator Spawn(GameObject weap)
    {
        yield return new WaitForSeconds(timeBetweenSpawns);
        transf = new Vector3(Random.Range(-57, 57), transform.position.y, transform.position.z);
        timeBetweenSpawns = Random.Range(2.5f, 8.5f);
        Instantiate(weap, transf, Quaternion.Euler(0, 90, 0));
        StartCoroutine(Spawn(weaponsToSpawn[Random.Range(0, weaponsToSpawn.Length)]));
    }
}
