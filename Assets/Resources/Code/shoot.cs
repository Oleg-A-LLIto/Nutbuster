using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class shoot : MonoBehaviour
{
    private Rigidbody rb;
    private Transform tf;
    public float force = 5000f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        rb.AddForce(tf.forward * force);
    }

}
