using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSprint : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnActivate()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(5, 20, 0));
    }

 
}
