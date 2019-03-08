using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerScript : MonoBehaviour
{
    //called whenever something with a collider exits the invisible destroyer box
    void OnTriggerExit(Collider collider)
    {
        //if it is a player
        if (collider.gameObject.GetComponent<BaseCharacterBehaviour>() != null)
        {
            collider.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(collider.gameObject.GetComponent<BaseCharacterBehaviour>().GetHealth());
        }
        //if it is a bullet (DON'T DESTROY BULLETS PLS)
        else if (collider.tag == "Bullet")
        {
            collider.gameObject.SetActive(false);
        }

        //add stuff that shouldn't be destroyed via tag search, the rest can be destroyed

        else
        {
            Destroy(collider.gameObject);
        }
    }
}
