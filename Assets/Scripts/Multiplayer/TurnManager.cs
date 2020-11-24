using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TurnManager : MonoBehaviourPun
{

    public static TurnManager instance;

    public Player currentPlayer;    
    private int _playersInGame;      //Active players count
    private List<int> _playersOrder = new List<int>();

    //Timer
    private float _turnDuration = 3.0f;

    private bool _isActive;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _playersInGame = PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.IsMasterClient)
        {
            int startPosition = Random.Range(1, _playersInGame + 1);
            photonView.RPC("InitialSetup", RpcTarget.AllBuffered, startPosition);
        }        
        
    }

    [PunRPC]
    void InitialSetup(int startPosition)
    {
        
        SetPlayerOrder(_playersInGame, startPosition);
        currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(startPosition);
        Debug.Log("Current Player :: " + startPosition);
        GameManager.instance.state = GameManager.GameStates.ROLL_DICE;
    }

    void SetPlayerOrder(int players, int startPlayer)
    {
        if (players == 4)
        {
            switch (startPlayer)
            {
                case 1:
                    _playersOrder.Add(1);
                    _playersOrder.Add(4);
                    _playersOrder.Add(2);
                    _playersOrder.Add(3);
                    break;
                case 2:
                    _playersOrder.Add(2);
                    _playersOrder.Add(3);
                    _playersOrder.Add(1);
                    _playersOrder.Add(4);
                    break;
                case 3:
                    _playersOrder.Add(3);
                    _playersOrder.Add(1);
                    _playersOrder.Add(4);
                    _playersOrder.Add(2);
                    break;
                case 4:
                    _playersOrder.Add(4);
                    _playersOrder.Add(2);
                    _playersOrder.Add(3);
                    _playersOrder.Add(1);
                    break;
                default:
                    break;
            }
        }
        else
        {
            for (int i = 0; i < players; i++)
            {
                _playersOrder.Add(startPlayer);
                startPlayer = (startPlayer % players) + 1;
            }
        }
    }

    [PunRPC]
    void SwitchPlayer()
    {
        //TO-DO: checks if any invalids like left room

        currentPlayer = GetNextPlayer(currentPlayer);
    }

    Player GetNextPlayer(Player player)
    {
        Debug.Log("Player order length ::" + _playersOrder.Count);
        int nextPlayerIndex = (_playersOrder.IndexOf(player.ActorNumber) + 1) % _playersInGame;
        GameManager.instance.state = GameManager.GameStates.ROLL_DICE;
        return PhotonNetwork.CurrentRoom.GetPlayer(_playersOrder[nextPlayerIndex]);
    }

    void OnPlayerLeft(Player player)
    {
        _playersInGame--;
        _playersOrder.Remove(player.ActorNumber);
    }

    


}
