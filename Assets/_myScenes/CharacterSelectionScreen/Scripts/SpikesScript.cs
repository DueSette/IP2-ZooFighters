using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesScript : MonoBehaviour
{
    public Vector3 push;
    public int damage;
    [SerializeField] float stopDuration;

    void OnCollisionEnter(Collision collision)
    {
        //Colliding with players
        if (collision.gameObject.layer == 10)
        {
            if (collision.gameObject.GetComponent<BaseCharacterBehaviour>().GetHealth() > 0)
            {
                collision.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(damage);
                collision.gameObject.GetComponent<BaseCharacterBehaviour>().SetDisablingMovementTime(stopDuration);

                if (!collision.gameObject.GetComponent<BaseCharacterBehaviour>().alive)
                {
                    StartCoroutine(CollisionSuspension(collision.collider));
                }
                else
                {
                    collision.rigidbody.velocity = Vector3.zero;
                    collision.rigidbody.AddForce(push, ForceMode.Impulse);
                    GetComponent<AudioSource>().Play();
                }
            }
            //If the character is dead make it pass through the spikes
            else
            {
                StartCoroutine(CollisionSuspension(collision.collider));
            }
        }

        if(collision.gameObject.tag == "Thrown")
        {
            collision.rigidbody.AddForce(push/4, ForceMode.VelocityChange);

            if(!collision.gameObject.GetComponent<GrenadeScript>().exploded)
                GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //When unequipped weapons touch the trap
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
