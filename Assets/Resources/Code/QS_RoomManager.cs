using UnityEngine;
using Photon.Pun;

public class QS_RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int mutiplayerSceneIndex;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting game...");
            PhotonNetwork.LoadLevel(mutiplayerSceneIndex);
        }
    }
}
