using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonFogScript : MonoBehaviour
{
    private GameManagerScript gmScript;

    public bool acidRain = false;
    public bool enable = false;

    public int wall = 0;
    
    //public GameObject poisonWallLeft;
    //public GameObject poisonWallRight;

    public RawImage[] hazard;

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
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!enable)
                {
                    wall = Random.Range(1, 100);
                }


                if (wall <= 49 && wall > 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        StartCoroutine(HazardLeft());
                    }
                }
                else if (wall >= 50)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        StartCoroutine(HazardRight());
                    }
                }

            }

            if (wall <= 49 && wall > 0)
            {
                if (acidRain && enable)
                {
                    for (int i = 0; i < gmScript.inGameChars.Length; i++)
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
            else if (wall >= 50)
            {
                if (acidRain && enable)
                {
                    for (int i = 0; i < gmScript.inGameChars.Length; i++)
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

            if (!enable)
            {
                acidRain = false;
            }
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

        PoisonLeft();
    }

    public IEnumerator HazardRight()
    {
        for (int i = 0; i < 4; i++)
        {
            hazard[1].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            hazard[0].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < 8; i++)
        {
            hazard[1].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            hazard[1].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }

        PoisonRight();
    }
}
