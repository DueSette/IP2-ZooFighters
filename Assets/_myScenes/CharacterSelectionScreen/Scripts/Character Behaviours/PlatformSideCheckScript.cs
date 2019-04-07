using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSideCheckScript : MonoBehaviour
{
    public float touchTime = 0;
    public bool touching = false;
    public float threshold;

    void Start()
    {
        gameObject.SetActive(true);
    }

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Floor")
        {
            touching = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.tag == "Floor")
        {
            touching = false;
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if(touching)
        {
            touchTime += Time.deltaTime;
            if (touchTime > threshold)
                RefreshJump();
        }
        else
        {
            touchTime = 0;
        }   
    }

    void RefreshJump()
    {
        GetComponentInParent<BaseCharacterBehaviour>().canExtraJump = true;
        touchTime = 0;
    }
}
