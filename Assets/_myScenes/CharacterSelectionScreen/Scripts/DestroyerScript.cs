using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerScript : MonoBehaviour
{
    void OnTriggerExit(Collider collider)
    {
        collider.gameObject.SetActive(false);
        
        //SHOULD DO SOMETHING IF COLLIDING WITH PLAYER RESETTING HP AND REMOVING ONE LIFE, THEN CALLING THE RESPAWN FUNCTION
    }
}
