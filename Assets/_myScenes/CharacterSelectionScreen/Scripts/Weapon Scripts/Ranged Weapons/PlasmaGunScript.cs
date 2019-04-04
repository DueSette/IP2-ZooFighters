using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaGunScript : RangedWeaponScript
{
    public Vector2 recoilKnockback;

    public override void Fire(float damageMod, float direction)
    {
        base.Fire(damageMod, direction);
        weaponHolderCollider.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(recoilKnockback.x * -Mathf.Sign(weaponHolderCollider.gameObject.transform.rotation.y), recoilKnockback.y, 0), ForceMode.Impulse);
    }
}
