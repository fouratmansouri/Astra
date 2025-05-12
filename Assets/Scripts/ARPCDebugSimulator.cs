using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPCDebugSimulator : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private Camera arCamera;

    [Header("Debug Settings")]
    [SerializeField] private bool simulatePlanesOnPC = true;
    [SerializeField] private float planeGenerationInterval = 1f;
    [SerializeField] private float planeDistance = 2f;
    [SerializeField] private Vector2 planeSize = new Vector2(1f, 1f);
    
    [Header("Visualization")]
    [SerializeField] private bool useCustomVisualization = true;
    [SerializeField] private Material planeMaterial;
    [SerializeField] private Color planeColor = new Color(0, 0.7f, 1f, 0.5f);

    private bool isOnPC;
    private float timeSinceLastPlane;
    private List<GameObject> debugPlanes = new List<GameObject>();
    private int planeIdCounter = 0;

    private void Start()
    {
        // Auto-detect if we're on a PC (not a mobile device)
        isOnPC = Application.platform == RuntimePlatform.WindowsEditor || 
                 Application.platform == RuntimePlatform.WindowsPlayer || 
                 Application.platform == RuntimePlatform.OSXEditor ||
                 Application.platform == RuntimePlatform.OSXPlayer ||
                 Application.platform == RuntimePlatform.LinuxEditor ||
                 Application.platform == RuntimePlatform.LinuxPlayer;

        // Auto-assign references if not set in the inspector
        if (arSession == null)
            arSession = FindObjectOfType<ARSession>();
            
        if (arPlaneManager == null)
            arPlaneManager = FindObjectOfType<ARPlaneManager>();
            
        if (arCamera == null)
            arCamera = FindObjectOfType<Camera>();

        // Ensure we have all required components
        if (arCamera == null)
        {
            Debug.LogError("ARPCDebugSimulator: Missing required Camera component! Please assign it in the inspector.");
            enabled = false;
            return;
        }
        
        // Create default material if not assigned
        if (planeMaterial == null && useCustomVisualization)
        {
            planeMaterial = new Material(Shader.Find("Transparent/Diffuse"));
            planeMaterial.color = planeColor;
        }
        
        // If we're on PC and simulating planes, start simulation
        if (isOnPC && simulatePlanesOnPC)
        {
            StartCoroutine(EnableWebCam());
            
            // If using ARPlaneManager, subscribe to its events
            if (arPlaneManager != null)
            {
                arPlaneManager.planesChanged += OnPlanesChanged;
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (arPlaneManager != null)
            arPlaneManager.planesChanged -= OnPlanesChanged;
            
        // Clean up any debug planes
        foreach (var plane in debugPlanes)
        {
            if (plane != null)
                Destroy(plane);
        }
    }

    private void Update()
    {
        if (!isOnPC || !simulatePlanesOnPC)
            return;

        // Generate planes at intervals
        timeSinceLastPlane += Time.deltaTime;
        if (timeSinceLastPlane >= planeGenerationInterval)
        {
            timeSinceLastPlane = 0;
            GenerateDebugPlane();
        }
        
        // Add keyboard controls for debugging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateDebugPlane();
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // This is a hookpoint to inject our debug planes if needed
        // We don't need to do anything here as we're creating planes directly
    }

    private IEnumerator EnableWebCam()
    {
        // Request webcam permission
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogError("ARPCDebugSimulator: WebCam permission denied!");
            yield break;
        }

        // Get available webcams
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("ARPCDebugSimulator: No webcam detected!");
            yield break;
        }

        // Use the first available webcam
        WebCamTexture webCamTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height, 30);
        webCamTexture.Play();
        
        // Create a debug canvas to display the webcam feed
        GameObject debugCanvas = new GameObject("DebugWebcamCanvas");
        Canvas canvas = debugCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        GameObject rawImageObj = new GameObject("WebcamRawImage");
        rawImageObj.transform.SetParent(debugCanvas.transform, false);
        
        UnityEngine.UI.RawImage rawImage = rawImageObj.AddComponent<UnityEngine.UI.RawImage>();
        rawImage.texture = webCamTexture;
        
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Set the image to be behind UI elements but visible
        rawImage.raycastTarget = false;
        canvas.sortingOrder = -1;
        
        Debug.Log("ARPCDebugSimulator: WebCam initialized successfully!");
    }

    private void GenerateDebugPlane()
    {
        // Create a position in front of the camera
        Vector3 position = arCamera.transform.position + arCamera.transform.forward * planeDistance;
        
        // Add some randomness to position
        position += new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.2f, 0.2f),
            Random.Range(-0.5f, 0.5f)
        );

        GameObject planeGO;
        
        if (arPlaneManager != null && arPlaneManager.enabled && arPlaneManager.planePrefab != null && !useCustomVisualization)
        {
            // Use the AR plane prefab if available
            planeGO = Instantiate(arPlaneManager.planePrefab, position, Quaternion.Euler(90, 0, 0));
            
            // Try to add ARPlane component if it doesn't exist
            ARPlane debugPlane = planeGO.GetComponent<ARPlane>();
            if (debugPlane == null)
            {
                debugPlane = planeGO.AddComponent<ARPlane>();
            }
            
            // Try to set trackable ID using reflection (since it's normally set internally)
            try
            {
                var trackableIdField = typeof(ARTrackable<BoundedPlane, ARPlane>)
                    .GetField("m_Id", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (trackableIdField != null)
                {
                    trackableIdField.SetValue(debugPlane, new TrackableId(1, (ulong)planeIdCounter));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("ARPCDebugSimulator: Could not set TrackableId: " + e.Message);
            }
        }
        else
        {
            // Create a simple quad mesh for the plane
            planeGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
            planeGO.transform.position = position;
            
            // Orient the quad to face upward
            planeGO.transform.rotation = Quaternion.Euler(90, 0, 0);
            
            // Apply the visualization material
            if (planeMaterial != null)
            {
                Renderer renderer = planeGO.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = planeMaterial;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderer.receiveShadows = false;
                }
            }
        }
        
        planeGO.name = "DebugPlane_" + planeIdCounter++;
        
        // Set the plane's scale based on the desired size
        planeGO.transform.localScale = new Vector3(planeSize.x, planeSize.y, 1f);
        
        // Add to our list of debug planes
        debugPlanes.Add(planeGO);
        
        Debug.Log("ARPCDebugSimulator: Generated debug plane at " + position);
    }
}