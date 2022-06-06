using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;

public class ActionControls : MonoBehaviour
{
    public float sens = 30;
    static int ITEMS = 1;
    int item = 1;
    public bool flying = false;
    public Transform tf;
    public Rigidbody rb;
    public Mesh cylinder;
    public Mesh[] item_meshes;
    public Material[] item_mats;
    public Sprite[] item_sights;
    public Slider[] bar = new Slider[ITEMS+1];
    public float[] scales;
    public float grapple_speed = 20f;
    //public GameObject sight; 
    public float prev_dist_to_hook;
    public float dist_to_hook;
    public Object bomb;
    public GameObject hook;
    public GameObject player;
    public GameObject feet;
    GameObject myhook;
    public MeshFilter inhands;
    public MeshRenderer inhands_render;
    public Material hook_mtl;
    float xrot = 0;
    public float fov = 96f;
    public float hookpower = 10000f;
    public AudioClip grapple_snd;
    public AudioSource aud;
    public float[] cooldown = new float[ITEMS+1];
    int[] lim = new int[ITEMS+1];
    float[] cdspeed = new float[ITEMS + 1];
    public Camera cam;
    public Transform trans;
    public PhotonView PView;
    public AudioListener ears;
    public MeshRenderer appearance;
    public MeshRenderer legsappearance;
    public GameObject playerUI;
    public GameObject sight;
    UI_handler UI;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //cam = GetComponent<Camera>();
        //trans = GetComponent<Transform>();
        cooldown[0] = 300;
        cooldown[1] = 400;
        lim[0] = 300;
        lim[1] = 400;
        cdspeed[0] = 30f;
        cdspeed[1] = 100f;
        PView = GetComponent<PhotonView>();
        if (!PView.IsMine)
        {
            Destroy(cam);
            Destroy(ears);
            Destroy(playerUI);
            appearance.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            legsappearance.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        else
        {
            UI = playerUI.GetComponent<UI_handler>();
        }
    }

    void throwbomb()
    {
        GameObject mybomb = PhotonNetwork.Instantiate(Path.Combine("Objects","Bomb"), trans.position + trans.forward * 2, trans.rotation);
        PView.RPC("daddy", RpcTarget.All, mybomb.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void daddy(int mythrowable)
    {
        GameObject mythrowableobj = PhotonView.Find(mythrowable).gameObject;
        mythrowableobj.GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume;
        try
        {
            mythrowableobj.GetComponent<stick>().master = gameObject;
        }
        catch
        {
            mythrowableobj.GetComponent<launch>().master = gameObject;
        }
    }

    [PunRPC]
    void playgrapple()
    {
        aud.clip = grapple_snd;
        aud.Play();
    }

    void grapple()
    {
        if (!flying)
        {
            flying = true;
        }
        else
        {
            PView.RPC("killhook", RpcTarget.All, myhook.GetComponent<PhotonView>().ViewID);
        }
        PView.RPC("playgrapple", RpcTarget.All);
        myhook = PhotonNetwork.Instantiate(Path.Combine("Objects", "Hook"), trans.position + trans.forward * 2, trans.rotation);
        PView.RPC("daddy", RpcTarget.All, myhook.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void killhook(int myhook)
    {
        GameObject myhookobj = PhotonView.Find(myhook).gameObject;
        Object.Destroy(myhookobj);
    }

    void switch_item()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            item++;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            item--;
        if (item > ITEMS)
            item = 0;
        if (item < 0)
            item = ITEMS;
        inhands.mesh = item_meshes[item];
        inhands_render.material = item_mats[item];
        inhands.transform.localScale = new Vector3(scales[item], scales[item], scales[item]);
        sight.GetComponent<Image>().sprite = item_sights[item];
    }

    void zoom()
    {
        if (!PView.IsMine)
            return;
            if (Input.GetMouseButton(1))
        {
            if (cam.fieldOfView > fov / 2)
                cam.fieldOfView -= fov / 16;
        }
        else
            if (cam.fieldOfView < fov)
            cam.fieldOfView += fov / 16;
    }

    void use_item()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (cooldown[item] >= 100)
            {
                cooldown[item] -= 100;
                switch (item)
                {
                    case 0:
                        throwbomb();
                        break;
                    case 1:
                        grapple();
                        break;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            switch (item)
            {
                case 0:
                    //throwbomb();
                    break;
                case 1:
                    if (flying)
                    {
                        flying = false;
                        if (!myhook.GetComponent<stick>().col)
                            cooldown[item] += 100;
                        PView.RPC("killhook", RpcTarget.All, myhook.GetComponent<PhotonView>().ViewID);
                    }
                    break;
            }
        }
    }

    void Update()
    {
        if (!PView.IsMine) return;
        if (UI.menu_on) return;
        zoom();
        use_item();
        switch_item();
        player.GetComponent<movement>().flying = flying;
        if (flying)
        {
            Vector3 dir_to_hook = (myhook.GetComponent<Rigidbody>().position - rb.position).normalized;
            dist_to_hook = (myhook.GetComponent<Rigidbody>().position - rb.position).magnitude;
            Graphics.DrawMesh(cylinder, Matrix4x4.TRS((myhook.GetComponent<Rigidbody>().position + rb.position) / 2, Quaternion.LookRotation(myhook.GetComponent<Rigidbody>().position - rb.position) * Quaternion.Euler(90,0,0), new Vector3(0.05f, dist_to_hook/2, 0.05f)), hook_mtl, 0);
            if (myhook.GetComponent<stick>().col)
            {
                if ((Vector3.Project(rb.velocity, dir_to_hook).magnitude<grapple_speed)) //if the hook speed is less than max speed then drag more
                    rb.AddForce(dir_to_hook * hookpower);
                if (dist_to_hook < 2)
                {
                    PView.RPC("killhook", RpcTarget.All, myhook.GetComponent<PhotonView>().ViewID);
                    flying = false;
                }
                if (prev_dist_to_hook < dist_to_hook) //prevent the rope from stretching
                {
                    rb.position += dir_to_hook * (dist_to_hook - prev_dist_to_hook);
                    if (Vector3.Dot(rb.velocity, dir_to_hook) < 0)
                    {
                        rb.velocity -= Vector3.Project(rb.velocity, dir_to_hook);
                    }
                }
            }
            prev_dist_to_hook = (myhook.GetComponent<Rigidbody>().position - rb.position).magnitude;
        }
        for (int i = 0; i<= ITEMS; i++)
        {
            if (cooldown[i] < lim[i])
                cooldown[i] += cdspeed[i]*Time.deltaTime;
            if (cooldown[i] > lim[i])
                cooldown[i] = lim[i];
            bar[i].value = cooldown[i]/lim[i];
        }
    }
}
