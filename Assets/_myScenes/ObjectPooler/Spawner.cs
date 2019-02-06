using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public ObjectPooler pooler;

    private void Start()
    {
        pooler = ObjectPooler.Instance;
    }

    private int x = 0;

    private void FixedUpdate()
    {
        int x = Random.Range(1, 4);

        switch(x)
        {
            case 1:
                pooler.SpawnFromPool(pooler.pools[x - 1].tag, transform.position, Quaternion.identity);
                break;
            case 2:
                pooler.SpawnFromPool(pooler.pools[x - 1].tag, transform.position, Quaternion.identity);
                break;
            case 3:
                pooler.SpawnFromPool(pooler.pools[x - 1].tag, transform.position, Quaternion.identity);
                break;
            default:
                break;
        }
    }
}
