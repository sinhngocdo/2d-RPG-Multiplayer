using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowserScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button findRoomButton;

    [Header("Lobby")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;

    [Header("LobbyBrowser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefabs;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();

    public float numberCount;

    // Start is called before the first frame update
    void Start()
    {
        numberCount = 0;

        //disable the menu buttons at the start of the game
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        //enable the cursor
        Cursor.lockState = CursorLockMode.None;

        //if we are in game or not
        if(PhotonNetwork.InRoom)
        {
            //make the room visiable again
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }


    //Swab current screen
    public void SetScreen(GameObject screen)
    {
        //disable all screen first
        mainScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        //active the requested Screen
        screen.SetActive(true);

        if (screen == lobbyBrowserScreen)
        {
            UpdateLobbyBrowserUI();
        }
    }

    public void OnBackToMainScreen()
    {
        SetScreen(mainScreen);
    }

    public void OnPlayerNameChange(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnConnectedToMaster()
    {
        //enable the menu buttons when we connected to master
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;
    }

    public void OnCreateRoonButton()
    {
        SetScreen(createRoomScreen);
    }

    public void OnFindRoomButton()
    {
        SetScreen(lobbyBrowserScreen);
    }

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    [PunRPC]
    void UpdateLobbyUI()
    {
        //enable start button just for the player who create the room
        startGameButton.interactable = PhotonNetwork.IsMasterClient;

        //display all of the players
        playerListText.text = "";

        //loop through all players
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        //set the room info text
        roomInfoText.text = "<b>Room Name </b> \n" + PhotonNetwork.CurrentRoom.Name;
    }

    public void OnStartGameButton()
    {
        
        //invisable the room which client master going to start it
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        numberCount += 1;
        //Tell everyone to load to the game scene
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    public void OnStartGameButtonClicked()
    {
        numberCount += 1;
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    GameObject CreateRoomButton()
    {
        Debug.LogWarning("Created Room");
        GameObject buttonObject = Instantiate(roomButtonPrefabs, roomListContainer.transform);
        roomButtons.Add(buttonObject);
        return buttonObject;
    }


    void UpdateLobbyBrowserUI()
    {
        //disable all rooms buttons
        foreach (GameObject button in roomButtons)
        {
            button.SetActive(false);
        }

        //display all current rooms in the master Client
        for (int i = 0; i < roomList.Count; i++)
        {
            //Get or create the button object 
            GameObject button = i >= roomButtons.Count ? CreateRoomButton() : roomButtons[i];

            button.SetActive(true);
            //Set the room name and player count text
            button.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = roomList[i].Name;
            button.transform.Find("PlayerCountText").GetComponent<TextMeshProUGUI>().text = roomList[i].PlayerCount + " / " + roomList[i].MaxPlayers;

            //set the button when we click on them
            Button buttonComp = button.GetComponent<Button>();
            string roomName = roomList[i].Name;

            buttonComp.onClick.RemoveAllListeners();
            buttonComp.onClick.AddListener(() => { OnjoinRoomButton(roomName); });
        }
    }

    public void OnRefreshButton()
    {
        UpdateLobbyBrowserUI();
    }

    public void OnjoinRoomButton(string roomName)
    {
        NetworkManager.instance.JoinRoom(roomName);
    }

    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        roomList = allRooms;
    }
}
