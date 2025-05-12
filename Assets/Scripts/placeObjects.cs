using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class placeObjects : MonoBehaviour
{
    public GameObject cube;
    public ARRaycastManager raycastManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                GameObject placedCube = Instantiate(cube, hitPose.position + new Vector3(0, 0.01f, 0), hitPose.rotation);

                Rigidbody rb = placedCube.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                Debug.Log("Cube placed on detected plane");
            }
            else
            {
                Debug.Log("No plane detected under center of screen");
            }
        }
    }
}
