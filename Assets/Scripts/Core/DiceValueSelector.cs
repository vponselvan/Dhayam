using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceValueSelector : MonoBehaviour
{
    [SerializeField] private Button _diceValue;
    [SerializeField] private GameObject _container;
    private List<Button> _buttonContainer = new List<Button>();
    //public Text _sampleText;
    private List<Button> _diceValues = new List<Button>();
    private Transform _selectorPosition;
    private bool _showSelector = false;
    private int _selectedValue;
    //private Vector3 _position;

    //private void Start()
    //{
    //    GameObject parentGo = transform.parent.gameObject;
    //    _selectorPosition = parentGo.transform;
    //}
    void Update()
    {
        
        //Vector3 selectorPosition = Camera.main.WorldToScreenPoint(_selectorPosition.position);
        //_container.transform.position = selectorPosition;
        //_sampleText.transform.position = selectorPosition;
        _container.gameObject.SetActive(_showSelector);
    }

    public void UpdateContainer(List<int> values)
    {
        ClearAll();
        //_position = new Vector3(-50f, 0f, 0f);
        //IEnumerable<int> distinctValues = values.Distinct();
        foreach (var value in values)
        {
            //Debug.Log("Selector Value :: " + value);
            Button valueButton = Instantiate(_diceValue, _container.transform);
            //valueButton.transform.localPosition = _position;
            TextMeshProUGUI valueText = valueButton.GetComponentInChildren<TextMeshProUGUI>();
            valueText.text = value.ToString();            
            _buttonContainer.Add(valueButton);
            //_position.x += 25;
        }
        _showSelector = true;
        _container.gameObject.SetActive(_showSelector);
        Debug.Log("Show Selectot state :: " + _showSelector);
    }

    void ClearAll()
    {
        foreach (Transform child in _container.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void SetShowSelector(bool on)
    {
        _showSelector = on;
    }

    public bool GetShowSelector()
    {
        return _showSelector;
    }

    public void SetSelectedValue(int value)
    {
        _selectedValue = value;
    }

    public int GetSelectedValue()
    {
        return _selectedValue;
    }
}
