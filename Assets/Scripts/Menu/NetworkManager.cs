using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    public static NetworkManager instance;

    [Header("Components")]
    //public PhotonView photonView;

    public string roomName;


    private void Awake()
    {
        if (instance != null && instance != this)
            //gameObject.SetActive(false);
            Destroy(instance.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
        
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }   

    public string CreateRoom()
    {
        int roomNumber = Random.Range(0, 1000000);
        RoomOptions options = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte) Constants.maxPlayers };
        PhotonNetwork.CreateRoom(roomNumber.ToString(), options);
        Debug.Log("Room " + roomNumber + " created");
        return roomNumber.ToString();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room already exist.");
        CreateRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("Room count:: " + roomList.Count);
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log("Available Room Name ::" + roomList[i]);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Join Room failed:: " + message);
    }

    [PunRPC]
    public void ChangeScene(int sceneIndex)
    {
        PhotonNetwork.LoadLevel(sceneIndex);
    }
    
}
