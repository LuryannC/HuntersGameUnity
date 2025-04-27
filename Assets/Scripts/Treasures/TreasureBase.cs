using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBase : MonoBehaviour
{
    public TreasureStructure treasureStructure;
    
    private void Awake()
    {
        // Call the method to set the icon and mesh based on the treasureType
        SetTreasureIconAndMesh();
    }

    private void SetTreasureIconAndMesh()
    {
        // Get the instance of the TreasureAssetManager
        TreasureAssetManager assetManager = TreasureAssetManager.Instance;
        
        if (assetManager == null)
        {
            Debug.LogError("SetTreasureIconAndMesh() - TreasureAssetManager is not set in the scene!");
            return;
        }
        
        // Set the treasure icon and mesh based on the treasureType enum
        switch (treasureStructure.treasureType)
        {
            case TreasureType.Location:
                treasureStructure.treasureIcon = assetManager.locationIcon;
                treasureStructure.treasureMesh = assetManager.locationMesh;
                break;

            case TreasureType.Art:
                treasureStructure.treasureIcon = assetManager.artIcon;
                treasureStructure.treasureMesh = assetManager.artMesh;
                break;

            case TreasureType.BusStop:
                treasureStructure.treasureIcon = assetManager.busStopIcon;
                treasureStructure.treasureMesh = assetManager.busStopMesh;
                break;

            case TreasureType.None:
            default:
                treasureStructure.treasureIcon = null;
                treasureStructure.treasureMesh = null;
                break;
        }
    }
}
