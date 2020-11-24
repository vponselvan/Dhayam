using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{

    private bool _isRoll = false;

    private Animator _rollAnimator;
    [SerializeField]
    private List<Sprite> _diceSprites;
    [SerializeField]
    private GameObject _valueContainer;
    //private Image _diceFacePrefab;
    //private List<Image> _container = new List<Image>();
    private GameObject _displayDiceValue;
    private Button _rollButton;

    private int _animatorRollId;
    private int _animatorFaceId;
    private int _diceValue;
    private int[] _reRolls = new int[2] { 1, 6 };
    private float _rollDuration = 0.5f;
    private Vector3 _imagePosition = new Vector3(-150f, 0f, 0f);
    private Vector2 _imageSize = new Vector2(20, 20);
    

    private void Awake()
    {
        _rollAnimator = GetComponent<Animator>();
        if (_rollAnimator == null)
        {
            Debug.Log("Roll Animation is null");
        }
        else
        {
            _animatorRollId = Animator.StringToHash("rollTrigger");
            _animatorFaceId = Animator.StringToHash("diceValue");
        }

        _rollButton = GetComponent<Button>();
        if (_rollButton == null)
        {
            Debug.Log("Dice button is null");
        }

        //_displayDiceValue = transform.parent.transform.Find("DiceDisplay").gameObject;
        //if (_displayDiceValue == null)
        //{
        //    Debug.Log("Dice Display is null");
        //}

    }

    public void RollDice()
    {
        _rollAnimator.SetTrigger(_animatorRollId);        
        StartCoroutine(Roll());            
    }

    IEnumerator Roll()
    {
        _diceValue = Random.Range(1, 7);        
        _rollAnimator.SetInteger(_animatorFaceId, _diceValue);        
        yield return new WaitForSeconds(_rollDuration);
        _rollButton.interactable = false;
        DiceValue.diceImageList.Add(_diceValue);
        //Add it to the dice value dictionary
        DiceValue.AddValue(_diceValue);
        DisplayDiceImages();

        if (ReRoll(_diceValue))
        {
            GameManager.instance.state = GameManager.GameStates.ROLL_DICE;
        }
        else
        {

            GameManager.instance.PlayerMove();
        }
    }

    public void DisplayDiceImages()
    {
        ClearAll();
        //_imagePosition.x = -150f;

        for (int i = 0; i < DiceValue.diceImageList.Count; i++)
        {
            GameObject imageGo = new GameObject();
            Image _containerImg = imageGo.AddComponent<Image>();
            _containerImg.rectTransform.sizeDelta = _imageSize;
            switch (DiceValue.diceImageList[i])
            {
                case 1:
                    _containerImg.sprite = _diceSprites[0];

                    break;
                case 2:
                    _containerImg.sprite = _diceSprites[1];
                    break;
                case 3:
                    _containerImg.sprite = _diceSprites[2];
                    break;
                case 4:
                    _containerImg.sprite = _diceSprites[3];
                    break;
                case 5:
                    _containerImg.sprite = _diceSprites[4];
                    break;
                case 6:
                    _containerImg.sprite = _diceSprites[5];
                    break;
                default:
                    break;
            }
            _containerImg.transform.SetParent(_valueContainer.transform);
           // _containerImg.transform.localPosition = _imagePosition;
           // _container.Add(_containerImg);
            //_imagePosition.x += 50f;
        }        
    }

    public bool ReRoll(int diceValue)
    {
        for (int i = 0; i < _reRolls.Length; i++)
        {
            if (diceValue == _reRolls[i])
                return true;
        }
        return false;
    }

    void ClearAll()
    {
        foreach (Transform child in _valueContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
