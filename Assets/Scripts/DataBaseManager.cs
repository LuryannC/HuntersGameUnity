using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class DataBaseManager : MonoBehaviour
{
  public static DataBaseManager Instance { get; private set; } 
  
  [SerializeField] private string serverUrl = "http://127.0.0.1";
  [SerializeField] private string serverPort = "5000";
  
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
  
  public void AddData(string name, int score)
  {
    string serverUrlCombined = serverUrl + ":" + serverPort;
    
    StartCoroutine(PostRequest($"{serverUrlCombined}/add", name, score));
  }
  
  public void GetData()
  {
    string serverUrlCombined = serverUrl + ":" + serverPort;
    
    StartCoroutine(GetRequest($"{serverUrlCombined}/get"));
  }

  IEnumerator PostRequest(string url, string name, int score)
  {
    // Instead of passing the url I can pass the jsonData as string and add to the specific place.
    string jsonData = $"{{\"name\": \"{name}\", \"score\": {score}}}";
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

    string serverUrlCombined = serverUrl + ":" + serverPort;

    using (UnityWebRequest request = new UnityWebRequest(serverUrlCombined, "POST"))
    {
      request.uploadHandler = new UploadHandlerRaw(bodyRaw);
      request.downloadHandler = new DownloadHandlerBuffer();
      request.SetRequestHeader("content-type", "application/json");

      yield return request.SendWebRequest();

      if (request.result == UnityWebRequest.Result.Success)
      {
        Debug.Log("Data added successfully!");
      }
      else
      {
        Debug.LogError($"Error: {request.error}");
      }
    }
  }

  IEnumerator GetRequest(string url)
  {
    string serverUrlCombined = serverUrl + ":" + serverPort;
    
    using (UnityWebRequest request = UnityWebRequest.Get(serverUrlCombined))
    {
      yield return request.SendWebRequest();
      
      if (request.result == UnityWebRequest.Result.Success)
      {
        Debug.Log($"Received Data: {request.downloadHandler.text}");
      }
      else
      {
        Debug.LogError($"Error: {request.error}");
      }
    }
  }
}
