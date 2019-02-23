using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]

public class BulletScript : MonoBehaviour
{
    private BaseCharacterBehaviour charScript;
    [HideInInspector]
    public int damage;
    public Vector2 pushBack;
    [Tooltip("Don't touch, this is visible only for debugging purposes")]
    public float direction;
    public Vector2 bulletSpeed;
    public float stopTargetDuration = 0;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public virtual void OnEnable()
    {
        //stops the bullet in case it had some leftover momentum from a previous isntantiation (because the objects in the pool are the same)
        rb.velocity = Vector3.zero;
    }

    //This is called manually by the weapon script after it is done doing the initilisation stuff. Otherwise it can and will update info in the wrong order.
    public virtual void LateOnEnable()
    {
        rb.AddForce(new Vector3(bulletSpeed.x * -direction, bulletSpeed.y, 0), ForceMode.VelocityChange);
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        //if this collides with something THAT IS NEITHER the weapon or the destroyer volume (since they are almost always going to collide with them)
        if (!collider.name.Contains("Weapon") && !collider.name.Contains("Destroyer"))
        {
            //if this hits a character
            if (collider.gameObject.GetComponent<BaseCharacterBehaviour>())
            {
                charScript = collider.gameObject.GetComponent<BaseCharacterBehaviour>();
                charScript.TakeDamage(damage);
                charScript.GetStopped(direction);
                StartCoroutine(charScript.DisableMove(stopTargetDuration));
                collider.GetComponent<Rigidbody>().AddForce(new Vector3(pushBack.x * -direction, pushBack.y, 0), ForceMode.VelocityChange);
            }
            OnImpact();
        }
    }

    public virtual void OnImpact()
    {
        gameObject.SetActive(false);
        //play sound
        //release particle

        //do additional stuff if need be
    }
}
