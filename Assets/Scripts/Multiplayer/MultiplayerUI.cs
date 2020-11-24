using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MultiplayerUI : MonoBehaviourPun
{
    [Header("PlayerInfo")]
    public Image avatar;
    public TextMeshProUGUI playerName;
    public Button diceButton;

    [Header("Dice")]
    public GameObject _valueContainer;
    public Dice dice;

    private PlayerController player;    

    //Colors - Temporary
    private Color[] _colors = new Color[4]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    public static MultiplayerUI instance;

    private void Awake()
    {
        instance = this;
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        Debug.Log("Player Name on UI:: " + player.NickName);
        int id = player.ActorNumber - 1;
        //GameManager.instance.players[id].playerUI = this;
        playerName.text = player.NickName;
        avatar.color = _colors[id];
        //int playerCount = PhotonNetwork.PlayerList.Length;
        //for (int i = 0; i < playerCount; i++)
        //{
                
        //}
    }

    //public void UpdateDiceValue()
    //{

    //}

    public void SetDiceState(bool on)
    {
        diceButton.interactable = on;
    }

}
