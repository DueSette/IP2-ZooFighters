using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTriggerScript : MonoBehaviour
{
    float timer = 0;
    bool canDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && canDamage)
        {
            StartCoroutine(DamagePlayer(other.gameObject));
        }
    }

    IEnumerator DamagePlayer(GameObject player)
    {
        player.GetComponent<BaseCharacterBehaviour>().TakeDamage(10);
        canDamage = false;
        yield return new WaitForSeconds(2);
        canDamage = true;
    }
}
