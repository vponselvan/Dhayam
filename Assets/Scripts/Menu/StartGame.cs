using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class StartGame : MonoBehaviour
{
    private string[] _playerType;
    private string[] _playerName;
    private int _playerCount;
    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < SaveSettings.players.Length; i++)
        //{
        //    SaveSettings.players[i] = "COMPUTER";
        //}
    }

    public void StartClassic(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartHumanVsComputer()
    {
        _playerType = new string[2];
        _playerType[0] = "HUMAN";
        _playerType[1] = "COMPUTER";
        _playerName = new string[2];
        _playerName[0] = "ScaR";
        _playerName[1] = "Computer";
        SaveSettings.gameType = SaveSettings.GameType.OFFLINE;
        SaveSettings.SetPlayer(_playerType, _playerName);
        SceneManager.LoadScene(Constants.classicScene);
    }

    public void StartPrivate()
    {
        _playerCount = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < _playerCount; i++)
        {
            _playerType[i] = "HUMAN";
            _playerName[i] = "Player" + i + 1;
        }
        SaveSettings.gameType = SaveSettings.GameType.PRIVATE;
        SaveSettings.SetPlayer(_playerType, _playerName);
        //Room.room.StartGame();
    }
    
}
