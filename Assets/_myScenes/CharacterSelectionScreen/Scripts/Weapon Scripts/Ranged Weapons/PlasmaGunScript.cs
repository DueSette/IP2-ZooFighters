using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaGunScript : RangedWeaponScript
{
    public Vector2 recoilKnockback;
    private GameObject weaponHolder;

    public override void Fire(float damageMod, float direction)
    {
        base.Fire(damageMod, direction);
        weaponHolder = weaponHolderCollider.gameObject;
        weaponHolder.GetComponent<Rigidbody>().AddForce(new Vector3(recoilKnockback.x * -Mathf.Sign(weaponHolderCollider.gameObject.transform.rotation.y), recoilKnockback.y, 0), ForceMode.Impulse);
        weaponHolder.GetComponent<Animator>().SetTrigger("Staggered");
        weaponHolder.GetComponent<Animator>().SetLayerWeight(9, 0.23f);
        weaponHolder.GetComponent<BaseCharacterBehaviour>().SetDisablingMovementTime(0.35f);
    }
}
