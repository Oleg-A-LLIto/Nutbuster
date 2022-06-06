using UnityEngine;
using System.IO;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    public string tospawn;
    private Transform tf;
    private void Start()
    {
        tf = GetComponent<Transform>();
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Debug.Log("Spawning a player ("+tospawn+");");
        PhotonNetwork.Instantiate(tospawn, tf.position, tf.rotation);
    }
}
