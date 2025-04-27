using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class IntStringPair
{
    public int key;
    public string value;
}

public class ArrowButton : MonoBehaviour
{
    public int buttonCurrentValue;
    [SerializeField] public List<IntStringPair> dictionaryEntries = new List<IntStringPair>();

    [SerializeField] private string _displayText;
    [SerializeField] private Text _textField;
    
    [SerializeField] private Button _leftButton;
    [SerializeField] private Text _valueField;
    [SerializeField] private Button _rightButton;
    
    [SerializeField] private UIManagerField targetField;

    private void Awake()
    {
        // Set initial display
        UpdateDisplay();
        UpdateButtonText();

        // Add listeners
        _leftButton.onClick.AddListener(OnLeftButtonClick);
        _rightButton.onClick.AddListener(OnRightButtonClick);
    }

    private void Start()
    {
        UpdateUIManager();
        UpdateDisplay();
        UpdateButtonText();
    }

    private void OnValidate()
    {
        UpdateDisplay();
        UpdateButtonText();
    }
    
    private void OnLeftButtonClick()
    {
        buttonCurrentValue--;
        if (buttonCurrentValue < 0)
            buttonCurrentValue = dictionaryEntries.Count - 1;

        UpdateDisplay();
    }

    private void OnRightButtonClick()
    {
        buttonCurrentValue++;
        if (buttonCurrentValue >= dictionaryEntries.Count)
            buttonCurrentValue = 0;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (dictionaryEntries != null && 
            buttonCurrentValue >= 0 && 
            buttonCurrentValue < dictionaryEntries.Count &&
            dictionaryEntries[buttonCurrentValue] != null)
        {
            _valueField.text = dictionaryEntries[buttonCurrentValue].value;
            UpdateUIManager();
        }
    }

    private void UpdateButtonText()
    {
        _textField.text = _displayText;
    }

    private void UpdateUIManager()
    {
        int value = Convert.ToInt32(dictionaryEntries[buttonCurrentValue].value);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetField(targetField, value);
        }
    }
}
