using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PlayerController : MonoBehaviourPun
{

    public int id;                              //Index
    public int playerId;                        //Actor number
    private int _localPlayerId;                               //Kill player Id

    private string _color;
    //public bool isLocal;
    public Player photonPlayer;                 // Photon.Realtime.Player class
    public Transform[] cameraPositions;

    [HideInInspector]
    public MultiplayerUI playerUI;

    public Base[] bases = new Base[4];
    public int totalTokens = 6;
    public string[] tokensToSpawn;              //Token location
    private List<Transform> tokenSpawnPositions = new List<Transform>();      // array of all spawn positions for this player    

    public List<Token> tokens = new List<Token>(); // list of all our tokens

    public bool hasKill;
    public bool hasWon;

    // called when the game begins
    [PunRPC]
    void Initialize(Player player)
    {
        photonPlayer = player;
        playerId = player.ActorNumber;
        id = playerId - 1;
        Debug.Log("Player Actor Number :: " + id);
        GameManager.instance.players.Add(this);
        // if this is our local player, spawn the units
        if (player.IsLocal)
        {                       
            SetTokenPosition();            
            //this.isLocal = true;
            Transform cameraTransform = GetComponentInChildren<Camera>().gameObject.transform;
            cameraTransform.position = cameraPositions[id].position;
            cameraTransform.rotation = cameraPositions[id].rotation;            
            SpawnTokens();  
        }
        else
        {
            //this.isLocal = false;
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }        
    }

    void SetTokenPosition()
    {        
        bases = GameObject.FindWithTag(Constants.basesTag).GetComponentsInChildren<Base>();
        tokenSpawnPositions.AddRange(bases[id].GetComponentsInChildren<Transform>().Where(x => x != bases[id].transform));
    }

    // spawns the player's units
    void SpawnTokens()
    {
        for (int x = 0; x < totalTokens; ++x)
        {            
            GameObject token = PhotonNetwork.Instantiate(tokensToSpawn[id], tokenSpawnPositions[x].position, Quaternion.identity);
            token.GetPhotonView().RPC("Initialize", RpcTarget.AllBuffered, photonPlayer, tokenSpawnPositions[x].position);
            //token.GetPhotonView().RPC("Initialize", RpcTarget.Others, false);
            //token.GetPhotonView().RPC("Initialize", photonPlayer, true);
        }
    }

    //[PunRPC]
    //void UpdateTile(int tileId, string operation)
    //{
    //    int index = tokens[0].fullPath.FindIndex(x => x.tileId == tileId);
    //    Debug.Log("Tile Index ::" + index + " TileId ::" + tileId);
    //    if (operation == "Add")
    //    {
    //        tokens[0].fullPath[index].token.Add(this);
    //        fullPath[index].SetIsTaken(true);
    //    }
    //    else if (operation == "Remove")
    //    {
    //        fullPath[index].token.Remove(this);
    //        if (fullPath[index].token.Count > 0)
    //            fullPath[index].SetIsTaken(true);
    //        else
    //            fullPath[index].SetIsTaken(false);
    //    }
    //}

    //void Update()
    //{
    //    // only the local player can control this player
    //    if (!photonView.IsMine)
    //        return;

    //    // when we press the LMB and it's our turn
    //    if (Input.GetMouseButtonDown(0) && GameManager.instance.activePlayer == this)
    //    {
    //        // calculate the tile we selected and try to select whatever that is
    //        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        //TrySelect(new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0));
    //    }
    //}

    //public void ActivateDice()
    //{
    //    MultiplayerUI.instance.dice.interactable = true;
    //}

}
