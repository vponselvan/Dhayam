using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceFace : MonoBehaviour
{
    private bool _onGround;
    [SerializeField] private int _sideValue;
    private string _stationTag = "DiceStation";

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == _stationTag)
        {
            _onGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == _stationTag)
        {
            _onGround = false;
        }
    }

    public bool OnGround()
    {
        return _onGround;
    }

    public int GetFaceValue()
    {
        return _sideValue;
    }
}
