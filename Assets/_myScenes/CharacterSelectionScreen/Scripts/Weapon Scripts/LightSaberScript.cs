using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaberScript : MeleeWeaponScript
{
    public override IEnumerator Flung(Collider coll)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().loop = false;
        return base.Flung(coll);
    }
}
