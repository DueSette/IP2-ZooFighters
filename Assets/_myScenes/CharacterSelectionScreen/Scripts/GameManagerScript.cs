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
            spawnManager.StartCoroutine("SpawnCharacters");

            //shifts the UI away
            StartCoroutine(LerpUI(portraitsHolder, -400));
            StartCoroutine(LerpUI(selectedPortraits, 1200));
        }
    }

    //moves the UI pieces from view to outside view or viceversa
    private IEnumerator LerpUI(GameObject theObject, float destination)
    {
        float t = 0;
        Vector3 startPos = theObject.transform.position;
        while (t < 1)
        {

            t += Time.deltaTime;
            //UnityEngine.Debug.Log(t);

            theObject.transform.position = Vector3.Lerp(startPos, new Vector3(startPos.x, destination, startPos.z), t * t);
            if (t >= 1)
            {
                theObject.transform.position = new Vector3(startPos.x, destination, startPos.z);
            }
            yield return null;
        }
        //UnityEngine.Debug.Log("coroutine time taken: " + stp.Elapsed);
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
