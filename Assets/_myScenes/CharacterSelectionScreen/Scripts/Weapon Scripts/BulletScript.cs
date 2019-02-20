using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]

public class BulletScript : MonoBehaviour
{
    private BaseCharacterBehaviour charScript;
    [Tooltip("This value will be modified by the weapon that's shooting it")]
    public int damage;
    public Vector2 pushBack;
    [Tooltip("Don't touch, this is visible only for debugging purposes")]
    public float direction;
    public Vector2 bulletSpeed;

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
        if (!collider.name.Contains("Weapon") || !collider.name.Contains("Destroyer"))
        {
            if (collider.gameObject.GetComponent<BaseCharacterBehaviour>())
            {
                charScript = collider.gameObject.GetComponent<BaseCharacterBehaviour>();
                charScript.TakeDamage(damage);
                collider.GetComponent<Rigidbody>().AddForce(new Vector3(pushBack.x * -direction, pushBack.y, 0), ForceMode.Impulse);
                gameObject.SetActive(false);
            }
            
        }
    }
}
