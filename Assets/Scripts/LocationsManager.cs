using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Map;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

[System.Serializable]

public class LocationsManager : MonoBehaviour
{
    [SerializeField] private OverallManager _overallManager;

    [SerializeField] AbstractMap _map;

    public MapboxFinder MapboxFinder;

    [SerializeField] [Geocode] string[] _locationStrings;
    Vector2d[] _locations;

    private List<Vector2d> _desiredLocations;

    [SerializeField] float _spawnScale = 100f;

    [SerializeField] GameObject _markerPrefab;

    List<GameObject> _spawnedObjects;

    private double spawnRadius;

    #region HashMap

    public class TreasurePointEntry
    {
        public UnwrappedTileId gridKey;
        public List<GameObject> treasureObjects;
    }

    [SerializeField] private List<TreasurePointEntry> _treasurePointList = new List<TreasurePointEntry>();

    public void AddTreasurePoint(Vector2d coordinates, GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("AddTreasurePoint() - Cannot add null treasure point.");
            return;
        }

        UnwrappedTileId gridKey =
            Conversions.LatitudeLongitudeToTileId(coordinates.x, coordinates.y, Mathf.FloorToInt(_map.Zoom));

        // Check if the key already exists in the list
        TreasurePointEntry existingEntry = _treasurePointList.Find(entry => entry.gridKey.Equals(gridKey));
        if (existingEntry == null)
        {
            // If not, create a new entry and add it to the list
            _treasurePointList.Add(new TreasurePointEntry
                { gridKey = gridKey, treasureObjects = new List<GameObject> { obj } });
        }
        else
        {
            // If exists, just add the new object to the existing list
            existingEntry.treasureObjects.Add(obj);
        }
    }

    #endregion

    public static LocationsManager Instance { get; private set; }

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

    IEnumerator SpawnTreasures()
    {
        int amountToSpawn = UIManager.Instance.DesiredAmountOfTreasuresToCollect;

        _locations = new Vector2d[amountToSpawn];
        _spawnedObjects = new List<GameObject>();

        for (int i = 0; i < _desiredLocations.Count; i++)
        {
            var latitudeLongitude = MapboxAPIsManager.Instance.locationProvider.CurrentLocation.LatitudeLongitude;
            Debug.Log("RunAfterStart() - Latitude and Longitude: " + latitudeLongitude);

            var newRandomCoordinate = GetRandomCoordinate(latitudeLongitude.x, latitudeLongitude.y, spawnRadius);
            Debug.Log("RunAfterStart() - New Random Coordinates: " + newRandomCoordinate);

            Vector2d convertedLatLon = new Vector2d(newRandomCoordinate.Item1, newRandomCoordinate.Item2);
            _locations[i] = convertedLatLon;
            Debug.Log("RunAfterStart() - Converted Lat and Lon: " + convertedLatLon);

            var instance = Instantiate(_markerPrefab);
            instance.GetComponent<EventPointer>().eventPos = _locations[i];
            instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
            instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            _spawnedObjects.Add(instance);

            AddTreasurePoint(_locations[i], instance);
            yield return new WaitForSeconds(0.5f);
        }

        _overallManager.RaycastPlane.SetActive(!_overallManager.RaycastPlane.activeSelf);
        yield return null;
    }

    private void Update()
    {
        if (_spawnedObjects == null)
        {
            return;
        }

        int count = _spawnedObjects.Count;

        for (int i = 0; i < count; i++)
        {
            var spawnedObject = _spawnedObjects[i];
            var location = _locations[i];
            spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
            spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
        }
    }

    public void RetrieveSpawnableLocations()
    {
        _desiredLocations = new List<Vector2d>();
        int amountToSpawn = UIManager.Instance.DesiredAmountOfTreasuresToCollect;

        // Grab radius from UI. Terrible but it is a prototype.
        spawnRadius = UIManager.Instance.DesiredRadiusToCollect;

        MapboxFinder.FindLocations(_overallManager.locationProvider.CurrentLocation, spawnRadius, amountToSpawn,
            CheckRetrievedLocations);
    }

    private void CheckRetrievedLocations(List<Vector2d> coordinates)
    {
        int amountToSpawn = UIManager.Instance.DesiredAmountOfTreasuresToCollect;


        if (coordinates == null || coordinates.Count == 0)
        {
            Debug.LogError("The coordinates list is empty.");
            // return;
        }
        else
        {
            Random random = new Random();
            int randomIndex = random.Next(coordinates.Count);

            _desiredLocations.Add(coordinates[randomIndex]);
        }

        if (_desiredLocations.Count == amountToSpawn)
        {
            StartCoroutine(SpawnTreasures());
        }
    }

    public void RemoveSpawnedTreasure(GameObject gameObject)
    {
        _spawnedObjects.Remove(gameObject);
        
        if (HasFinishedChallenge())
        {
            _overallManager.OnChallengeFinished();
        }
    }

    private bool HasFinishedChallenge()
    {
        return _spawnedObjects.Count <= 0;
    }

public static (double, double) GetRandomCoordinate(double inLatitude, double inLongitude, double inRadius)
    {
        System.Random random = new System.Random();
			
        double latRad = inLatitude * Mathf.Deg2Rad;
        double lonRad = inLongitude * Mathf.Deg2Rad;
			
        double radiusInDegrees = inRadius / 111320f;
			
        // Get a random distance and a random angle.
        double u = random.NextDouble();
        double v = random.NextDouble();
        double w = radiusInDegrees * Math.Sqrt(u);
        double t = 2 * Math.PI * v;
			
        // Get the x and y delta values.
        double x = w * Math.Cos(t);
        double y = w * Math.Sin(t);
			
        // Compensate the x value.
        double new_x = x / Math.Cos(inLatitude *Mathf.Deg2Rad);
			
        double foundLatitude;
        double foundLongitude;
			
        foundLatitude = inLatitude + y;
        foundLongitude = inLongitude + new_x;
			
        // var finalString = foundLatitude + "," + foundLongitude;
        //
        // return finalString;
        return (foundLatitude, foundLongitude);
    }
}
