using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponScript : MonoBehaviour
{
    public ObjectPooler pooler;

    public int weaponDamage;
    public float rateOfFire;  
    public Vector2 pushback;
    public Vector2 throwPushback;
    public int throwDamage;
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
    protected Vector3 velo;

    public AudioClip[] audioClips;
    public delegate void EventSound(AudioClip clip);
    public static event EventSound SoundEvent;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        pooler = ObjectPooler.instance;
        rb.maxAngularVelocity = 150;
    }

    protected void Update()
    {
        if (canBeCollected)
        {
            transform.Rotate(Vector3.up * 30 * Time.deltaTime, Space.World);
        }
    }

    protected void FixedUpdate()
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

    protected IEnumerator SwingCD()
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
                if (gameObject.tag == "Lightsaber")
                {
                    Vector3 normal = (other.gameObject.transform.position - transform.position).normalized;
                    float x = normal.x;

                    //Makes x either 1 or -1 depenging on its position in relation to the character we it
                    x = x < 0 ? x = Mathf.FloorToInt(x) : x = Mathf.CeilToInt(x);

                    other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(throwPushback.x * x, throwPushback.y, 0), ForceMode.VelocityChange);
                    RaiseSoundEvent(audioClips[3]); //sound event
                }
                else
                {
                    other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(throwPushback.x * -Mathf.Sign(GetComponent<Rigidbody>().angularVelocity.x), throwPushback.y, 0), ForceMode.VelocityChange);
                    RaiseSoundEvent(audioClips[1]); //sound event
                }
                other.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(throwDamage);
                gameObject.SetActive(false);

            }
            //if hitting a floor, then just destroy this weapon
            else if (other.gameObject.layer == 9)
            {
                RaiseSoundEvent(audioClips[1]); //sound event
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
    public virtual IEnumerator Flung(Collider coll)
    {
        Physics.IgnoreCollision(coll, gameObject.GetComponent<Collider>());
        yield return new WaitForSeconds(0.5f);
        if (coll.gameObject != null && this != null)
        {
            Physics.IgnoreCollision(coll, gameObject.GetComponent<Collider>(), false);
        }
        
        canBeCollected = true;        
    }

    public virtual void RaiseSoundEvent(AudioClip clip)
    {
        SoundEvent(clip);
    }
}
