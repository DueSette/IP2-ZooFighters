using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "SlowedFloor")
        {
            timer += Time.deltaTime;
        }

        if (timer >= 10)
        {
            gameObject.tag = "Floor";
            timer = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (gameObject.tag == "Floor")
            {
                collision.gameObject.GetComponent<BaseCharacterBehaviour>().movSpeed = collision.gameObject.GetComponent<BaseCharacterBehaviour>().originalMoveSpeed;
            }

            if (gameObject.tag == "SlowedFloor")
            {
                collision.gameObject.GetComponent<BaseCharacterBehaviour>().movSpeed = collision.gameObject.GetComponent<BaseCharacterBehaviour>().slowedMoveSpeed;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            RemainSlowed(collision.gameObject);
        }
    }

    public IEnumerator RemainSlowed(GameObject collide)
    {
        yield return new WaitForSeconds(1);
        collide.GetComponent<BaseCharacterBehaviour>().movSpeed = collide.GetComponent<BaseCharacterBehaviour>().originalMoveSpeed;
    }
}
