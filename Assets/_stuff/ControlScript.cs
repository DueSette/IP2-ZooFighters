using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour
{
    private Material currMat;
    public Material red;
    private Vector3 thisPos;


    // Start is called before the first frame update
    void Start()
    {
        currMat = gameObject.GetComponent<Renderer>().material;
        thisPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("AButton"))
        {
            print("heu");
        }

        thisPos = thisPos + new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
    }
}
