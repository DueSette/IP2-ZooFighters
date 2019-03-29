using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    bool working = true;
    public int damage;
    public float radius;
    public Vector3 explosionPower;
    [HideInInspector]
    public float direction;
    [HideInInspector]
    public Collider shooterCollider;
    private LayerMask explosionLayerMask;
    public Vector3 grenadeSpeed;
    public Vector3 grenadeTorque;
    public delegate void Exploding(AudioClip clip);
    public static event Exploding OnExplode;

    public AudioClip explosionSound;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        explosionLayerMask = LayerMask.GetMask("Player");
    }
    void OnEnable()
    {
        working = true;
        rb.velocity = Vector3.zero;
    }
    public void AfterEnable()
    {
        rb.AddForce(new Vector3(grenadeSpeed.x * direction, grenadeSpeed.y, 0), ForceMode.VelocityChange);
        rb.AddTorque(Vector3.forward * 270 * Mathf.Sign(transform.rotation.y));
        Physics.IgnoreCollision(shooterCollider, GetComponent<Collider>());
    }

    void OnCollisionEnter(Collision coll)
    {

        switch (coll.gameObject.layer)
        {
            //Ground layer
            case 9:
                SetFuse(0.9f);
                break;
            //Player layer
            case 10:
                Explode();
                break;
            //Traps layer
            case 12:
                SetFuse(0.4f);
                break;
            default:
                SetFuse(1);
                break;
        }
    }

    void Explode()
    {
        if (working)
        {
            Vector3 pos = transform.position;
            Collider[] caughtInExplosion = Physics.OverlapSphere(pos, radius, explosionLayerMask);

            foreach (Collider coll in caughtInExplosion)
            {
                //Vector3 normal = (coll.gameObject.transform.position - pos).normalized;
                //coll.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(normal.x * explosionPower.x, normal.y * explosionPower.y, 0), pos, ForceMode.Impulse);

                coll.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionPower.x * 2, pos, radius * 2, 2.5f, ForceMode.Impulse);
                coll.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(damage);
                coll.gameObject.GetComponent<BaseCharacterBehaviour>().SetDisablingMovementTime(0.5f);
            }
            OnExplode(explosionSound);
            StartCoroutine(CameraScript.instance.SetShakeTime(0.5f, 6, 1.5f));
            StartCoroutine(OnImpact());
        }
        working = false;
    }

    void SetFuse(float time)
    {
        if(working)
            Invoke("Explode", time);
    }

    public IEnumerator OnImpact()
    {
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.6f);
        GetComponent<MeshRenderer>().enabled = true;
        gameObject.SetActive(false);
        Physics.IgnoreCollision(shooterCollider, GetComponent<Collider>(), false); //reset collision immunity
        //release particle
        //do additional stuff if need be
    }
}
