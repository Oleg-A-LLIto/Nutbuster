using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class movementVR : MonoBehaviour
{
    public Rigidbody rb;
    public Transform tf;
    public CapsuleCollider body;
    public CapsuleCollider feet;
    public PhysicMaterial Player_PM;
    public PhysicMaterial Feet_PM;
    public PhysicMaterial Slide_PM;
    public PhysicMaterial standing;
    public Quaternion standing_angle;
    public AudioClip[] steps;
    public AudioClip snd_slide;
    public AudioClip snd_jump;
    public SteamVR_Action_Vector2 input_walk;
    public SteamVR_Action_Boolean input_jump;
    public int menuSceneIndex;
    public float max_bearable_speed = 20f;
    static public float globalspd = 6f;
    static private float max_climb_speed = 3f;
    public float speedometer;
    public float spd = globalspd;
    static public float globalacc = 320000f;
    AudioSource aud;
    float globalbh;
    float globalfh;
    float acc = globalacc;
    public bool jump = true;
    public float jumpability;
    bool crawl = false;
    bool run = false;
    bool ran = false;
    bool slide = false;
    private const float climbmax = 0.85f;
    public float climb = climbmax;
    public float stp = 0;
    float jumpforce = 100000f;
    public bool flying = false;
    public PhotonView PView;
    public GameObject menu;
    private ContactPoint contact;
    Vector3 playerdir;
    UI_handler UI;
    public float epsilon = 0.01f;

    void Start()
    {
        aud = GetComponent<AudioSource>();
        globalbh = body.height;
        globalfh = feet.height;
        PView = GetComponent<PhotonView>();
        UI = menu.GetComponent<UI_handler>();
    }

    private void OnCollisionStay(Collision collision)
    {
        jump = true;
        contact = collision.contacts[0];
        standing_angle = Quaternion.FromToRotation(Vector3.up, contact.normal);
    }

    void OnCollisionExit(Collision collision)
    {
        jump = false;
        stp = 40;
    }

    void OnCollisionEnter(Collision collision)
    {
        climb = climbmax;
    }

    void start_crawling()
    {
        body.height = globalbh * 0.5f;
        feet.height = globalfh * 0.5f;
        body.center = new Vector3(0, globalbh * 0.5f, 0);
        feet.center = new Vector3(0, globalfh * 0.5f, 0);
        crawl = true;
    }

    void stop_crawling()
    {
        feet.material = Feet_PM;
        if (jumpability > 0)
            tf.localPosition += new Vector3(0, 0.5f*(globalbh + globalfh) - 0.75f, 0);
        body.height = globalbh;
        feet.height = globalfh;
        body.center = new Vector3(0, 0, 0);
        feet.center = new Vector3(0, 0, 0);
        crawl = false;
        slide = false;
    }

    void start_running()
    {
        run = true;
    }

    void stop_running()
    {
        run = false;
    }

    void Cmdaction_run()
    {
        if (Input.GetKeyDown("left shift"))
        {
            if (crawl)
                stop_crawling();
            start_running();
        }
        if (Input.GetKeyUp("left shift"))
        {
            stop_running();
        }
    }

    void Cmdaction_crawl()
    {
        if (Input.GetKeyDown("c") || Input.GetKeyDown("left ctrl"))
        {
            if (jumpability > 0)
                rb.AddForce(Vector3.down * 100000);
            if (run)
            {
                stop_running();
                ran = true;
            }
            if (rb.velocity.magnitude > 7)
            {
                if (jumpability > 0)
                    rb.AddForce(rb.velocity * 8000f);
                feet.material = Slide_PM;
                slide = true;
                slide_noise();
            }
            start_crawling();
        }
        if (Input.GetKeyUp("c") || Input.GetKeyUp("left ctrl"))
        {
            stop_crawling();
            if (ran)
            {
                start_running();
                ran = false;
            }
        }
    }

    void slippery(Vector3 drctn)
    {
        if (!crawl)
            if (drctn.magnitude > epsilon)
                feet.material = standing;
            else
                feet.material = Feet_PM;
    }

    void stepsound()
    {
        if ((!slide) && (aud.isPlaying == false) && jump && (rb.velocity.magnitude > 0))
            if (stp >= 0.4f)
            {
                PView.RPC("playstep", RpcTarget.All);
                stp = 0;
            }
            else
                stp += rb.velocity.magnitude / globalspd * Time.deltaTime;
    }

    void adjust_stats()
    {
        spd = globalspd;
        acc = globalacc;
        if (crawl)
        {
            spd = globalspd * 0.8f;
        }
        if (run)
        {
            spd = globalspd * 2f;
            acc = globalacc * 2f;
        }
        if (jumpability <= 0)
        {
            if (!flying)
                acc = globalacc * 0.2f;
            else
            {
                acc = globalacc * 1f;
                spd = globalspd * 3f;
            }
                
        }

    }

    void slide_noise()
    {
        if ((aud.isPlaying == false) && (slide))
        {
            PView.RPC("playslide", RpcTarget.All);
        }
    }

    void walking(Vector3 drctn)
    {
        Vector3 move = drctn;
        move.Normalize();
        if ((Mathf.Abs(standing_angle.x) < 0.4f) && (Mathf.Abs(standing_angle.z) < 0.4f))
            move = standing_angle * move;
        else
        {
            if ((climb > 0) && (Vector3.Project(rb.velocity,Vector3.up).magnitude<max_climb_speed))
            {
                climb -= Time.deltaTime;
                move = standing_angle * move;
                rb.AddForceAtPosition(Time.deltaTime * Vector3.Normalize(contact.point - rb.position) * 75000, contact.point, ForceMode.Force);
                Debug.DrawRay(contact.point, Time.deltaTime * Vector3.Normalize(contact.point - rb.position) * 75000, Color.blue);
            }
            else
                return;
        }
        Debug.DrawRay(tf.position, move, Color.red);
        speedometer = rb.velocity.magnitude;
        if ( Vector3.Project(rb.velocity, move.normalized).magnitude < spd)
            rb.AddForce(move * acc * Time.deltaTime);
    }

    void Cmdaction_jump()
    {
        if (input_jump.state == true)
            if (jumpability>0)
            {
                PView.RPC("playjump", RpcTarget.All);
                rb.AddForce((standing_angle * Vector3.up * jumpforce + Vector3.up * jumpforce) / 2);
                jumpability = -0.2f;
            }
    }

    void action_respawn()
    {
        if (Input.GetKeyDown("r"))
            tf.SetPositionAndRotation(new Vector3(0, 3, 0), new Quaternion(0, 0, 0, 1));
    }

    [PunRPC]
    void playslide()
    {
        aud.clip = snd_slide;
        aud.Play();
    }

    [PunRPC]
    void playjump()
    {
        aud.clip = snd_jump;
        aud.Play();
    }

    [PunRPC]
    void playstep()
    {
        aud.clip = steps[Random.Range(0, steps.Length)];
        aud.Play();
    }

    void jump_control()
    {
        if (jump)
            if (jumpability < 0.4f)
                jumpability += 10f*Time.deltaTime;
            else
                jumpability = 0.4f;
        else
            if (jumpability > 0)
                jumpability -= Time.deltaTime;
    }

    void Update()
    {
        if (!PView.IsMine) return;
        if (UI.menu_on) return;
        playerdir = Player.instance.hmdTransform.TransformDirection(new Vector3(input_walk.axis.x,0,input_walk.axis.y))*100;
        jump_control();
        Cmdaction_jump();
        Cmdaction_crawl();
        Cmdaction_run();
        action_respawn();
        adjust_stats();
        slippery(playerdir);
        walking(playerdir);
        stepsound();
        if(rb.velocity.magnitude > max_bearable_speed)
            rb.velocity = rb.velocity.normalized * max_bearable_speed;
        tf.position = rb.position;
    }
}
