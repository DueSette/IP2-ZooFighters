using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObserver : MonoBehaviour
{
    AudioSource aud;
    // Start is called before the first frame update
    void Start()
    {
        aud = GetComponent<AudioSource>();
        BulletScript.HitCharacter += PlayOnEvent;
        MeleeWeaponScript.BaseballBreak += PlayOnEvent;
        RangedWeaponScript.GunBreak += PlayOnEvent;
        GrenadeScript.OnExplode += PlayOnEvent;
    }

    private void PlayOnEvent(AudioClip clip)
    {
        if(clip != null)
            aud.PlayOneShot(clip);
    }
}
