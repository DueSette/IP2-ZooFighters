using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerScript : MonoBehaviour
{
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent<BaseCharacterBehaviour>() != null)
        {
            collider.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(collider.gameObject.GetComponent<BaseCharacterBehaviour>().GetHealth());
        }
        else
        {
            Destroy(collider.gameObject);
        }
    }
}
