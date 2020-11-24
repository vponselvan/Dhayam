using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private bool _isTaken;

    public bool isSafe;

    public int tileId;

    public List<Token> token = new List<Token>();

    private void Awake()
    {
        _isTaken = false;
    }

    public bool GetIsTaken()
    {
        return _isTaken;
    }

    public void SetIsTaken(bool isTaken)
    {
        _isTaken = isTaken;
    }

    public void TileLayout()
    {
        Debug.Log("Tile:: Token count ::" + token.Count);
        if (token.Count > 1)
        {
            //Debug.Log("Tile:: Token count ::" + token.Count);
            float spacing = 0.5f / token.Count;
            Debug.Log("Tile:: Token Spacing ::" + spacing);
            for (int i = 0; i < token.Count; i++)
            {
                //GameObject tokenGo = token[i].tr .gameObject;
                if (i < token.Count / 2)
                {
                    float x = this.transform.position.x;
                    token[i].transform.position = new Vector3((x + spacing), token[i].transform.position.y, token[i].transform.position.z);
                    Debug.Log("Tile:: Token Position in if ::" + i + " : " + token[i].transform.position);
                }
                else
                {
                    float x = this.transform.position.x;
                    token[i].transform.position = new Vector3((x - spacing), token[i].transform.position.y, token[i].transform.position.z);
                    Debug.Log("Tile:: Token Position in else ::" + i + " : " + token[i].transform.position);
                }

            }
        }
        else if (token.Count == 1)
        {
            token[0].transform.position = this.transform.position;
        }
    }

}
