using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManagerScript : MonoBehaviour
{
    private GameManagerScript gmScript;

    public float timer = 0;
    public float timeBetweenHazards = 20;

    public int hazardChoice = 0;

    public GameObject[] hazards;

    // Start is called before the first frame update
    void Start()
    {
        gmScript = GameManagerScript.gmInstance;
    }

    // Update is called once per frame
    void Update()
    {
        if (gmScript.GetGameState() == GameManagerScript.GameState.inGame)
        {
            timer += Time.deltaTime;
        }

        if (timer >= timeBetweenHazards)
        {
            hazardChoice = Random.Range(0, hazards.Length);

            if (hazardChoice == 0)
            {
                hazards[0].gameObject.SetActive(true);
            }
            else
            {
                Instantiate(hazards[hazardChoice], new Vector3(0, 0, 0), Quaternion.identity);
            }

            timer = 0;
        }
    }
}
