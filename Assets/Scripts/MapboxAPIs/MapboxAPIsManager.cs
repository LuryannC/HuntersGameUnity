using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using UnityEngine.Serialization;

public class MapboxAPIsManager : MonoBehaviour
{
    [FormerlySerializedAs("mapboxFoodFinder")] [SerializeField] public MapboxFinder mapboxFinder;
    
    // Player's location
    public AbstractLocationProvider locationProvider = null;
    private Location _lastPlayerLoc;
    [SerializeField] private double _radiusToCheck = 50.0;
    
    public static MapboxAPIsManager Instance { get; private set; } 
    
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

    private void Start()
    {
        if (null == locationProvider)
        {
            locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
        }
    }

    private void LateUpdate()
    {
        if (locationProvider != null)
        {
            if (!IsWithinRadius(_lastPlayerLoc.LatitudeLongitude, locationProvider.CurrentLocation.LatitudeLongitude, _radiusToCheck))
            {
                _lastPlayerLoc = locationProvider.CurrentLocation;
                //mapboxFinder.FindNearbyFoodShop(_lastPlayerLoc, _radiusToCheck);
            }
        }
    }

    public static double CalculateDistance(Vector2d loc1, Vector2d loc2)
    {
        double lat1 = loc1.x * Mathf.Deg2Rad;
        double lon1 = loc1.y * Mathf.Deg2Rad;
        double lat2 = loc2.x * Mathf.Deg2Rad;
        double lon2 = loc2.y * Mathf.Deg2Rad;

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = Mathf.Pow(Mathf.Sin((float)dLat / 2), 2) +
                   Mathf.Cos((float)lat1) * Mathf.Cos((float)lat2) *
                   Mathf.Pow(Mathf.Sin((float)dLon / 2), 2);
        double c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt(1 - (float)a));

        return 6371.0 * c; // Distance in kilometers
    }
    
    public static bool IsWithinRadius(Vector2d center, Vector2d player, double radiusKm)
    {
        double distance = CalculateDistance(center, player);
        return distance <= radiusKm;
    }
}
