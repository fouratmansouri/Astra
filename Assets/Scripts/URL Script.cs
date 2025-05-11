using UnityEngine;

public class OpenURLButton : MonoBehaviour
{
    public string url = "https://en.wikipedia.org/wiki/Solar_System"; // The URL to open

    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}
