using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponScript : MonoBehaviour
{
    public ObjectPooler pooler;

    public int weaponDamage;
    public float rateOfFire;
    public int ammo;
    public Sprite weaponSprite;

    public Collider weaponHolderCollider;

    [HideInInspector]
    public bool isEquipped = false;
    [HideInInspector]
    public bool canBeCollected = true;
    [HideInInspector]
    public bool canSpin = false;
    [HideInInspector]
    public bool canShoot = true;
    [HideInInspector]
    public bool actAsBullet = false;

    public string bulletName;

    [HideInInspector]
    public Rigidbody rb;
    protected Vector3 velo;
    public AudioClip[] audioClips;
    public AudioSource aud;

    public delegate void EventSound(AudioClip clip);
    public static event EventSound GunBreak;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
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

    //shooting a bullet (instantiating it from the pool)
    public virtual void Fire(float damageMod, float direction)
    {
        StartCoroutine(FireCD());
        if (ammo > 0)
        {
            //the tag, meaning the first parameter, is the name of the kind of bullet that will be pulled out of the object pooler
            GameObject bullet = pooler.SpawnFromPool(bulletName, transform.position, Quaternion.Euler(0, 90 * direction, 0));
            BulletScript bScript = bullet.GetComponent<BulletScript>();

            //this informs the bullet about what collider to ignore when shooting
            bScript.shooterCollider = weaponHolderCollider;
            //this sets damage, direction and post-on enable stuff
            bScript.damage = (int)(weaponDamage * (damageMod));
            bScript.direction = direction;
            bScript.AfterEnable();

            aud.clip = audioClips[2];
            aud.Play();
        }
        else
        {
            aud.clip = audioClips[1];
            aud.Play();
        }
        ammo--;
        //TO ADD: stuff about the shot: sound, muzzle flash? animation
    }

    //manages the rate of fire
    public virtual IEnumerator FireCD()
    {
        canShoot = false;
        yield return new WaitForSeconds(rateOfFire);
        canShoot = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!actAsBullet)
        {
            //if touches floor and floor is beneath it
            if (collider.gameObject.layer == 9 && collider.gameObject.transform.position.y < transform.position.y)
            {
                if (!isEquipped)
                {
                    StopOnGround();
                }
            }
        }

        //if being thrown as the result of being out of ammo, this kind of collision will take place
        if (actAsBullet)
        {
            //if hitting a player, move and damage it
            if (collider.gameObject.layer == 10 && !collider.gameObject.GetComponent<BaseCharacterBehaviour>().respawned)
            {
                GunBreak(audioClips[0]);
                collider.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(50 * -Mathf.Sign(GetComponent<Rigidbody>().angularVelocity.x), 50, 0), ForceMode.VelocityChange);
                collider.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(30);
                gameObject.SetActive(false);
            }
            //if hitting a floor, then just destroy this weapon
            else if (collider.gameObject.layer == 9)
            {
                GunBreak(audioClips[0]);
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
        if (ammo > 0)
        {
            canBeCollected = true;
        }
    }
}
