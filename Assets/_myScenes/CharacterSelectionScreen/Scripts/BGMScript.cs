using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMScript : MonoBehaviour
{
    IEnumerator ChangeTrack()
    {
        GetComponent<Animator>().SetTrigger("ChangeTrack");
        yield return new WaitForSeconds(5f);
        aud.clip = otherClip;
        aud.Play();
    }

    public AudioClip otherClip;
    AudioSource aud;
    
    void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    public void GameStart()
    {
        StartCoroutine(ChangeTrack());
    }
}
