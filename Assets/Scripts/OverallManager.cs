using UnityEngine;
using Mapbox.Unity.Location;
using UnityEngine.SceneManagement;

public class OverallManager : MonoBehaviour
{
    public AbstractLocationProvider locationProvider = null;
    public GameObject RaycastPlane;
    
    private void Start()
    {
        if (null == locationProvider)
        {
            locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
        }
    }
    
    public void StartChallenge()
    {
        LocationsManager.Instance.RetrieveSpawnableLocations();
    }

    public void OnChallengeFinished()
    {
        PlayerPrefsManager.Instance.AddToChallengesFinished();
        Debug.Log("You have finished the challenge!");
    }

    private void ResetGame()
    {
        
    }
}
