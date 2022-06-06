using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adjust_vol : MonoBehaviour
{
    public AudioSource a_src;
    public float vol;

    void Update()
    {
        a_src.volume = vol;
    }
}
