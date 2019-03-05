using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawn : MonoBehaviour
{
    public GameObject weaponToSpawn;
    public float time = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn(weaponToSpawn));
    }

   private IEnumerator Spawn(GameObject weap)
    {
        yield return new WaitForSeconds(time);
        Instantiate(weap, transform.position, Quaternion.Euler(0, 90, 0));
        StartCoroutine(Spawn(weap));
    }
}
