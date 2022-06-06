using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class QS_LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public GameObject quickStartButton;
    [SerializeField]
    public GameObject quickCancelButton;
    [SerializeField]
    private int RoomSize;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        QualitySettings.vSyncCount = 4;
        PhotonNetwork.NickName = "Clown-down-" + Random.Range(10000, 99999);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickStartButton.SetActive(true);
    }

    public void QuickStart()
    {
        quickStartButton.SetActive(false);
        quickCancelButton.SetActive(true);
        Debug.Log("tryna start");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join failed");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating a new room");
        int roomnum = Random.Range(0, 228);
        RoomOptions opts = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
        PhotonNetwork.CreateRoom("r" + roomnum, opts);
        Debug.Log(roomnum);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Creating a room failed, trying again...");
        CreateRoom();
    }

    public void QuickCancel()
    {
        quickStartButton.SetActive(true);
        quickCancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
            Application.Quit();
    }
}
