using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesScript : MonoBehaviour
{
    [SerializeField]
    Vector3 push;

    void OnCollisionEnter(Collision collision)
    {
        //Colliding with players
        if (collision.gameObject.layer == 10)
        {
            if (collision.gameObject.GetComponent<BaseCharacterBehaviour>().GetHealth() > 0)
            {
                collision.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(13);

                if (collision.gameObject.GetComponent<BaseCharacterBehaviour>().GetHealth() <= 0)
                {
                    StartCoroutine(CollisionSuspension(collision.collider));
                }
                else
                {
                    collision.rigidbody.AddForce(push, ForceMode.Impulse);
                    GetComponent<AudioSource>().Play();
                }
            }
            else
            {
                StartCoroutine(CollisionSuspension(collision.collider));
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            if(other.gameObject.GetComponent<MeleeWeaponScript>() != null && !other.gameObject.GetComponent<MeleeWeaponScript>().isEquipped)           
                Destroy(other.gameObject);
            
            else if (other.gameObject.GetComponent<RangedWeaponScript>() != null && !other.gameObject.GetComponent<RangedWeaponScript>().isEquipped)
                    Destroy(other.gameObject);
        }
    }

    private IEnumerator CollisionSuspension(Collider collider)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), collider);
        yield return new WaitForSeconds(1);
        Physics.IgnoreCollision(GetComponent<Collider>(), collider, false);
    }
}
