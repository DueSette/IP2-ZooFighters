using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameManagerScript gmScript;

    private Vector3 startPos;
    public float dampTime = 0.5f;

    public float shakeAmount;
    public float shakeSpeed;
    public float shakeTime = 0.4f;
    public bool shaking = false;

    float xOffset;
    float yOffset;
    float seed;
    public bool gameOver = false;

    private Transform[] players = new Transform[4];

    private Vector3 centrePos;
    private Vector3 moveVelocity;
    private Vector3 cameraStartPos;
    
    IEnumerator LerpCamera(Vector3 endPosition)
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            Debug.Log(t);
            transform.position = new Vector3(transform.position.x + (endPosition.x - transform.position.x) * (1 - (1 - t) * (1 - t)), 31, -44);
            if (t >= 1)
            {
                transform.position = endPosition;
            }
            yield return null;
        }
    }

    #region Singleton
    public static CameraScript instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    private void Start()
    {
        startPos = transform.position;
        gmScript = GameManagerScript.gmInstance;
        seed = Random.Range(0, 100000);
        cameraStartPos = transform.position;
    }

    public void SetUpTargets()
    {
        for (int i = 0; i < gmScript.inGameChars.Length; i++)
        {
            if(gmScript.inGameChars[i] != null)
                players[i] = gmScript.inGameChars[i].transform;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SetShakeTime(shakeTime, shakeSpeed, shakeAmount));
        }

        if (shaking)
        {
            Shake();
        }
        
        if (gmScript.GetGameState() == GameManagerScript.GameState.inGame)
        {
            FindCentre();
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(centrePos.x, centrePos.y, transform.position.z), ref moveVelocity, dampTime);
        }

        else if(gmScript.GetGameState() == GameManagerScript.GameState.victoryScreen)
        {
            centrePos = new Vector3(0, 35, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(centrePos.x, centrePos.y, transform.position.z), ref moveVelocity, dampTime);
        }
        
        //may want to add a little zoom-in here if needed   
    }

    public void FindCentre()
    {


        Vector3 averagePos = startPos;
        int numTargets = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (gmScript.inGameChars[i] != null && !gmScript.inGameChars[i].gameObject.GetComponent<BaseCharacterBehaviour>().alive)
                continue;

            if (players[i] != null)
                averagePos += players[i].position;
            averagePos.y += transform.position.y / 2.5f;

            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        if (averagePos.y < 17)
        {
            averagePos.y = 17;
        }

        if (averagePos.y > 40)
        {
            averagePos.y = 40;
        }

        centrePos = averagePos;
    }

    void Shake()
    {
        xOffset = Mathf.PerlinNoise(seed + Time.time * shakeSpeed, 0) * 2 - 1;
        yOffset = Mathf.PerlinNoise(0, seed + Time.time * shakeSpeed) * 2 - 1;
        transform.position = transform.position + new Vector3(xOffset * shakeAmount, yOffset * shakeAmount, 0); 
    }

    public IEnumerator SetShakeTime(float duration, float speed, float intensity)
    {
        if (!shaking)
        {
            shaking = true;
            shakeSpeed = speed;
            shakeAmount = intensity;
            yield return new WaitForSeconds(duration);
            shaking = false;
        }
    }

    private void CenterCamera()
    {

    }
}
