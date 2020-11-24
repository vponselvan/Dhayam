using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
    public static GameManager instance;

    //Players    
    public List<PlayerController> players;
    public PlayerController activePlayer;
    public PlayerController localPlayer = new PlayerController();
    public int playerId;
    public string playerPrefabLocation;
    public Transform[] playerPositions;

    //Local player UI
    [HideInInspector]
    public MultiplayerUI myUI = new MultiplayerUI();

    //UI Positions
    public GameObject playerUiPanel;
    [SerializeField]
    private GameObject myUIGo;

    //Game state
    public enum GameStates
    {
        WAITING,
        DONE,
        ROLL_DICE,
        SWITCH_PLAYER
    }
    public GameStates state;

    //public int activePlayers;
    public bool isLocal;
    public int activePlayerIndex;
    private int playersInGame;
    private bool _turnPossible = true;
    private bool _switchPlayer;
    private bool _readyToStart = false;

    private float _waitDuration = 1.0f;
    private float _initialLoadDuration = 2.0f;
    private int _enterValue = 1;

    private void Awake()
    {
        instance = this;

        StartCoroutine(SetupUI());            
    }

    void Start()
    {
        //activePlayerIndex = 0;
        //activePlayers = PhotonNetwork.PlayerList.Length;
        //players = new PlayerController[activePlayers];
        //Debug.Log("Players count:: " + players.Length);       

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        
    }

    private void Update()
    {
        if (!_readyToStart)
            return;


        playerId = TurnManager.instance.currentPlayer.ActorNumber;
        //Debug.Log("Current Player :: " + playerId);        

        if (TurnManager.instance.currentPlayer == PhotonNetwork.LocalPlayer)
        {
            //Debug.Log("Active player index :: " + players.FindIndex(x => x.playerId == playerId));
            activePlayer = players[players.FindIndex(x => x.playerId == playerId)];
            switch (state)
            {
                case GameStates.ROLL_DICE:
                    if (_turnPossible)
                    {
                        //TO-DO: Selector Animation deactivate
                        //TO-DO: Focus on player dice
                        activePlayer.playerUI.SetDiceState(true);
                        if (DiceValue.GetValues().Count != 0)
                            DeactivateSelector(); //Need it if its reroll for kill
                        state = GameStates.WAITING;
                    }
                    break;
                case GameStates.WAITING:
                    break;
                case GameStates.DONE:
                    //Clear completed token
                    ClearToken();
                    //Check if won                    
                    if (DiceValue.GetValues().Count == 0)
                        state = GameStates.SWITCH_PLAYER;
                    else
                    {
                        activePlayer.playerUI.dice.DisplayDiceImages();
                        state = GameStates.WAITING;
                        PlayerMove();
                    }

                    break;
                case GameStates.SWITCH_PLAYER:
                    if (_turnPossible)
                    {
                        //TO-DO: Selector Animation deactivate
                        StartCoroutine(SwitchPlayer());
                        state = GameStates.WAITING;
                    }
                    break;
                default:
                    break;
            }
        }        
    }

    IEnumerator SetupUI()
    {
        GameObject uiPanel = Instantiate(playerUiPanel);
        int totalPlayers = PhotonNetwork.PlayerList.Length;
        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = 0;
        //Wait till all the players are loaded
        while (players.Count != totalPlayers)
        {
            yield return null;
        }
        //yield return new WaitForSeconds(_initialLoadDuration);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int id = players.FindIndex(x => x.playerId == playerNumber);
            myUIGo = uiPanel.transform.GetChild(index).gameObject;
            myUIGo.SetActive(true);
            myUIGo.GetComponent<MultiplayerUI>().Initialize(PhotonNetwork.CurrentRoom.GetPlayer(playerNumber));
            players[id].playerUI = myUIGo.GetComponent<MultiplayerUI>();
            //players[id].playerUI.dice = myUIGo.GetComponent<Dice>(); NOT REquired set at inspector
            playerNumber = (playerNumber % totalPlayers) + 1;
            index++;
        }
        _readyToStart = true;
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SetPlayers", RpcTarget.All);
    }

    [PunRPC]
    // creates the player data and spawns in the units
    void SetPlayers()
    {
        GameObject playerGo = PhotonNetwork.Instantiate(playerPrefabLocation, playerPositions[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
        playerGo.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        // set the owners of the two player's photon views

        //for (int i = 0; i < playersInGame; i++)
        //{            
        //    players[i].photonView.TransferOwnership(i + 1);
        //    players[i].photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(i + 1));
        //}
        // set the first player's turn
        //photonView.RPC("SetNextTurn", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(activePlayerIndex));
    }
    
    //void SetNextTurn(Player player)
    //{
    //    int playerNumber = player.ActorNumber;
    //    // is this the first turn?
    //    if (activePlayer == null)
    //    {
    //        Debug.Log("Active player index :: " + activePlayerIndex);
    //        activePlayer = players[activePlayerIndex];
    //        Debug.Log("Player count: " + players.Length);
    //    }        
    //    else
    //    {
    //        activePlayerIndex = (activePlayerIndex + 1) % playersInGame;
    //        activePlayer = players[activePlayerIndex];
    //    }

    //    // if it's our turn - enable the end turn button
    //    if (activePlayer.photonView.IsMine)
    //    {
    //        //Activate Dice
    //        myUI.SetDiceState(true);
    //        //activePlayer.BeginTurn();
    //    }
    //}

    IEnumerator SwitchPlayer()
    {
        if (_switchPlayer)
        {
            yield break;
        }

        _switchPlayer = true;
        //Clear DiceValue and Images
        DiceValue.ResetValue();
        DiceValue.diceImageList.Clear();
        yield return new WaitForSeconds(_waitDuration);
        activePlayer.playerUI.SetDiceState(false);
        activePlayer.playerUI.dice.DisplayDiceImages();     
        TurnManager.instance.photonView.RPC("SwitchPlayer", RpcTarget.AllBuffered);
        _switchPlayer = false;
    }

    public void SetTurnPossible(bool possible)
    {
        _turnPossible = possible;
    }

    public void DeactivateSelector()
    {
        for (int i = 0; i < activePlayer.tokens.Count; i++)
        {
            activePlayer.tokens[i].SetSelector(false);
            activePlayer.tokens[i].SetIsSelectable(false);
        }
    }

    void ClearTokenValues()
    {
        for (int i = 0; i < activePlayer.tokens.Count; i++)
        {
            activePlayer.tokens[i].movableValues.Clear();
        }
    }

    void SetTokenValues(Token token)
    {
        List<int> diceValue = new List<int>();

        diceValue = DiceValue.GetValues();

        for (int j = 0; j < diceValue.Count; j++)
        {
            Debug.Log("Token Value setting:: " + diceValue[j]);
            if (token.CanMove(diceValue[j]))
            {
                token.movableValues.Add(diceValue[j]);
            }
        }
    }

    void ClearToken()
    {
        for (int i = 0; i < activePlayer.tokens.Count; i++)
        {
            //Debug.Log("Token is complete :: " + playerList[_activePlayer].playerTokens[i].GetIsComplete());
            if (activePlayer.tokens[i].GetIsComplete())
            {
                //TO-DO: Animation on destroy
                Destroy(activePlayer.tokens[i].gameObject);
                activePlayer.tokens.Remove(activePlayer.tokens[i]);
                //Debug.Log("Token count afeter destroy :: " + playerList[_activePlayer].playerTokens.Count);
                if (activePlayer.tokens.Count == 0)
                    SetWin();
                return;
            }
        }
    }

    public void SetWin()
    {
        //TO-DO: Show animation
        activePlayer.hasWon = true;
        for (int i = 0; i < SaveSettings.winners.Length; i++)
        {
            if (SaveSettings.winners[i] == "")
            {
                SaveSettings.winners[i] = TurnManager.instance.currentPlayer.NickName;
                break;
            }
        }
        //Debug.Log("Player " + playerList[_activePlayer].playerName + " has won");

        //TO-DO: check if game ended
    }

    public void PlayerMove()
    {    

        List<Token> moveableToken = new List<Token>();


        List<int> diceValue = new List<int>();

        diceValue = DiceValue.GetValues();//_rolledHumanDice.Distinct();

        //Clear existing token moveable values before re-adding values
        ClearTokenValues();
        int value = diceValue[0];

        //1: Get Moveable Token list
        if (value == _enterValue)
        {
            //Anyone in base
            for (int i = 0; i < activePlayer.tokens.Count; i++)
            {
                if (!activePlayer.tokens[i].GetIsActive())
                {
                    //Add the only moveable value for inactive tokens
                    activePlayer.tokens[i].movableValues.Add(value);
                    moveableToken.Add(activePlayer.tokens[i]);
                }
            }
            moveableToken.AddRange(PossibleTokens(value));
        }
        else
        {
            moveableToken.AddRange(PossibleTokens(value));
        }
        //Check if we have any moveable token
        if (moveableToken.Count > 0)
        {
            for (int i = 0; i < moveableToken.Count; i++)
            {
                //Turn on selector for the token and add all the moveable values
                moveableToken[i].SetSelector(true);
                moveableToken[i].SetIsSelectable(true);
                Debug.Log("Token id : " + i + " Selectable :: " + moveableToken[i].GetIsSelectable());
                if (moveableToken[i].GetIsActive())
                {
                    SetTokenValues(moveableToken[i]);
                }
            }
        }
        //If no moveable token - Reset dicevalue and switch player?
        else
        {
            DiceValue.ResetValue();
            state = GameStates.SWITCH_PLAYER;
        }       
    }

    List<Token> PossibleTokens(int value)
    {
        List<Token> tokenList = new List<Token>();

        for (int i = 0; i < activePlayer.tokens.Count; i++)
        {
            if (activePlayer.tokens[i].GetIsActive())
            {
                if (activePlayer.tokens[i].CanMove(value))
                {
                    tokenList.Add(activePlayer.tokens[i]);
                }
                //Debug.Log("current token : " + i);
            }
        }

        return tokenList;
    }

}
