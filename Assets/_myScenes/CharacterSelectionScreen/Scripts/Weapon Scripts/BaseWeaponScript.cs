using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponScript : MonoBehaviour
{
    public ObjectPooler pooler;

    public int weaponDamage;
    public float rateOfFire;
    public string weaponName;
    public Sprite weaponSprite;
    private Canvas canvas;

    public bool isEquipped = false;
    public bool canBeCollected = true;
    public bool canSpin = false;

    private bool canShoot = true;

    private void Start()
    {
        pooler = ObjectPooler.instance;
        canvas = GameManagerScript.gmInstance.canvas;
    }

    private void Update()
    {
        if(canBeCollected)
        {
            transform.Rotate(Vector3.up * 30 * Time.deltaTime, Space.World);
        }
    }

    public void Fire(float damageMod, float direction)
    {
        if (canShoot)
        {
            StartCoroutine(FireCD());
            GameObject bullet;

            //the tag, meaning the first parameter, is the name of the kind of bullet that will be pulled out of the object pooler
            bullet = pooler.SpawnFromPool("Bullet1", transform.position, Quaternion.Euler(0, 90 * -direction, 0));
            BulletScript bScript = bullet.GetComponent<BulletScript>();

            bScript.damage = (int)(weaponDamage * (damageMod));
            bScript.direction = direction;
            bScript.AfterEnable();
        }
    }

    private IEnumerator FireCD()
    {
        canShoot = false;
        yield return new WaitForSeconds(rateOfFire);
        canShoot = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Ignore collision if it is not with ground or player
        if(collision.collider.gameObject.layer != 9 && collision.collider.gameObject.layer != 10)
        {
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
        }
        else
        {
            StopOnGround();
        }
    }

    private void StopOnGround()
    {
        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, out hit);

        if (hit.distance < 0.4f && hit.collider.gameObject.layer == 9)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().isKinematic = true;

            canSpin = true;
        }
    }

    public IEnumerator Flung(Collider coll)
    {
        Physics.IgnoreCollision(coll, gameObject.GetComponent<Collider>());
        yield return new WaitForSeconds(0.5f);
        Physics.IgnoreCollision(coll, gameObject.GetComponent<Collider>(), false);
    }

    public IEnumerator PickableCD()
    {
        yield return new WaitForSeconds(1.1f);
        canBeCollected = true;
        canSpin = true;
    }
}
