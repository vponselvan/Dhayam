using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static Menu menu;

    [Header("Screens")]
    public GameObject introScreen;
    public GameObject offlineScreen;
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Intro Screen")]
    public Button enterButton;
    //public TMP_InputField playerName;

    [Header("Main Screen")]
    public Button privateButton;

    [Header("Lobby Screen")]
    //Options
    public GameObject optionScreen;
    public Button createRoomButton;
    public Button joinRoomButton;
    //Waiting area
    public GameObject waitingArea;
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public Button startGameButton;              // button the host can press to start the game
    public Button cancelLobbyButton;
    public TextMeshProUGUI roomNameTitle;
    //Join
    public GameObject joinScreen;
    //public TMP_InputField joinRoomName;
    public Button joinGameButton;

    [Header("Components")]
    //public PhotonView photonView;

    private string _roomName;

    private void Awake()
    {
        if (Menu.menu == null)
        {
            Menu.menu = this;
        }
        else
        {
            if (Menu.menu != this)
            {
                Destroy(Menu.menu.gameObject);
                Menu.menu = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        startGameButton.interactable = false;
    }

    void SetScreen (GameObject screen)
    {
        introScreen.SetActive(false);
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        offlineScreen.SetActive(false);
        optionScreen.SetActive(false);
        waitingArea.SetActive(false);
        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);
        joinScreen.SetActive(false);

        //Set required screen to active
        screen.SetActive(true);
        if (screen == lobbyScreen)
            optionScreen.SetActive(true);
        else if (screen == optionScreen || screen == waitingArea || screen == joinScreen)
            lobbyScreen.SetActive(true);
    }

    public void OnPlayerNameEnter (TMP_InputField playerNameInput)
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
            enterButton.interactable = false;
        else
        {
            enterButton.interactable = true;
            PhotonNetwork.NickName = playerNameInput.text;
        }
            
    }

    //Button events
    public void OnEnterGameButtonClick()
    {        
        if (PhotonNetwork.IsConnected)
        {
            SetScreen(mainScreen);            
        }
        else
        {
            SetScreen(offlineScreen);
        }
        
    }

    public void OnRetryButtonClick()
    {
        SetScreen(introScreen);
        PhotonNetwork.ConnectUsingSettings();
        OnEnterGameButtonClick();
    }

    public void OnQuitButtonClick()
    {
        //TO-DO: Quit Game
    }

    public void OnPrivateGameButtonClick()
    {
        SetScreen(lobbyScreen);
    }

    public void OnCancelLobbyButtonClick()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
        //mainScreen.SetActive(true);
        //lobbyScreen.SetActive(false);
        //optionScreen.SetActive(true);
        //waitingArea.SetActive(false);
    }

    public void OnCreateRoomButtonClick()
    {
        SetScreen(waitingArea);
        _roomName = NetworkManager.instance.CreateRoom();
        Debug.Log("Room Number:: " + _roomName + " current player count:: " + PhotonNetwork.PlayerList.Length);
        //OnJoinedRoom();            
    }    

    //Prompt to get room number
    public void OnJoinRoomButtonClick()
    {
        SetScreen(joinScreen);
    }

    public void OnRoomNameChanged(TMP_InputField roomNameInput)
    {
        //Enable Join game button
        if (string.IsNullOrEmpty(roomNameInput.text))
            joinGameButton.interactable = false;
        else
            joinGameButton.interactable = true;
    }

    //Actual join room
    public void OnJoinGameButtonClick(TMP_InputField roomNameInput)
    {
        Debug.Log("Join game clicked");
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    public void OnStartGameButtonClick()
    {
        Debug.Log("Game Started");
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.AllBuffered, Constants.privateScene);
    }

    public override void OnJoinedRoom()
    {        
        base.OnJoinedRoom();
        roomNameTitle.text = _roomName;
        SetScreen(waitingArea);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        ResetPlayers();
        int playerCount = PhotonNetwork.PlayerList.Length;
        //PhotonNetwork.NickName = playerName.text;
        roomNameTitle.text = _roomName;
        Debug.Log(PhotonNetwork.NickName + " Joined room " + _roomName);
        int i = 1;
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            switch (i)
            {
                case 1:
                    player1.SetActive(true);
                    TextMeshProUGUI player1Name = player1.GetComponentInChildren<TextMeshProUGUI>();
                    player1Name.text = player.NickName;
                    break;
                case 2:
                    player2.SetActive(true);
                    TextMeshProUGUI player2Name = player2.GetComponentInChildren<TextMeshProUGUI>();
                    player2Name.text = player.NickName;
                    break;
                case 3:
                    player3.SetActive(true);
                    TextMeshProUGUI player3Name = player3.GetComponentInChildren<TextMeshProUGUI>();
                    player3Name.text = player.NickName;
                    break;
                case 4:
                    player4.SetActive(true);
                    TextMeshProUGUI player4Name = player4.GetComponentInChildren<TextMeshProUGUI>();
                    player4Name.text = player.NickName;
                    break;
                default:
                    break;
            }

            i++;
        }       

        if (playerCount == Constants.maxPlayers)
            //Load Game Scene
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.AllBuffered, Constants.privateScene);        
        //else if (playerCount > 1 && PhotonNetwork.IsMasterClient)
        else if (PhotonNetwork.IsMasterClient) //TO-DO: Testing. NEed to change it to above
            startGameButton.interactable = true;
    }

    void ResetPlayers()
    {
        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdateLobbyUI();
    }



    //public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    //{
    //    StartCoroutine(LeaveRoom());
    //}

    //IEnumerator LeaveRoom()
    //{
    //    PhotonNetwork.LeaveRoom();
    //    while (PhotonNetwork.InRoom)
    //        yield return null;
    //    SceneManager.LoadScene(Constants.menuScene);
    //}

}
