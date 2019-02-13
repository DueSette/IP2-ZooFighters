using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //=====================HANDLES SPAWNING LOGICS (INCLUDING NPCS, WEAPONS, PLATFORMS)====================

    private GameManagerScript gmScript;  //reference to game manager script

    public float timeBetweenSpawns;
    [Tooltip("Populate with all the desired spawning places. Make sure that they correspond to the transf of an empty object on the map for easier location")]
    public Transform[] spawnLocations = new Transform[4];
    public GameObject[] selectors;

    #region Singleton and Initialisation
    public static SpawnManager cSpawnInstance;  //variable holding reference to this script              
    private void Start()
    {
    gmScript = GameManagerScript.gmInstance;    //populates the gmScript with its singleton

    //sets this as singleton
    if (cSpawnInstance != null)
    {
        return;
    }
    else
    {
        cSpawnInstance = this;
    }

    //populates the selectors
    selectors = gmScript.selectors;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SpawnCharacters()
    {
        int spawnLocationNumber = 0;
        foreach(GameObject selector in selectors)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SelectorBehaviour selScript;
            selScript = selector.GetComponent<SelectorBehaviour>();

            if(selScript.chosenCharacter != null)
            {
                //iterates through the SpawnLocations transforms...
                Transform transform = spawnLocations[spawnLocationNumber].transform;

                //and assigns its position to the character's transform 
                selScript.chosenCharacter.transform.position = transform.position;

                //assigns each character his controller input

                //selScript.chosenCharacter.AddComponent;

                //finally activates the character
                selScript.chosenCharacter.SetActive(true);
                spawnLocationNumber++;
            }
        }
        gmScript.SetGameState(GameManagerScript.gameState.inGame);
        yield return null;
    }
}
