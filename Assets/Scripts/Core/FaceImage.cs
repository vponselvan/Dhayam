using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceImage : MonoBehaviour
{
    private bool _isDestroy = false;
    public int faceValue = 0;

    public void Update()
    {
        if (_isDestroy)
        {
            Destroy(this.gameObject);
        }        
    }

    public void SetDestroy()
    {
        _isDestroy = true;
    }

}
