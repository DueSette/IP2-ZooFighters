using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public enum GameState { mainMenu, charSelect, characterSpawning, inGame, victoryScreen };
    public GameState theGameState;

    public Camera cam;
    public Canvas canvas;
    public GameObject portraitsHolder;
    public GameObject selectedPortraits;
    public GameObject[] selectors = new GameObject[4];

    [Tooltip("The InGame spawned characters")]
    public GameObject[] inGameChars = new GameObject[4];
    [Tooltip("The UI prefab that will be populated with info about the in game status")]
    public GameObject inGameUIObj;
    [Tooltip("The (up to) 4 HUD boxes with the character info (hp, weapon)")]
    public GameObject[] inGameUIObjects = new GameObject[4];

    public float timeBetweenCharSpawns = 0.2f;
    [Tooltip("Populate with all the desired spawning places. Make sure that they correspond to the transf of an empty object on the map for easier location")]
    public Transform[] spawnLocations = new Transform[4];

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

    private void Update()
    {
        if (GetGameState() == GameState.inGame)
        {
            UpdateInGameUI();
        }
    }

    private void Start()
    {
        SetGameState(GameState.charSelect);

        //populates the selectors array
        for (int i = 0; i < selectors.Length; i++)
        {
            selectors[i] = gameObject.GetComponent<InputManager>().selectors[i];
        }
    }
    
    //Sets everything for the transition from character selection to gameplay phase
    public void StartGame()
    {
        if (CheckIfReady())
        {
            StartCoroutine("SpawnCharacters");

            //shifts the UI away
            StartCoroutine(LerpUI(portraitsHolder, -400));
            StartCoroutine(LerpUI(selectedPortraits, 1200));

            //Should ready the UI to work with each portrait
            InitialiseInGameUI();
        }
    }

    public IEnumerator SpawnCharacters()
    {
        SetGameState(GameState.characterSpawning);

        int spawnLocationNumber = 0;

        foreach (GameObject selector in selectors)
        {
            yield return new WaitForSeconds(timeBetweenCharSpawns);
            SelectorBehaviour selScript;
            selScript = selector.GetComponent<SelectorBehaviour>();

            if (selScript.chosenCharacter != null)
            {
                //iterates through the SpawnLocations transforms...
                Transform transform = spawnLocations[spawnLocationNumber].transform;

                //and assigns its position to the character's transform 
                selScript.chosenCharacter.transform.position = transform.position;

                //finally activates the character
                selScript.chosenCharacter.SetActive(true);
                spawnLocationNumber++;
            }
        }
        yield return null;
        SetGameState(GameState.inGame);
    }

    //Sets up UI
    public void InitialiseInGameUI()
    {
        int i = 0;
        foreach (GameObject selector in selectors)
        {
            if (selector.activeSelf)
            {
                //Creates and positions the UI objects, then it communicates to them the character they are portraying
                inGameUIObjects[i] = Instantiate(inGameUIObj, transform.position + new Vector3 (225 + (i * 450), 85, 0), Quaternion.identity);
                inGameUIObjects[i].transform.SetParent(canvas.transform);

                inGameUIObjects[i].GetComponent<InGameUIScript>().representedCharacter = selector.GetComponent<SelectorBehaviour>().chosenCharacter;                
            }
            i++;
        }
    }

    //Updates the UI for the current frame
    public void UpdateInGameUI()
    {
        int i = 0;
        foreach (GameObject uiObj in inGameUIObjects)
        {
            if (uiObj != null)
            {
                inGameUIObjects[i].GetComponent<InGameUIScript>().UpdateHUD();
                i++;
            }
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

            theObject.transform.position = Vector3.Lerp(startPos, new Vector3(startPos.x, destination, startPos.z), t * t);
            if (t >= 1)
            {
                theObject.transform.position = new Vector3(startPos.x, destination, startPos.z);
            }
            yield return null;
        }
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

        //cannot start if no one joined the game
        if (playersActive == playersReady && playersActive > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public GameState GetGameState()
    {
        return theGameState;
    }

    public void SetGameState(GameState gs)
    {
        theGameState = gs;
    }
}
