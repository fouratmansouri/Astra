using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    public Transform sun;         // Reference to the Sun
    public float orbitSpeed;      // Speed of orbit rotation
    public float rotationSpeed;   // Planet's own rotation speed
    public Vector3 tilt; // Add tilt to planetary rotation

    void Update()
    {
        // Orbit around the Sun
        transform.RotateAround(sun.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // Self-rotation of the planet
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Orbit around the Sun
        transform.RotateAround(sun.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // Self-rotation with tilt
        transform.Rotate(tilt * rotationSpeed * Time.deltaTime);
    }
}

