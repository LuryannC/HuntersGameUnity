using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UIManagerField
{
    DesiredAmountOfTreasuresToCollect,
    DesiredRadiusToCollect
}

public class UIManager : MonoBehaviour
{
    public int DesiredAmountOfTreasuresToCollect;
    public int DesiredRadiusToCollect;
    
    public static UIManager Instance { get; private set; } 
    
    private void Awake()
    {
        // Ensure there's only one instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public void OnGameFinishedUI()
    {
        
    }
    
    public void SetField(UIManagerField field, int value)
    {
        switch (field)
        {
            case UIManagerField.DesiredAmountOfTreasuresToCollect:
                DesiredAmountOfTreasuresToCollect = value;
                break;
            case UIManagerField.DesiredRadiusToCollect:
                DesiredRadiusToCollect = value;
                break;
            // Add more fields as needed
            default:
                Debug.LogWarning($"Unhandled UIManagerField: {field}");
                break;
        }
    }
}
