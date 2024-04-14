using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
   


    void Start()
    {
        
       
        
    }
    public void Game_Scene(string Scene)
    {

        SceneManager.LoadScene(Scene);
    }
    // Update is called once per frame
    void Update()
    {
        
    }



    public void Desconect()
    {
            SceneManager.LoadScene("Menu");

    }

    
}
