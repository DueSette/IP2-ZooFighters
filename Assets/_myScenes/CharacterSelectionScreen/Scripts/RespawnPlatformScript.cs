using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlatformScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(DestroyPlatform());
    }

    public IEnumerator DestroyPlatform()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        Destroy(gameObject);
    }
}
