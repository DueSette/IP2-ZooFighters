using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]

public class BulletScript : MonoBehaviour
{
    #region Data

    protected BaseCharacterBehaviour charScript;
    protected Rigidbody rb;
    public AudioSource aud;

    [HideInInspector]
    public int damage;
    [HideInInspector]
    public float direction;
    [HideInInspector]
    public Collider shooterCollider;    //reference to the character shooting this bullet

    public Vector2 pushBack;  
    public Vector2 bulletSpeed;
    [Tooltip("How long should this stop the players from moving when they are hit")]
    public float stopTargetDuration = 0;

    public delegate void HitTarget(AudioClip clip);
    public static event HitTarget HitCharacter;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
    }

    public virtual void OnEnable()
    {
        //stops the bullet in case it had some leftover momentum from a previous isntantiation (because the objects in the pool are the same)
        rb.velocity = Vector3.zero;
    }

    //This is called manually by the weapon script after it is done doing the initilisation stuff. Otherwise it can and will update info in the wrong order.
    public virtual void AfterEnable()
    {
        rb.AddForce(new Vector3(bulletSpeed.x * direction, bulletSpeed.y, 0), ForceMode.VelocityChange);
        Physics.IgnoreCollision(shooterCollider, GetComponent<Collider>());
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        //if this collides with something THAT IS NEITHER the weapon or the destroyer volume (since they are almost always going to collide with them)
        if (collider.gameObject.layer != 11 && !collider.name.Contains("Destroyer"))
        {
            //HITTING A CHARACTER
            if (collider.gameObject.GetComponent<BaseCharacterBehaviour>() && !collider.gameObject.GetComponent<BaseCharacterBehaviour>().respawned)
            {
                charScript = collider.gameObject.GetComponent<BaseCharacterBehaviour>();

                charScript.TakeDamage(damage);
                charScript.GetStopped(direction);

                collider.GetComponent<Rigidbody>().AddForce(new Vector3(pushBack.x * direction, pushBack.y, 0), ForceMode.Impulse);
                charScript.SetDisablingMovementTime(stopTargetDuration);
                RaiseSoundEvent(aud.clip);     //sound event
            }
            OnImpact();
        }
    }

    public virtual void OnImpact()
    {
        gameObject.SetActive(false);
        Physics.IgnoreCollision(shooterCollider, GetComponent<Collider>(), false); //reset collision immunity
        //release particle
        //do additional stuff if need be
    }

    protected void RaiseSoundEvent(AudioClip clip)
    {
        HitCharacter(clip);
    }
}
