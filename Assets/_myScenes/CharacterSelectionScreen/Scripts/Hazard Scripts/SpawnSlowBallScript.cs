using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSlowBallScript : MonoBehaviour
{
    public GameObject[] ballSpawns;

    public GameObject ballPrefab;

    public int spawnChoice = 0;

    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            spawnChoice = Random.Range(1, ballSpawns.Length);

            Instantiate(ballPrefab, ballSpawns[spawnChoice - 1].transform.position, Quaternion.identity);

            active = false;
            Destroy(gameObject);
        }
    }
}
