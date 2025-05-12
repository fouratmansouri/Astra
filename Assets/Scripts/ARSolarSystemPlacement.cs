using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARSolarSystemPlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject solarSystemPrefab;  // Assign your solar system prefab in the inspector

    [SerializeField]
    private Button placeSolarSystemButton;  // Assign a UI button in the inspector

    [SerializeField]
    private GameObject planeMarkerPrefab;  // Optional visual indicator for detected planes

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
    private GameObject spawnedSolarSystem;
    
    private bool solarSystemPlaced = false;

    void Awake()
    {
        // Get required AR components
        raycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        // Setup the button
        if (placeSolarSystemButton)
        {
            placeSolarSystemButton.onClick.AddListener(PlaceSolarSystem);
            placeSolarSystemButton.gameObject.SetActive(false); // Hide button initially
        }
        else
        {
            Debug.LogError("Place Solar System Button not assigned in the inspector!");
        }
    }

    void Start()
    {
        // Make sure planes are being detected
        if (planeManager != null)
        {
            planeManager.planesChanged += OnPlanesChanged;
        }
    }

    void OnDisable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // When planes are detected, show the placement button
        if (args.added.Count > 0 && !placeSolarSystemButton.gameObject.activeSelf && !solarSystemPlaced)
        {
            placeSolarSystemButton.gameObject.SetActive(true);
        }

        // Optionally, you could place visual indicators on each plane
        if (planeMarkerPrefab != null)
        {
            foreach (var plane in args.added)
            {
                // Create a marker for each new plane
                GameObject marker = Instantiate(planeMarkerPrefab, plane.transform);
                marker.transform.localPosition = Vector3.zero;
                marker.transform.localRotation = Quaternion.identity;
                
                // Scale the marker to match the plane size
                marker.transform.localScale = new Vector3(plane.size.x, 1, plane.size.y);
            }
        }
    }

    private void PlaceSolarSystem()
    {
        // Get center of screen
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        
        // Raycast from center of screen
        if (raycastManager.Raycast(screenCenter, hitResults, TrackableType.PlaneWithinPolygon))
        {
            // Get the first hit pose
            Pose hitPose = hitResults[0].pose;
            
            // If we already placed a solar system, destroy it
            if (spawnedSolarSystem != null)
            {
                Destroy(spawnedSolarSystem);
            }
            
            // Instantiate the solar system at the hit position
            spawnedSolarSystem = Instantiate(solarSystemPrefab, hitPose.position, hitPose.rotation);
            
            // Mark that we've placed the solar system
            solarSystemPlaced = true;
            
            // Hide the button after placement
            placeSolarSystemButton.gameObject.SetActive(false);
            
            // Optionally, you can disable plane detection after placement
            // planeManager.enabled = false;
        }
    }

    // Call this when your "AR Solar System" button is clicked
    public void StartARExperience()
    {
        // Enable plane detection
        if (planeManager != null)
        {
            planeManager.enabled = true;
        }
        
        // Reset state
        solarSystemPlaced = false;
        
        // Remove any previously spawned solar system
        if (spawnedSolarSystem != null)
        {
            Destroy(spawnedSolarSystem);
            spawnedSolarSystem = null;
        }
        
        // Button will be shown when planes are detected
    }
}