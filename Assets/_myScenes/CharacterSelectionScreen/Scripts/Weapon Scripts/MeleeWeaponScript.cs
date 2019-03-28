using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponScript : MonoBehaviour
{
    public ObjectPooler pooler;

    public int weaponDamage;
    public float rateOfFire;  
    public Vector2 pushBack;
    public Sprite weaponSprite;

    [HideInInspector]
    public Collider weaponHolderCollider;
    [HideInInspector]
    public bool isEquipped = false;
    [HideInInspector]
    public bool canBeCollected = true;
    [HideInInspector]
    public bool canSpin = false;
    [HideInInspector]
    public bool canSwing = true;
    [HideInInspector]
    public bool swinging = false;
    [HideInInspector]
    public bool actAsBullet = false;

    [HideInInspector]
    public Rigidbody rb;
    private Vector3 velo;

    public AudioClip breakSound;
    public delegate void EventSound(AudioClip clip);
    public static event EventSound BaseballBreak;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pooler = ObjectPooler.instance;
        rb.maxAngularVelocity = 150;
    }

    private void Update()
    {
        if (canBeCollected)
        {
            transform.Rotate(Vector3.up * 30 * Time.deltaTime, Space.World);
        }
    }

    private void FixedUpdate()
    {
        velo = rb.velocity;
        if (velo.y < -45)
        {
            rb.AddForce(new Vector3(0, 2.7f, 0), ForceMode.Impulse);
        }
    }

    public void Swing()
    {
        StartCoroutine(SwingCD());
        swinging = true;
        GetComponent<AudioSource>().Play();
    }

    private IEnumerator SwingCD()
    {
        canSwing = false;
        yield return new WaitForSeconds(1);
        canSwing = true;
        swinging = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!actAsBullet)
        {
            //if touches floor and floor is beneath it
            if (other.gameObject.layer == 9 && other.gameObject.transform.position.y < transform.position.y)
            {
                if (!isEquipped)
                {
                    StopOnGround();
                }
            }
        }

        //if being thrown aggressively
        if (actAsBullet)
        {           
            //if hitting a player, move and damage it
            if (other.gameObject.layer == 10)
            {
                BaseballBreak(breakSound); //sound event
                other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(100 * -Mathf.Sign(GetComponent<Rigidbody>().angularVelocity.x), 50, 0), ForceMode.VelocityChange);
                other.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(20);
                gameObject.SetActive(false);
            }
            //if hitting a floor, then just destroy this weapon
            else if (other.gameObject.layer == 9)
            {
                BaseballBreak(breakSound); //sound event
                gameObject.SetActive(false);
            }          
        }
    }

    private void StopOnGround()
    {
        float yRot = 0; //will determine the rotation that will be set as the gun's rotation upon landing
        if (transform.rotation.y < 0)
        {
            yRot = -90;
        }
        else
        {
            yRot = 90;
        }

        //makes the landing look smooth but also snappy and clean
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, yRot, transform.rotation.z));
        transform.position = new Vector3(transform.position.x, transform.position.y + GetComponent<Collider>().bounds.extents.y, transform.position.z);

        //stops the object
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        canBeCollected = true;
        canSpin = true;
    }

    //when this is called, it makes the weapon unable to instantly collide again with the player
    public IEnumerator Flung(Collider coll)
    {
        Physics.IgnoreCollision(coll, gameObject.GetComponent<Collider>());
        yield return new WaitForSeconds(0.5f);
        if (coll.gameObject != null && this != null)
        {
            Physics.IgnoreCollision(coll, gameObject.GetComponent<Collider>(), false);
        }
        
        canBeCollected = true;        
    }
}
