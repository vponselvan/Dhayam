using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceValueButton : MonoBehaviour
{
    private int _valueText;
    private DiceValueSelector _container;
    private Token _token;
    // Start is called before the first frame update
    void Start()
    {
        _valueText = Int32.Parse(this.GetComponentInChildren<TextMeshProUGUI>().text);
        _container = transform.parent.parent.GetComponentInChildren<DiceValueSelector>();
        if (_container == null)
        {
            Debug.Log("Dice Value Button:: Dice Value Container is null");
        }
        _token = transform.parent.parent.parent.GetComponent<Token>();
        if (_token == null)
        {
            Debug.Log("Dice Value Button:: token is null");
        }
    }

    public void OnSelection()
    {
        _token.OnSelect(_valueText);     
        _container.SetShowSelector(false);
        Destroy(this.gameObject, 0.2f); //Destroy selected button after 0.2s                
    }
}
