using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlatformScript : MonoBehaviour
{
    public List<GameObject> collidedPlayer;
    public float rotSpeed;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(DestroyPlatform());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
    }

    public IEnumerator DestroyPlatform()
    {
        yield return new WaitForSeconds(3.0f);
        collidedPlayer[0].GetComponent<BaseCharacterBehaviour>().respawned = false;
        gameObject.SetActive(false);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == collidedPlayer[0])
        {
            collidedPlayer[0].GetComponent<BaseCharacterBehaviour>().respawned = false;
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collidedPlayer.Add(collision.gameObject);   
        }      
        collidedPlayer[0].GetComponent<BaseCharacterBehaviour>().respawned = true;
    }
}
