using System;
using System.Collections;
using Mapbox.Map;

namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

	public class SpawnOnMap : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		string[] _locationStrings;
		Vector2d[] _locations;

		[SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		GameObject _markerPrefab;

		List<GameObject> _spawnedObjects;

		public double spawnRadius = 0.0;

		List<string[]> locationsToSpawn;
		private List<int> indexesToRemove;

		#region QuadTree
		
		[SerializeField]
		private int treeCapacity;
		
		[SerializeField]
		List<Vector2> nearbyObjects;

		[SerializeField] 
		private float searchSize;
		
		private QuadTree tree;
		
		#endregion

		#region HashMap
		
		[System.Serializable]
		public class TreasurePointEntry
		{
			public UnwrappedTileId gridKey;
			public List<GameObject> treasureObjects;
		}

		[SerializeField]
		private List<TreasurePointEntry> _treasurePointList = new List<TreasurePointEntry>();

		public void AddTreasurePoint(Vector2d coordinates, GameObject obj)
		{
			if (obj == null)
			{
				Debug.LogWarning("AddTreasurePoint() - Cannot add null treasure point.");
				return;
			}
			
			UnwrappedTileId gridKey = Conversions.LatitudeLongitudeToTileId(coordinates.x, coordinates.y, Mathf.FloorToInt(_map.Zoom));

			// Check if the key already exists in the list
			TreasurePointEntry existingEntry = _treasurePointList.Find(entry => entry.gridKey.Equals(gridKey));
			if (existingEntry == null)
			{
				// If not, create a new entry and add it to the list
				_treasurePointList.Add(new TreasurePointEntry { gridKey = gridKey, treasureObjects = new List<GameObject> { obj } });
			}
			else
			{
				// If exists, just add the new object to the existing list
				existingEntry.treasureObjects.Add(obj);
			}
		}

		#endregion

		void Start()
		{
			// tree = new QuadTree(new Rect(0, 0, _spawnScale, _spawnScale), treeCapacity)


			// locationsToSpawn = new List<string[]>();
			// for (int i = 0; i < 5; i++)
			// {
			// 	var latitudeLongitude = MapboxAPIsManager.Instance.locationProvider.CurrentLocation.LatitudeLongitude;
			// 	var newRandomCoordinate = GetRandomCoordinate(latitudeLongitude.y, latitudeLongitude.x, spawnRadius);
			// 	
			// 	
			// }
			


			StartCoroutine(RunAfterStart());
		}
		
		IEnumerator RunAfterStart()
		{
			// Wait until the next frame
			yield return new WaitForSeconds(2.0f);
			// Code here will run after the first frame update
			
			_locations = new Vector2d[_locationStrings.Length];
			_spawnedObjects = new List<GameObject>();
			// for (int i = 0; i < _locationStrings.Length; i++)
			for (int i = 0; i < 5; i++)
			{
				var locationString = _locationStrings[i];
				var latitudeLongitude = MapboxAPIsManager.Instance.locationProvider.CurrentLocation.LatitudeLongitude;
				Debug.Log("RunAfterStart() - Latitude and Longitude: " + latitudeLongitude);
				
				var newRandomCoordinate = GetRandomCoordinate(latitudeLongitude.x, latitudeLongitude.y, spawnRadius);
				Debug.Log("RunAfterStart() - New Random Coordinates: " + newRandomCoordinate);
				
				// _locations[i] = Conversions.StringToLatLon(locationString);
				// Vector2d convertedLatLon = Conversions.StringToLatLon(newRandomCoordinate);
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

			for (int i = 0; i < _locationStrings.Length; i++)
			{
				var locationString = _locationStrings[i];
				var location = _locations[i];
				Vector3 treeVector = _map.GeoToWorldPositionXZ(location);
				Vector2 finalVector = new Vector2(treeVector.x, treeVector.z);
				// tree.Insert(finalVector);
			}
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
				if (_spawnedObjects[i] == null)
				{
					indexesToRemove.Add(i);
					_spawnedObjects.Remove(_spawnedObjects[i]);
					continue;
				}
					
				var spawnedObject = _spawnedObjects[i];
				var location = _locations[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				
				// Vector3 treeVector = _map.GeoToWorldPositionXZ(location);
				// Vector2 finalVector = new Vector2(treeVector.x, treeVector.z);
				// Rect searchArea = new Rect(finalVector.x, finalVector.y, searchSize, searchSize);
				// nearbyObjects = tree.Query(searchArea);
			}
		}

		// private void OnDrawGizmos()
		// {
		// 	if (tree != null)
		// 	{
		// 		tree.DrawGizmos();
		// 	}
		// }

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
}