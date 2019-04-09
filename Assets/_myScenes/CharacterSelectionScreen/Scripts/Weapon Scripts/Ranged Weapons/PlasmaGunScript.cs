using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaGunScript : RangedWeaponScript
{
    public Vector2 recoilKnockback;
    public float windUpTime;
    private GameObject weaponHolder;
    public AudioClip windUp;
    Coroutine chargingShot;

    private IEnumerator FireWithDelay(float damageMod, float direction)
    {
        aud.clip = windUp;
        aud.Play();
        yield return new WaitForSeconds(windUpTime);
        aud.Stop();

        float sign = Mathf.Sign(weaponHolderCollider.gameObject.transform.rotation.y);
        base.Fire(damageMod, sign);
        weaponHolder = weaponHolderCollider.gameObject;
        weaponHolder.GetComponent<Rigidbody>().AddForce(new Vector3(recoilKnockback.x * -sign, recoilKnockback.y, 0), ForceMode.Impulse);
        weaponHolder.GetComponent<Animator>().SetTrigger("Staggered");
        weaponHolder.GetComponent<Animator>().SetLayerWeight(9, 0.23f);
        weaponHolder.GetComponent<BaseCharacterBehaviour>().SetDisablingMovementTime(0.35f);
        chargingShot = null;
    }

    public override void Fire(float damageMod, float direction)
    {
        canShoot = false;
        if(chargingShot == null)
            chargingShot = StartCoroutine(FireWithDelay(damageMod, direction));
    }
}
