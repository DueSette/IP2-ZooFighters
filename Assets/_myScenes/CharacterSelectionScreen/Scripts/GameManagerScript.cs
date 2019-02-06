using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    #region Singleton
    public static GameManagerScript instance;

    void Awake()
    {
        if (instance != null)
        {
            return;
        }
        
        instance = this;
    }
    #endregion

    public enum GameStates { mainMenu, charSelect, inGame, victoryScreen };

    // Update is called once per frame
    void Update()
    {
        
    }
}
