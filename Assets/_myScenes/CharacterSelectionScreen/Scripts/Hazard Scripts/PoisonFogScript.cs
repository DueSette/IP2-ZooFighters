using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonFogScript : MonoBehaviour
{
    private GameManagerScript gmScript;

    public bool acidRain = false;
    public bool enable = true;

    public int wall = 0;

    public GameObject poisonTrigger;
    //public GameObject poisonWallRight;

    public RawImage[] hazard;

    // Start is called before the first frame update
    void Start()
    {
        gmScript = GameManagerScript.gmInstance;

        wall = Random.Range(1, 100);

        enable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gmScript.GetGameState() == GameManagerScript.GameState.inGame)
        {
            if (enable)
            {
                if (wall <= 49 && wall > 0)
                {
                    StartCoroutine(HazardLeft());

                }
                else if (wall >= 50)
                {
                    StartCoroutine(HazardRight());

                }
            }
            

            /*if (wall <= 49 && wall > 0)
            {
                if (acidRain && enable)
                {
                    for (int i = 0; i < gmScript.inGameChars.Length; i++)
                    {
                        if (gmScript.inGameChars[i] != null)
                        {
                            if (gmScript.inGameChars[i].transform.position.x < -6)
                            {
                                BaseCharacterBehaviour charScript = gmScript.inGameChars[i].gameObject.GetComponent<BaseCharacterBehaviour>();

                                charScript.TakeDamage(10);

                                StartCoroutine(AcidRainDamage());
                            }

                        }
                    }
                }
            }
            else if (wall >= 50)
            {
                if (acidRain && enable)
                {
                    for (int i = 0; i < gmScript.inGameChars.Length; i++)
                    {
                        if (gmScript.inGameChars[i] != null)
                        {
                            if (gmScript.inGameChars[i].transform.position.x > -6)
                            {
                                BaseCharacterBehaviour charScript = gmScript.inGameChars[i].gameObject.GetComponent<BaseCharacterBehaviour>();

                                charScript.TakeDamage(10);

                                StartCoroutine(AcidRainDamage());
                            }

                        }
                    }
                }
            }

            if (!enable)
            {
                Destroy(gameObject);
            }*/
        }
           
    }

    public IEnumerator AcidRainDamage()
    {
        acidRain = false;
        yield return new WaitForSeconds(2);
        acidRain = true;
    }

    public IEnumerator AcidRainDisable()
    {
        yield return new WaitForSeconds(15);
        acidRain = false;
        enable = false;
        //poisonWallLeft.SetActive(false);
        //poisonWallRight.SetActive(false);
        hazard[0].gameObject.SetActive(false);
        hazard[1].gameObject.SetActive(false);
    }

    public void PoisonLeft()
    {
        if (!acidRain)
        {
            acidRain = true;
            enable = true;
            //poisonWallLeft.SetActive(true);
            StartCoroutine(AcidRainDisable());
        }
    }

    public void PoisonRight()
    {
        if (!acidRain)
        {
            acidRain = true;
            enable = true;
            //poisonWallRight.SetActive(true);
            StartCoroutine(AcidRainDisable());
        }
    }

    public IEnumerator HazardLeft()
    {
        enable = false;
        for (int i = 0; i < 4; i++)
        {
            hazard[0].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            hazard[0].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < 8; i++)
        {
            hazard[0].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            hazard[0].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }

        if (GameObject.FindWithTag("PoisonFog") == null)
        {
            hazard[0].gameObject.SetActive(false);
            Instantiate(poisonTrigger, new Vector3(-50, 25, 0), Quaternion.identity);
        }
        
    }

    public IEnumerator HazardRight()
    {
        enable = false;

        for (int i = 0; i < 4; i++)
        {
            hazard[1].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            hazard[1].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < 8; i++)
        {
            hazard[1].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            hazard[1].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }

        if (GameObject.FindWithTag("PoisonFog") == null)
        {
            hazard[1].gameObject.SetActive(false);
            Instantiate(poisonTrigger, new Vector3(50, 25, 0), Quaternion.identity);

        }
        gameObject.SetActive(false);
    }
}
