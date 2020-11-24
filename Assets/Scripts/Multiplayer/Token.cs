using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Token : MonoBehaviourPun
{
   // [SerializeField]
    private int _teamId;
    //public int tokenId;    

    [Header("Path Settings")]
    public List<Path> paths;
    public List<Tile> fullPath = new List<Tile>();

    [Header("Tile Settings")]
    public List<Tile> startTiles;
    public Vector3 basePosition;
    private Tile _finalTile;
    private int[] _redTileIndex = new int[4] { 0, 0, 0, 0 };
    private int[] _greenTileIndex = new int[4] { 12, 8, 4, 0 };
    private int[] _blueTileIndex = new int[4] { 6, 12, 6, 0 };
    private int[] _yellowTileIndex = new int[4] { 18, 4, 2, 0 };

    private int _pathPosition;
    private int _currentTilePosition;
    private int _targetTilePosition;

    private int _steps; //Dice value
    private int _doneSteps;
    private int _level2Index = 24;

    private float _moveSpeed = 5.0f;
    private float _killMoveSpeed = 100.0f;
    private float _waitDuration = 0.1f;

    [Header("Booleans")]
    private bool isMine;
    private bool _isActive;
    private bool _isMoving;
    private bool _isComplete = false;
    private bool _isSelectable = false;
    private bool _hasTurn; //For Input

    [Header("Selector Settings")]

    public ParticleSystem _highlighter;
    public DiceValueSelector _diceValueContainer;
    public List<int> movableValues;
    //private ParticleSystem _selector;

    //Arc Movement
    private float amplitude = 0.5f;
    private float cTime = 0f;

    [Header("Components")]
    public Player photonPlayer;

    [Header("UI")]
    public Image valueList;
    public Button[] values;

    

    [PunRPC]
    void Initialize(Player player, Vector3 basePosition)
    {
        //if (GameManager.instance.players[id] == null)
        //    Debug.Log("Gamemanager player is null at id :: " + id);
        //if (GameManager.instance.players[id].tokens == null)
        //    Debug.Log("Token is null for id :: " + id);
        int id = GameManager.instance.players.FindIndex(y => y.playerId == player.ActorNumber);
        Debug.Log("Token player id ::" + id);
        GameManager.instance.players[id].tokens.Add(this);
        this.basePosition = basePosition;
        SetupPath(player);
    }

    private void Start()
    {
        SetSelector(false);
        //_highlighter = GetComponentInChildren<ParticleSystem>();
        //if (_highlighter == null)
        //{
        //    Debug.Log("Highlighter is null");
        //}
        //else
        //{
            
        //}

        

        if (!photonView.IsMine)
            return;

        //Transform[] children = transform.GetComponentsInChildren<Transform>();
        //foreach (var child in children)
        //{
        //    if (child.name == "ValueList")
        //    {
        //        _diceValueContainer = child.GetComponent<DiceValueSelector>();
        //        if (_diceValueContainer == null)
        //        {
        //            Debug.Log("Token:: Dice Value Container is null");
        //        }
        //    }
        //}

        photonPlayer = PhotonNetwork.LocalPlayer;
        _teamId = photonPlayer.ActorNumber;

        //switch (_teamId)
        //{
        //    case 1:
        //        for (int i = 0; i < paths.Count; i++)
        //        {
        //            int tileIndex = _redTileIndex[i];
        //            CreatePath(paths[i], tileIndex);
        //        }
        //        break;
        //    case 2:
        //        for (int i = 0; i < paths.Count; i++)
        //        {
        //            int tileIndex = _greenTileIndex[i];
        //            CreatePath(paths[i], tileIndex);
        //        }
        //        break;
        //    case 3:
        //        for (int i = 0; i < paths.Count; i++)
        //        {
        //            int tileIndex = _blueTileIndex[i];
        //            CreatePath(paths[i], tileIndex);
        //        }
        //        break;
        //    case 4:
        //        for (int i = 0; i < paths.Count; i++)
        //        {
        //            int tileIndex = _yellowTileIndex[i];
        //            CreatePath(paths[i], tileIndex);
        //        }
        //        break;
        //    default:
        //        break;
        //}

        ////for (int i = 0; i < paths.Count; i++)
        ////{
        ////    int tileIndex = paths[i].GetPosition(startTiles[i].gameObject.transform);
        ////    CreatePath(paths[i], tileIndex);
        ////}
        //_finalTile = fullPath[fullPath.Count - 1];
    }

    void SetupPath(Player player)
    {
        int playerId = player.ActorNumber;
        switch (playerId)
        {
            case 1:
                for (int i = 0; i < paths.Count; i++)
                {
                    int tileIndex = _redTileIndex[i];
                    CreatePath(paths[i], tileIndex);
                }
                break;
            case 2:
                for (int i = 0; i < paths.Count; i++)
                {
                    int tileIndex = _greenTileIndex[i];
                    CreatePath(paths[i], tileIndex);
                }
                break;
            case 3:
                for (int i = 0; i < paths.Count; i++)
                {
                    int tileIndex = _blueTileIndex[i];
                    CreatePath(paths[i], tileIndex);
                }
                break;
            case 4:
                for (int i = 0; i < paths.Count; i++)
                {
                    int tileIndex = _yellowTileIndex[i];
                    CreatePath(paths[i], tileIndex);
                }
                break;
            default:
                break;
        }

        //for (int i = 0; i < paths.Count; i++)
        //{
        //    int tileIndex = paths[i].GetPosition(startTiles[i].gameObject.transform);
        //    CreatePath(paths[i], tileIndex);
        //}
        _finalTile = fullPath[fullPath.Count - 1];
    }

    //GETTER SETTERS
    public bool GetIsActive()
    {
        return _isActive;
    }

    public bool GetIsComplete()
    {
        return _isComplete;
    }

    public bool GetIsSelectable()
    {
        return _isSelectable;
    }

    public void SetIsSelectable(bool on)
    {
        _isSelectable = on;
    }

    public void SetSelector(bool turnOn)
    {
        _hasTurn = turnOn; //Selectable Token by human
        _highlighter.gameObject.SetActive(turnOn);
    }

    void CreatePath(Path _path, int _pathIndex)
    {        
        for (int i = 0; i < _path.childTileList.Count; i++)
        {
            int tempPosition = _pathIndex + i;
            tempPosition %= _path.childTileList.Count;
            fullPath.Add(_path.childTileList[tempPosition].GetComponent<Tile>());
            int index = fullPath.Count - 1;
            fullPath[index].SetIsTaken(false);
            fullPath[index].token.Clear();
        }
    }

    public bool CanMove(int diceValue)
    {
        int tempPosition = _pathPosition + diceValue;

        if (tempPosition >= fullPath.Count)
        {
            return false;
        }
        else if (tempPosition >= _level2Index)
        {
            if (GameManager.instance.players[_teamId - 1].hasKill)
                return true;
            else
                return false;
        }
        else
            return true;
    }

    //public bool CanKill(int teamId, int diceValue)
    //{
    //    int tempPosition = _pathPosition + diceValue;

    //    //No kill if the tile is safe
    //    if ((tempPosition >= fullPath.Count) || fullPath[tempPosition].isSafe)
    //    {
    //        return false;
    //    }

    //    //TO-DO: Logic to find killable token when more than 1 token in a tile
    //    if (fullPath[tempPosition].GetIsTaken() && fullPath[tempPosition].token.Count == 2)
    //    {            
    //        if (teamId == fullPath[tempPosition].token[1]._teamId)
    //            return false;
    //        else
    //            return true;
    //    }
    //    else
    //        return false;
    //}

    //Initial Move
    IEnumerator MoveIn()
    {
        if (_isMoving)
        {
            yield break;
        }
        _isMoving = true;
        while (_steps > 0)
        {
            Vector3 nextPosition = fullPath[_pathPosition].gameObject.transform.position;
            while (MoveToNextTile(nextPosition, _moveSpeed))
            {
                yield return null;
            }
            yield return new WaitForSeconds(_waitDuration);
            _steps--;
            _doneSteps++;
        }        
        photonView.RPC("UpdateTile", RpcTarget.All, fullPath[_pathPosition].tileId, "Add");
        fullPath[_pathPosition].TileLayout();
        GameManager.instance.state = GameManager.GameStates.DONE;
        _isMoving = false;
    }


    IEnumerator Move(int diceValue)
    {
        if (_isMoving)
        {
            yield break;
        }
        _isMoving = true;
        _currentTilePosition = _pathPosition;

        //Movement on token
        while (_steps > 0)
        {
            _pathPosition++;

            Vector3 nextPosition = fullPath[_pathPosition].gameObject.transform.position;            
            //TO-DO if not wanted to hop
            while (MoveToNextTile(nextPosition, _moveSpeed)) //(MoveToNextTile(nextPosition, _moveSpeed))
            {
                yield return null;
            }
            fullPath[_currentTilePosition].TileLayout();
            yield return new WaitForSeconds(_waitDuration);            
            _steps--;
            _doneSteps++;
        }
        _targetTilePosition = _pathPosition;        

        //Remove token from current tile and set tile status (before start)
        photonView.RPC("UpdateTile", RpcTarget.All, fullPath[_currentTilePosition].tileId, "Remove");

        //Check if token reached destination        
        if (_targetTilePosition == fullPath.Count)
        {
            _isActive = false;
            _isComplete = true;
            GameManager.instance.state = GameManager.GameStates.DONE;
            _isMoving = false;
            yield break;
        }

        //Check the current tile if safe. If not safe, check if there is an opponent token
        //If opponent token exist, return them to base
        if (!fullPath[_targetTilePosition].isSafe)
        {
            if (fullPath[_targetTilePosition].GetIsTaken() && fullPath[_targetTilePosition].token.Count >= 1)
            {
                //Actual Kill
                if (fullPath[_targetTilePosition].token.Count(x => x._teamId != _teamId) > 0)
                {
                    int tokenCount = fullPath[_targetTilePosition].token.Count;
                    for (int i = 0; i < tokenCount; i++)
                    {
                        if (fullPath[_targetTilePosition].token[0]._teamId != _teamId)
                        {
                            Kill(fullPath[_targetTilePosition].tileId, 0); //Index will always be 0 as the item will be removed from the list in Kill method                            
                            GameManager.instance.players[_teamId - 1].hasKill = true;
                        }
                    }
                    photonView.RPC("UpdateTile", RpcTarget.All, fullPath[_targetTilePosition].tileId, "Add");
                    GameManager.instance.state = GameManager.GameStates.ROLL_DICE;
                    _isMoving = false;
                    yield break;
                }
            }
        }
        else
        {
            if (fullPath[_targetTilePosition].GetIsTaken())
            {
                //TO-DO: Need to revisit. Move based on the direction of the token
                for (int i = 0; i < fullPath[_targetTilePosition].token.Count; i++)
                {
                    transform.position += new Vector3(0.1f, 0, 0);
                }
            }
        }
        photonView.RPC("UpdateTile", RpcTarget.All, fullPath[_targetTilePosition].tileId, "Add");
        fullPath[_targetTilePosition].TileLayout();

        //TO-DO: check if the current tile has more than 1 token. Check who occupies it. Return if only one opponenet token is available

        GameManager.instance.state = GameManager.GameStates.DONE;

        _isMoving = false;
    }

    [PunRPC]
    void UpdateTile(int tileId, string operation)
    {
        int index = fullPath.FindIndex(x => x.tileId == tileId);
        if (operation == "Add")
        {
            fullPath[index].token.Add(this); 
            fullPath[index].SetIsTaken(true);
        }
        else if (operation == "Remove")
        {
            fullPath[index].token.Remove(this);
            if (fullPath[index].token.Count > 0)
                fullPath[index].SetIsTaken(true);
            else
                fullPath[index].SetIsTaken(false);
        }        
    }

    void Kill(int tileId, int tokenIndex)
    {
        GameManager.instance.SetTurnPossible(false);
        int index = fullPath.FindIndex(x => x.tileId == tileId);
        fullPath[index].token[tokenIndex].photonView.RPC("ReturnToBase", RpcTarget.All); // ReturnToBase(fullPath[tilePosition].token[tokenIndex]);
        photonView.RPC("UpdateTile", RpcTarget.All, tileId, "Remove");
        
        GameManager.instance.SetTurnPossible(true);
    }

    [PunRPC]    
    public void ReturnToBase()
    {
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {        
        _pathPosition = 0;
        _isActive = false;
        _doneSteps = 0;
                
        while (MoveToNextTile(basePosition, _moveSpeed))
        {
            yield return null;
        }        
    }

    [PunRPC]
    void Complete()
    {

    }

    [PunRPC]
    void UpdateUI()
    {

    }

    public void StartMove(int diceValue)
    {
        DiceValue.RemoveValue(diceValue);
        DiceValue.diceImageList.Remove(diceValue);
        _steps = diceValue;
        StartCoroutine(Move(diceValue));
    }

    public void LeaveBase()
    {
        DiceValue.RemoveValue(1);
        DiceValue.diceImageList.Remove(1);
        _steps = 1;
        _isActive = true;
        _pathPosition = 0;
        StartCoroutine(MoveIn());
    }


    ////TO-DO: Remove speed parameter
    bool MoveToNextTile(Vector3 nextPosition, float speed)
    {
        return nextPosition != (transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime));
    }

    //bool HopToNextTile(Vector3 startPosition, Vector3 nextPosition, float speed)
    //{
    //    cTime += speed * Time.deltaTime;
    //    Vector3 currentPosition = Vector3.Lerp(startPosition, nextPosition, cTime);
    //    currentPosition.y += amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);
    //    return nextPosition != (transform.position = Vector3.Lerp(transform.position, currentPosition, cTime));
    //}

    ////TO-DO: Add auto move; Enable selector again?
    private void OnMouseDown()
    {
        if (photonView.IsMine)
        {
            _diceValueContainer.SetShowSelector(false);
            Debug.Log("Mouse Clicked");
            if (_isSelectable)
            {
                Debug.Log("Moveable count :: " + movableValues.Count);
                //Check if more than 1 selectable value
                switch (movableValues.Count)
                {
                    case 0:
                        break;
                    case 1:                        
                        MoveSelected(movableValues[0]);
                        break;
                    default:
                        _diceValueContainer.UpdateContainer(movableValues);
                        break;
                }
            }
        }
        else
            Debug.Log("Photonview not us");        
    }

    //Selection from the list
    public void OnSelect(int value)
    {
        StartMove(value);
    }

    private void MoveSelected(int diceValue)
    {
        if (_hasTurn)
        {
            if (!_isActive)
            {
                LeaveBase();
            }
            else
            {
                StartMove(diceValue);
            }
            GameManager.instance.DeactivateSelector();
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("Trigger Stay Triggered");
    //    if (other.transform == this.transform)
    //    {
    //        float x = other.transform.position.x;
    //        other.transform.position = new Vector3((x + 0.25f), other.transform.position.y, other.transform.position.z);
    //    }
    //}
}
