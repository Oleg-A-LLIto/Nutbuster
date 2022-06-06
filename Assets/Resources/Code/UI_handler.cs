using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI_handler : MonoBehaviour
{
    public bool menu_on = false;
    public GameObject menu;
    public int menuSceneIndex = 0;
    private AudioSource music;
    public Slider mus_vol;
    public Slider noise_vol;
    public AudioSource[] vols;

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();
        vols = Resources.FindObjectsOfTypeAll<AudioSource>();
        foreach (AudioSource vol in vols)
        {
            if (vol.gameObject != gameObject)
                vol.volume = 1;
        }
        music.volume = mus_vol.value;
    }

    public void sw()
    {
        menu_on = !menu_on;
        menu.SetActive(menu_on);
        Cursor.lockState = menu_on ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void quit()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(menuSceneIndex);
    }

    public void mv_update()
    {
        music.volume = mus_vol.value;
    }

    public void nv_update()
    {
        vols = Resources.FindObjectsOfTypeAll<AudioSource>();
        foreach (AudioSource vol in vols)
        {
            if (vol.gameObject != gameObject)
                vol.volume = noise_vol.value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
            sw();
    }
}
