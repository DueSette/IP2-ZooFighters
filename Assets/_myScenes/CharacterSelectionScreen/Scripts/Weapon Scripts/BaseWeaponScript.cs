using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponScript : MonoBehaviour
{
    public ObjectPooler pooler;

    public int weaponDamage;
    public float rateOfFire;
    public int ammo;
    public Sprite weaponSprite;

    public Collider weaponHolderCollider;

    public bool isEquipped = false;
    public bool canBeCollected = true;
    public bool canSpin = false;
    private bool canShoot = true;
    public bool actAsBullet = false;


    private void Start()
    {
        pooler = ObjectPooler.instance;
        gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 150;
    }

    private void Update()
    {      
        if(canBeCollected)
        {
            transform.Rotate(Vector3.up * 30 * Time.deltaTime, Space.World);
        }
    }

    //shooting a bullet (instantiating it from the pool)
    public void Fire(float damageMod, float direction)
    {
        if (canShoot && ammo > 0)
        {
            StartCoroutine(FireCD());
            ammo--;
            GameObject bullet;

            //the tag, meaning the first parameter, is the name of the kind of bullet that will be pulled out of the object pooler
            bullet = pooler.SpawnFromPool("Bullet1", transform.position, Quaternion.Euler(0, 90 * direction, 0));
            BulletScript bScript = bullet.GetComponent<BulletScript>();

            //this informs the bullet about what collider to ignore when shooting
            bScript.shooterCollider = weaponHolderCollider;
            bScript.damage = (int)(weaponDamage * (damageMod));
            bScript.direction = direction;
            bScript.AfterEnable();

            //TO ADD: stuff about the shot: sound, muzzle flash? animation??
        }
    }

    //manages the rate of fire
    private IEnumerator FireCD()
    {
        canShoot = false;
        yield return new WaitForSeconds(rateOfFire);
        canShoot = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Ignore collision if it is not with ground or player
        if (collision.collider.gameObject.layer != 9 && collision.collider.gameObject.layer != 10)
        {
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
        }
        else if (collision.collider.gameObject.layer == 9 && collision.gameObject.transform.position.y < transform.position.y)
        {
            StopOnGround();
        }
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
            if (collider.gameObject.layer == 10)
            {
                collider.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(50 * -Mathf.Sign(GetComponent<Rigidbody>().angularVelocity.x), 50, 0), ForceMode.VelocityChange);
                collider.gameObject.GetComponent<BaseCharacterBehaviour>().TakeDamage(30);
                gameObject.SetActive(false);
            }
            //if hitting a floor, then just destroy this weapon
            else if (collider.gameObject.layer == 9)
            {
                gameObject.SetActive(false);
                print("hit floor");
            }           
        }
    }

    private void StopOnGround()
    {
        float yRot = 0; //will determine the rotation that will be set as the gun's rotation upon landing
        if(transform.rotation.y < 0)
        {
            yRot = -90;
        }
        else
        {
            yRot = 90;
        }

        //makes the landing look smooth but also snappy and clean
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, yRot, transform.rotation.z));

        //stops the object
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;

        canBeCollected = true;
        canSpin = true;
    }

    //when this is called, it makes the weapon unable to instantly collide again with the player
    public IEnumerator Flung(Collider coll)
    {
        Physics.IgnoreCollision(coll, gameObject.GetComponent<Collider>());
        yield return new WaitForSeconds(1);
        if (coll.gameObject != null)
        {
            Physics.IgnoreCollision(coll, gameObject.GetComponent<Collider>(), false);
        }
        if (ammo > 0)
        {
            canBeCollected = true;
        }
    }
}
