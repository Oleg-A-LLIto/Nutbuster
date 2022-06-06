using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    
    /*
    public override void OnConnectedToMaster()
    {
        Debug.Log("nice");
        CreateRoom("FoodForSex");

    }
    */

    public override void OnCreatedRoom()
    {
        Debug.Log("super nice");
    }


    public void CreateRoom(string roomname)
    {
        PhotonNetwork.CreateRoom(roomname);
    }

    public void JoinRoom(string roomname)
    {
        PhotonNetwork.JoinRoom(roomname);
    }

    public void ChangeScene(string scenename)
    {
        PhotonNetwork.LoadLevel(scenename);
    }

}
