using UnityEngine;

public class ARSetupPersistence : MonoBehaviour
{
    private static ARSetupPersistence instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // Avoid duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }
}
