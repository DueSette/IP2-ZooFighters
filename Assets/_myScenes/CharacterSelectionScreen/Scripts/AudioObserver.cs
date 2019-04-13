using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObserver : MonoBehaviour
{
    public List<AudioSource> sources = new List<AudioSource>();

    void OnEnable()
    {
        sources.AddRange(GetComponents<AudioSource>());
        BulletScript.HitCharacter += PlayOnEvent;
        MeleeWeaponScript.SoundEvent += PlayOnEvent;
        RangedWeaponScript.GunBreak += PlayOnEvent;
        GrenadeScript.OnExplode += PlayOnEvent;
        BaseCharacterBehaviour.SoundEvent += PlayOnEvent;
        SelectorBehaviour.OnSelectorAction += PlayOnEvent;
    }

    //plays specific clip on event
    private void PlayOnEvent(AudioClip clip)
    {
        if(clip != null)
            ChooseSource().PlayOneShot(clip);
    }

    //Selects which source should play the sound based on theri availability
    private AudioSource ChooseSource()
    {
        foreach(AudioSource source in sources)
        {
            if (source.isPlaying && source != null)
                continue;
            else
                return source;
        }
        sources.Add(gameObject.AddComponent<AudioSource>());
        return sources[sources.Count-1];
    }

    void OnDisable()
    {
        BulletScript.HitCharacter -= PlayOnEvent;
        MeleeWeaponScript.SoundEvent -= PlayOnEvent;
        RangedWeaponScript.GunBreak -= PlayOnEvent;
        GrenadeScript.OnExplode -= PlayOnEvent;
        BaseCharacterBehaviour.SoundEvent -= PlayOnEvent;
    }
}
