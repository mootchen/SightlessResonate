using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{

    
   public void Replay(){
       print("Replay");
        Application.Quit();

   }
   public void quit(){
       Application.Quit();
       print("quit");
   }
   public void MainMenu(){
       Application.Quit();
       print("Mainmenu");
   }
}
