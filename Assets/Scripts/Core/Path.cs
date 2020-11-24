using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    Transform[] childTiles;

    public List<Transform> childTileList = new List<Transform>();
    
    void Start()
    {
        CreateTiles();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        CreateTiles();

        for (int i = 0; i < childTileList.Count; i++)
        {
            Vector3 currentPosition = childTileList[i].position;
            if (i > 0)
            {
                Vector3 prevPosition = childTileList[i - 1].position;
                Gizmos.DrawLine(prevPosition, currentPosition);
            }
        }
    }

    void CreateTiles()
    {
        childTileList.Clear();
        childTiles = GetComponentsInChildren<Transform>();

        foreach (Transform child in childTiles)
        {
            if (child != this.transform)
            {
                childTileList.Add(child);
            }
        }
    }

    public int GetPosition(Transform tileTransform)
    {
        //Debug.Log("Child List count :" + childTileList.IndexOf(tileTransform));
        //Debug.Log("Child List count :" + childTileList.IndexOf(tileTransform));
        for (int i = 0; i < childTileList.Count; i++)
        {
            if (tileTransform.position == childTileList[i].transform.position)
            {
                return i;
            }
        }
        //return childTileList.IndexOf(tileTransform);
        return -1;
    }
}
