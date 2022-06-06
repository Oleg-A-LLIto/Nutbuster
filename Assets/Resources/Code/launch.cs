using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class launch : MonoBehaviour
{
    private Rigidbody rb;
    private Transform tf;
    public GameObject master;
    public float force = 5000f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        rb.AddForce(tf.forward * force);
        rb.AddForceAtPosition(tf.forward * force, tf.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
        rb.AddForce(tf.up * force/50);
        if (master == null) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
