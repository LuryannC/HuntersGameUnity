using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureAssetManager : MonoBehaviour
{
    public Texture2D locationIcon;
    public Texture2D artIcon;
    public Texture2D busStopIcon;

    public Mesh locationMesh;
    public Mesh artMesh;
    public Mesh busStopMesh;
    
    // Singleton instance
    public static TreasureAssetManager Instance { get; private set; }
    
    private void Awake()
    {
        // Ensure there is only one instance of the asset manager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
