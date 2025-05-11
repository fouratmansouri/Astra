using UnityEngine;

public class RotateBlackhole : MonoBehaviour
{
    public float rotationSpeed = 50f; // Adjust this to control the speed of rotation

    void Update()
    {
        // Rotate around the Y-axis (vertical axis)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Uncomment the following lines if you want to rotate on other axes:
        // Rotate around the X-axis
        // transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);

        // Rotate around the Z-axis
        // transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
