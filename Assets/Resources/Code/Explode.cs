using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Explode : MonoBehaviour
{
    Transform tf;
    public ParticleSystem expl_FX;
    public float range = 10f;
    public float timer = 3f;
    public float force = 750f;
    public float upForce = 1f;
    private float counter = 0f;
    void Start()
    {
        tf = GetComponent<Transform>();
    }

    void explosion()
    {
        Collider[] toboom = Physics.OverlapSphere(tf.position, range);
        foreach (Collider hit in toboom)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            RaycastHit hit_inf;
            if (rb != null)
                if (!Physics.Linecast(tf.position, rb.position, out hit_inf))
                    rb.AddExplosionForce(force, tf.position, range, upForce, ForceMode.Impulse);
                else
                    if (hit_inf.rigidbody != null)
                        rb.AddExplosionForce((force - force/hit_inf.rigidbody.mass)/2, tf.position, range, upForce, ForceMode.Impulse);
        }
        var det = Object.Instantiate(expl_FX);
        det.GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume;
        det.transform.position = tf.position;
        det.transform.localScale = new Vector3(10, 10, 10);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= timer)
            explosion();
    }
}
