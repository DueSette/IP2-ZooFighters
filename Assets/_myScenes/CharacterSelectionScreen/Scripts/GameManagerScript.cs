using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public enum gameState { mainMenu, charSelect, inGame, victoryScreen };
    public gameState theGameState;

    private SpawnManager spawnManager; //reference to CharacterSpawner, which is also a singleton
    public Camera cam;
    public Canvas canvas;
    public GameObject portraitsHolder;
    public GameObject selectedPortraits;
    public GameObject[] selectors = new GameObject[4];

    #region Singleton
    public static GameManagerScript gmInstance;
    public void Awake()
    {
        if (gmInstance != null)
        {
            return;
        }
        else
        {
            gmInstance = this;
        }
    }
    #endregion

    private void Start()
    {
        SetGameState(gameState.charSelect);
        spawnManager = SpawnManager.cSpawnInstance;

        //populates the selectors array
        for (int i = 0; i < selectors.Length; i++)
        {
            selectors[i] = cam.GetComponent<InputManager>().selectors[i];
        }
    }
    
    public gameState GetGameState()
    {
        return theGameState;
    }

    public void SetGameState(gameState gs)
    {
        theGameState = gs;
    }

    public void StartGame()
    {
        if (CheckIfReady())
        {
            SetGameState(gameState.inGame);
            spawnManager.StartCoroutine("SpawnCharacters");
        }

        ExitCharacterSelect();
        //set up music
        //make characterselect UI go away
    }

    private void ExitCharacterSelect()
    {
        //portraitsHolder.transform.position;
        
    }

    private bool CheckIfReady()
    {
        int playersActive = 0;
        int playersReady = 0;

        for (int i = 0; i < selectors.Length; i++)
        {
            if (selectors[i].activeSelf)
            {
                playersActive++;
            }
        }

        for (int i = 0; i < selectors.Length; i++)
        {
            if (selectors[i].GetComponent<SelectorBehaviour>().ready)
            {
                playersReady++;
            }
        }

        if (playersActive == playersReady)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
