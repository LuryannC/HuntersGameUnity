using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mapbox.Json.Linq;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

[System.Serializable]
public class Place
{
    public string name;
    public float longitude;
    public float latitude;
}
    
[System.Serializable]
public class PlaceList
{
    public List<Place> places = new List<Place>();
}

[System.Serializable]
public enum SearchTypes
{
    Poi,
    Restaurants,
    Park,
    Bar
}

public class MapboxFinder : MonoBehaviour
{
    [SerializeField] private string mapboxAccessToken = "MAPBOX_ACCESS_TOKEN";
    [SerializeField] private string apiUrl = "https://api.mapbox.com/geocoding/v5/mapbox.places/";

    private Location _lastPlayerLoc;
    private float _radiusToCheck;
    private float _normalizedRadius;
    
    //private List<Vector2> selectedLocations = new List<Vector2>();
    public delegate void LocationRetrievedCallback(List<Vector2d> coordinates);
    
    public void FindLocations(Location location, double radius, int amountToRetrieve, LocationRetrievedCallback callback)
    {
        _lastPlayerLoc = location;
        _radiusToCheck = (float)radius;
        _normalizedRadius = _radiusToCheck * 0.01f;

        for (int i = 0; i < amountToRetrieve; i++)
        {
            Debug.Log($"Starting iteration {i + 1}");
            StartCoroutine(GetNearbyLocations(location, radius, callback));
        }
    }

    IEnumerator GetNearbyLocations(Location location, double radius, LocationRetrievedCallback callback)
    {
        string randomSearchType = GetRandomSearchType();
        Debug.Log("GetNearbyLocations() - Search Type: " + randomSearchType);

        List<Vector2d> selectedLocations = new List<Vector2d>();
        
        float latitude = (float)location.LatitudeLongitude.x;
        float longitude = (float)location.LatitudeLongitude.y;
        
        // Define bounding box (min_lon, min_lat, max_lon, max_lat)
        float minLon = longitude - _normalizedRadius;
        float maxLon = longitude + _normalizedRadius;
        float minLat = latitude - _normalizedRadius;
        float maxLat = latitude + _normalizedRadius;
        
        //string url = $"{apiUrl}?proximity={latitude},{longitude}&access_token={mapboxAccessToken}";
        string url = $"{apiUrl}{randomSearchType}.json?bbox={minLon},{minLat},{maxLon},{maxLat}&access_token={mapboxAccessToken}";
        
        //string url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/food.json?proximity={longitude},{latitude}&radius={radius * 1000}&access_token={mapboxAccessToken}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log("Response: " + request.downloadHandler.text);
            // Debug.Log("User Coordinates: " + longitude + ", " + latitude);
            // SaveToJson(request.downloadHandler.text);
            
            // Parse JSON and extract places with latitude and longitude
            var data = JsonUtility.FromJson<MapboxResponse>(request.downloadHandler.text);
        
            foreach (var feature in data.features)
            {
                float placeLongitude = feature.geometry.coordinates[0];
                float placeLatitude = feature.geometry.coordinates[1];
            
                float distance = HaversineDistance((float)_lastPlayerLoc.LatitudeLongitude.x, (float)_lastPlayerLoc.LatitudeLongitude.y, placeLatitude, placeLongitude);
           
                //if (distance <= _radiusToCheck)
                {
                    selectedLocations.Add(new Vector2d(placeLatitude, placeLongitude));
                }
            }
            
            callback?.Invoke(selectedLocations);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    private string GetRandomSearchType()
    {
        Random random = new Random();
        Array searchTypeValues = Enum.GetValues(typeof(SearchTypes));
        SearchTypes randomSearchType = (SearchTypes)searchTypeValues.GetValue
            (random.Next(searchTypeValues.Length));
        
        switch (randomSearchType)
        {
            case SearchTypes.Poi:
                return  "poi";
            case SearchTypes.Restaurants:
                return "restaurants";
            case SearchTypes.Park:
                return "park";
            case SearchTypes.Bar:
                return "bar";
        }

        return "";
    }

    void SaveToJson(string jsonData)
    {
        // Parse JSON and extract relevant data
        PlaceList placeList = new PlaceList();
        var data = JsonUtility.FromJson<MapboxResponse>(jsonData);
        
        foreach (var feature in data.features)
        {
            float placeLongitude = feature.geometry.coordinates[0];
            float placeLatitude = feature.geometry.coordinates[1];
            
            float distance = HaversineDistance((float)_lastPlayerLoc.LatitudeLongitude.x, (float)_lastPlayerLoc.LatitudeLongitude.y, placeLatitude, placeLongitude);
           
            //if (distance <= _radiusToCheck)
            {
                Place place = new Place
                {
                    name = feature.text,
                    longitude = placeLongitude,
                    latitude = placeLatitude
                };
                placeList.places.Add(place);
            }
        }
        
        string json = JsonUtility.ToJson(placeList, true);
        File.WriteAllText(Application.persistentDataPath + "/food_shops.json", json);
        
        Debug.Log("Saved JSON to: " + Application.persistentDataPath + "/food_shops.json"); 
    }
    
    float HaversineDistance(float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6371f; // Earth radius in KM
        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return R * c; // Distance in KM
    }
    
    [System.Serializable]
    private class MapboxResponse
    {
        public Feature[] features;
    }

    [System.Serializable]
    private class Feature
    {
        public string text; // Place name
        public Geometry geometry;
    }

    [System.Serializable]
    private class Geometry
    {
        public List<float> coordinates; // [longitude, latitude]
    }
}
