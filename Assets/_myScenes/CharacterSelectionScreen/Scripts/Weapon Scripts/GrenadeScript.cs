using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    #region Data
    bool ticking = false;
    [HideInInspector]
    public bool exploded = false;

    public float fuseSeconds;
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
    private float timeInExistence;

    public delegate void Exploding(AudioClip clip);
    public static event Exploding OnExplode;

    public GameObject bombMeshObject;
    public GameObject explosionParticle;
    public AudioClip explosionSound;
    Rigidbody rb;
    private float startFuseLength;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        explosionLayerMask = LayerMask.GetMask("Player");
        startFuseLength = fuseSeconds;
        grenadeSpeed.x = Mathf.Clamp(grenadeSpeed.x, 25, 80);
    }

    void OnEnable()
    {
        timeInExistence = 0;
        ticking = false;
        exploded = false;
        rb.velocity = Vector3.zero;
    }

    public void AfterEnable()
    {
        rb.AddForce(new Vector3(grenadeSpeed.x * direction, grenadeSpeed.y, 0), ForceMode.VelocityChange);
        rb.AddTorque(Vector3.forward * 270 * Mathf.Sign(transform.rotation.y));
        Physics.IgnoreCollision(shooterCollider, GetComponent<Collider>());
    }

    void Update()
    {
        if (timeInExistence < 7)
            timeInExistence += Time.deltaTime;
        else
            gameObject.SetActive(false);

        if(ticking)
        {
            fuseSeconds -= Time.deltaTime;
        }
        if (fuseSeconds <= 0)
            Explode();
    }

    void OnCollisionEnter(Collision coll)
    {
        if (!ticking)
        {
            switch (coll.gameObject.layer)
            {
                //Ground layer
                case 9:
                    ticking = true;
                    break;
                //Player layer
                case 10:
                    ticking = true;
                    if (!coll.gameObject.GetComponent<BaseCharacterBehaviour>().respawned)
                    {
                        coll.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(5 + (20 * ((int)grenadeSpeed.x / 80)));
                    }
                    break;
                //Traps layer
                case 12:
                    ticking = true;
                    break;
                default:
                    ticking = true;
                    break;
            }
        }
    }

    void Explode()
    {
        if (!exploded)
        {
            Vector3 pos = transform.position;
            Collider[] caughtInExplosion = Physics.OverlapSphere(pos, radius, explosionLayerMask);

            foreach (Collider coll in caughtInExplosion)
            {
                coll.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionPower.x * 2, pos, radius * 2, 2.5f, ForceMode.Impulse);
                if (!coll.gameObject.GetComponent<BaseCharacterBehaviour>().respawned)
                {
                    coll.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(damage);
                    coll.gameObject.GetComponent<BaseCharacterBehaviour>().SetDisablingMovementTime(0.5f);
                }
            }
            OnExplode(explosionSound);
            StartCoroutine(ExplosionParticle());
            StartCoroutine(CameraScript.instance.SetShakeTime(0.5f, 6, 1.5f));
            StartCoroutine(AfterExplosion());
            exploded = true;
        }
    }

    public IEnumerator AfterExplosion()
    {
        bombMeshObject.SetActive(false);
        yield return new WaitForSeconds(2.15f);
        bombMeshObject.SetActive(true);
        gameObject.SetActive(false);

        ticking = false;
        fuseSeconds = startFuseLength;
        Physics.IgnoreCollision(shooterCollider, GetComponent<Collider>(), false); //reset collision immunity
        //release particle
        //do additional stuff if need be
    }

    public IEnumerator ExplosionParticle()
    {
        explosionParticle.SetActive(true);
        explosionParticle.transform.SetParent(null);
        yield return new WaitForSeconds(2.05f);
        
        explosionParticle.transform.position = transform.position;
        explosionParticle.transform.SetParent(transform);
        explosionParticle.SetActive(false);
    }
}
