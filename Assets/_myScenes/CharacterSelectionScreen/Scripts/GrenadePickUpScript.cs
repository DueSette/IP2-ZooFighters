using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePickUpScript : MonoBehaviour
{
    Rigidbody rb;
    Vector3 velo;
    bool landed = false;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        velo = rb.velocity;
        if (velo.y < -45)
        {
            rb.AddForce(new Vector3(0, 2.7f, 0), ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if(landed)
            transform.Rotate(Vector3.up * 30 * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 9 && collider.gameObject.transform.position.y < transform.position.y)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            landed = true;
            transform.position = new Vector3(transform.position.x, transform.position.y + GetComponent<Collider>().bounds.extents.y, transform.position.z);
        }
    }
}
