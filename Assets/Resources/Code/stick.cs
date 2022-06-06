using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class stick : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject master;
    GameObject masterbody;
    GameObject masterfeet;
    public GameObject cldr;
    public Transform pltf;
    public bool col = false;
    public float drag_force = 10000f;
    public float len = 30f;
    public ParticleSystem hit_FX;
    public PhotonView PView;
    void Start()
    {
        PView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        pltf = master.GetComponent<Transform>();
        masterbody = master.GetComponent<MouseControls>().player;
        masterfeet = master.GetComponent<MouseControls>().feet;
    }

    [PunRPC]
    void die_net()
    {
        master.GetComponent<MouseControls>().flying = false;
        Destroy(gameObject);
    }

    void die()
    {
        PView.RPC("die_net", RpcTarget.All);
    }

    /*
        private void OnCollisionEnter(Collision collision)
        {
            if (!col)
            {
                ContactPoint contact = collision.contacts[0];
                rb.rotation = Quaternion.LookRotation(rb.position - contact.point, Vector3.up);
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                col = true;
            }
        }
    */

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint pnt = collision.contacts[0];
        cldr = pnt.otherCollider.gameObject;
        if ((cldr.name != masterbody.name)&&(cldr.name != masterfeet.name))
        {
            if (!col)
            {
                var det = Object.Instantiate(hit_FX);
                det.transform.position = rb.position;
                //det.transform.localScale = new Vector3(10, 10, 10);
                if (pnt.otherCollider.attachedRigidbody == null)
                {
                    rb.rotation = Quaternion.LookRotation(rb.position - pltf.position, Vector3.up);
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    col = true;
                }
                else
                {
                    pnt.otherCollider.attachedRigidbody.AddForceAtPosition((pltf.position - rb.position).normalized * drag_force * pnt.otherCollider.attachedRigidbody.mass, pnt.point);
                    masterbody.GetComponent<Rigidbody>().AddForce((rb.position - pltf.position).normalized * drag_force * pnt.otherCollider.attachedRigidbody.mass);
                    die();
                }
            }
        }
    }

    void Update()
    {
        if (master == null)
            Destroy(gameObject);
        if ((rb.position - pltf.position).magnitude > len)
        {
            master.GetComponent<MouseControls>().cooldown[1] += 100;
            die();
        }
    }
}
