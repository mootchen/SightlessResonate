using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{

    //TODO add sceen name to be Swiched to
    private string SceneName = ""; 
   public void Replay(){
       SceneManager.LoadScene(SceneName);
       }

   public void quit(){
       Application.Quit();
      
   }
 
}
