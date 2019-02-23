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

    bool canShoot = true;

    private void Start()
    {
        pooler = ObjectPooler.instance;
        canvas = GameManagerScript.gmInstance.canvas;
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
}
