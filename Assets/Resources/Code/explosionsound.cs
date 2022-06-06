using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionsound : MonoBehaviour
{
    public AudioClip[] snds;
    AudioSource aud;
    void Start()
    {
        aud = GetComponent<AudioSource>();
        aud.clip = snds[Random.Range(0, snds.Length)];
        aud.Play();
    }
}
