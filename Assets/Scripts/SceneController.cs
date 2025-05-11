using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
   public void SwithScene(string sceneName)
   {
        SceneManager.LoadScene(sceneName);
    
   }
}
