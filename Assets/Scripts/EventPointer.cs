using System;
using System.Collections;
using System.Collections.Generic;
using GeoCoordinatePortable;
using UnityEngine;
// MapBox Library
using Mapbox.Examples;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using UnityEngine.UI;

public class EventPointer : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float amplitude = 2.0f;
    [SerializeField] private float frequency = 0.50f;		
    [SerializeField] private float UpPositionOffset;

    private LocationStatus playerLocation;
    public Vector2d eventPos;

    private AbstractLocationProvider _locationProvider = null;
    private Location currLoc;
    

    // This should live inside the prefab where I store the data for the collectable treasure
    [SerializeField] private float interactableDistance = 5.0f;
    void Start()
    {
        if (null == _locationProvider)
        {
            _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
        }
    }

    void Update()
    {
        FloatAndRotatePointer();
    }

    private void FloatAndRotatePointer()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude) + UpPositionOffset, transform.position.z);
        
    }

    private void OnMouseDown()
    {
        currLoc = _locationProvider.CurrentLocation;
        
        var currentPlayerLocation = new GeoCoordinatePortable.GeoCoordinate(currLoc.LatitudeLongitude.x, currLoc.LatitudeLongitude.y);
        var eventLocation = new GeoCoordinatePortable.GeoCoordinate(eventPos.x, eventPos.y);
        var distance = currentPlayerLocation.GetDistanceTo(eventLocation);
        if (distance < interactableDistance)
        {
            Debug.Log("Collected");
            LocationsManager.Instance.RemoveSpawnedTreasure(gameObject);
            PlayerPrefsManager.Instance.AddToPlacesVisited();
            Destroy(gameObject);
        }
        Debug.Log("Distance is: " + distance);
    }
}
