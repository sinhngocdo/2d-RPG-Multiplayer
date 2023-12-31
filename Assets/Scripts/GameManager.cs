using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;


public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public PlayerController[] players;

    public Transform[] spawnPoint;
    public float respawnTime;
    private int playersInGame;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        //spawn player randomly in spawn point list position
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);

        //initialized
        playerObject.GetComponent<PhotonView>().RPC("Initialized", RpcTarget.All, PhotonNetwork.LocalPlayer);
        //
    }


}
