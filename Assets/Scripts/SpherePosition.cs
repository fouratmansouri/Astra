using UnityEngine;

public class DebugSpherePosition : MonoBehaviour
{
    public Transform sphere;
    public Camera xrCamera;

    void Update()
    {
        if (sphere != null && xrCamera != null)
        {
            Debug.Log("Sphere Position: " + sphere.position);
            Debug.Log("Distance to Camera: " + Vector3.Distance(sphere.position, xrCamera.transform.position));
        }
    }
}
