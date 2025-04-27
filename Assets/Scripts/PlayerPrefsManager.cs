using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsManager : MonoBehaviour
{
    public InputField nameInput;
    public Text placesVisitedText;
    public Text challengesFinishedText;
    public Text displayText;
    
    public static PlayerPrefsManager Instance { get; private set; }
    
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
    
    void Start()
    {
        LoadData();
    }

    public void AddToPlacesVisited()
    {
        int currentValue = PlayerPrefs.GetInt("PlacesVisited", 0);
        PlayerPrefs.SetInt("PlacesVisited", currentValue + 1);
        Debug.Log("AddToPlacesVisited() - Added to place visited");
    }
    
    public void AddToChallengesFinished()
    {
        int currentValue = PlayerPrefs.GetInt("challengesFinished", 0);
        PlayerPrefs.SetInt("challengesFinished", currentValue + 1);
        Debug.Log("AddToPlacesVisited() - Added to Challenges Finished");
    }

    public void SavePlayerName()
    {
        string playerName = nameInput.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        Debug.Log($"SavePlayerName() - Player Name Saved! ({playerName})");
    }
    
    
    public void SaveData()
    {
        string playerName = nameInput.text;
        
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("PlacesVisited", 0);
        PlayerPrefs.Save();

        Debug.Log("Data Saved!");
    }
    
    public void LoadData()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        int placesVisited = PlayerPrefs.GetInt("PlacesVisited", 0);
        int challengesFinished = PlayerPrefs.GetInt("challengesFinished", 0);
        
        Debug.Log("LoadData() - Player Name: " + playerName);
        Debug.Log("LoadData() - Places Visited: " + placesVisited);
        Debug.Log("LoadData() - Challenges Finished: " + challengesFinished);

        nameInput.text = playerName;
        placesVisitedText.text = placesVisited.ToString();
        challengesFinishedText.text = challengesFinished.ToString();

        //displayText.text = $"Welcome back, {playerName}! High Score: {highScore}";
    }

    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All data cleared.");
    }
}
