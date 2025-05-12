using UnityEngine.XR.ARFoundation;
using UnityEngine;
public class ARResetter : MonoBehaviour
{
    public ARSession arSession;

    void Start()
    {
        arSession.Reset(); // Resets tracking, especially after a scene switch
    }
}
