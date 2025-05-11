using UnityEngine;
using UnityEngine.UI;

public class CameraZoom : MonoBehaviour
{
    public Camera mainCamera; // Assign the main camera
    public Slider zoomSlider; // Assign the slider from the UI

    public float minZoom = 5f;  // Minimum zoom level
    public float maxZoom = 60f; // Maximum zoom level

    void Start()
    {
        // Initialize the slider's values
        zoomSlider.minValue = minZoom;
        zoomSlider.maxValue = maxZoom;
        zoomSlider.value = mainCamera.orthographic ? mainCamera.orthographicSize : mainCamera.fieldOfView;

        // Add a listener to call ZoomCamera when the slider value changes
        zoomSlider.onValueChanged.AddListener(ZoomCamera);
    }

    void ZoomCamera(float value)
    {
        if (mainCamera.orthographic)
        {
            // Adjust orthographic size for orthographic cameras
            mainCamera.orthographicSize = value;
        }
        else
        {
            // Adjust field of view for perspective cameras
            mainCamera.fieldOfView = value;
        }
    }
}
