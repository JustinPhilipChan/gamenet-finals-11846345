using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    /*
        ROOMS:
        Login Panel
        Game Options Room Panel (Create Game/ Join Game)
        //create join random panel (just 3 buttons, Race button, Time attack, and BACK)
        Create Room Panel
        Inside Room Panel (where players will select drones and ready up and start)

        2 GAME MODES (for custom properties)
        rc = normal race
        ta = time attack
    */
    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    [Header("Game Options UI")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Setup")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string GameMode;

    [Header("Join Random Room UI")]
    public GameObject JoinRandomRoomUIPanel;

    [Header("Inside Room UI")]
    public GameObject InsideRoomUIPanel;
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListParent;
    public GameObject StartGameButton;

    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Methods
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    
    void Update()
    {
        
    }
    #endregion

    #region  UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(GameOptionsUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("PlayerName is invalid!");
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        ActivatePanel(CreateRoomUIPanel.name);
        string roomName = RoomNameInputField.text;
        
        if(string.IsNullOrEmpty(roomName)) roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions roomOptions = new RoomOptions();
        string[] roomPropertiesInLobby = { "gm" }; //game mode
        roomOptions.MaxPlayers = 5;

        //ta = time attack      rc = racing
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { {"gm", GameMode} };

        roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnJoinRoomRandomClicked(string gameMode)
    {
        GameMode = gameMode;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { {"gm", gameMode} };

        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnBackButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm")) {
            if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc")) { //racing game mode
                PhotonNetwork.LoadLevel("RacingScene");
            } else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("ta")) { //time attack mode
                PhotonNetwork.LoadLevel("TimeAttackScene");
            }
        }
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to Photon");
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom + " has been created.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Player count is " + PhotonNetwork.CurrentRoom.PlayerCount);
        ActivatePanel(InsideRoomUIPanel.name);
        object gameModeName;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName)) {
            Debug.Log(gameModeName.ToString());
            RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        }

        if (playerListGameObjects == null) {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList) {
            GameObject playerListItem = Instantiate(PlayerListPrefab);
            playerListItem.transform.SetParent(PlayerListParent.transform);
            playerListItem.transform.localScale = Vector3.one;

            playerListItem.GetComponent<PlayerList>().Initialize(player.ActorNumber, player.NickName);

            object isPlayerReady;
            if(player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady)) {
                playerListItem.GetComponent<PlayerList>().SetPlayerReady((bool) isPlayerReady);
            }

            playerListGameObjects.Add(player.ActorNumber, playerListItem);
        }

        StartGameButton.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerListItem = Instantiate(PlayerListPrefab);
        playerListItem.transform.SetParent(PlayerListParent.transform);
        playerListItem.transform.localScale = Vector3.one;

        playerListItem.GetComponent<PlayerList>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerListItem);

        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Destroy(playerListGameObjects[other.ActorNumber].gameObject);
        playerListGameObjects.Remove(other.ActorNumber);

        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsUIPanel.name);

        foreach(GameObject playerListGameObject in playerListGameObjects.Values) {
            Destroy(playerListGameObject);
        }

        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

        public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        if (GameMode != null) {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room " + Random.Range(1000, 10000);
            }

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 5;

            string[] roomPropertiesInLobby = { "gm" }; //gm = game mode

            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { {"gm", GameMode} };

            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) 
    {
        GameObject playerlistGameObject;
        if(playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerlistGameObject)) {
            object isPlayerReady;
            if(changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady)) {
                playerlistGameObject.GetComponent<PlayerList>().SetPlayerReady((bool) isPlayerReady);
            }
        }

        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber) {
            StartGameButton.SetActive(CheckAllPlayerReady());
        }
    }

    #endregion

    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
    }
    #endregion

    #region Private Methods
    private bool CheckAllPlayerReady()
    {
        if(!PhotonNetwork.IsMasterClient) {
            return false;
        }

        foreach(Player p in PhotonNetwork.PlayerList) {
            object isPlayerReady;
            if(p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady)) {
                if(!(bool) isPlayerReady) {
                    return false;
                }
            } else {
                return false;
            }
        }

        return true;
    }
    #endregion
}
