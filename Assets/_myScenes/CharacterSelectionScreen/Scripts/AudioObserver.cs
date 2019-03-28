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
    }

    private void PlayOnEvent(AudioClip clip)
    {
        //aud.clip = clip;
        if(clip != null)
            aud.PlayOneShot(clip);
        //aud.Play();
    }
}
