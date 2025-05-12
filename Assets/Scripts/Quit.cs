using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    // Method to quit the game
    public void QuitGame()
    {
        // If running in the editor, stop the play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();  // This will quit the application if built as a standalone.
#endif
    }
}
