using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class moveurfuckinghead : MonoBehaviour
{
    Camera cam;
    Transform trans;
    float xrot = 0;
    public float fov = 96f;
    public Transform tf;
    public float sens = 30;
    public PhotonView PView;
    public GameObject menu;
    UI_handler UI;

    void Start()
    {
        cam = GetComponent<Camera>();
        trans = GetComponent<Transform>();
        UI = menu.GetComponent<UI_handler>();
    }

    void Update()
    {
        if (!PView.IsMine) return;
        if (UI.menu_on) return;
        float mousex = Input.GetAxis("Mouse X") * sens / Time.deltaTime;
        float mousey = Input.GetAxis("Mouse Y") * sens / Time.deltaTime;
        xrot -= mousey;
        xrot = Mathf.Clamp(xrot, -90, 90);
        transform.localRotation = Quaternion.Euler(xrot, 0, 0);
        tf.Rotate(Vector3.up * mousex);
        if (Input.GetKeyDown("="))
            sens += 50f;
        if (Input.GetKeyDown("-"))
            sens -= 50f;
    }
}
