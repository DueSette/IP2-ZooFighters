using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameManagerScript gmScript;

    public float dampTime = 0.5f;
    public float decreaseAmount = 1.0f;
    public float amount;
    public float shakeSpeed;

    public int activeTime = 0;

    public bool shaking = false;

    Vector3 current;

    float xOffset;
    float yOffset;
    float seed;

    private Transform[] players = new Transform[4];
    public bool targetsAcquired = false;

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

    private void Start()
    {
        gmScript = GameManagerScript.gmInstance;
        seed = Random.Range(0, 100000);
        current = transform.position;
        cameraStartPos = transform.position;               
    }

    public void SetUpTargets()
    {
        
        for (int i = 0; i < gmScript.inGameChars.Length; i++)
        {
            if(gmScript.inGameChars[i] != null)
                players[i] = gmScript.inGameChars[i].transform;

            print(i);
        }
        targetsAcquired = true;      
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            shaking = true;
        }

        if (shaking)
        {
            activeTime++;
            Shake();
        }

        if (activeTime >= 00)
        {
            shaking = false;
            activeTime = 0;
        }

        if (targetsAcquired)
        {
            FindCentre();
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(centrePos.x, centrePos.y, transform.position.z), ref moveVelocity, dampTime);
        }
        
        //may want to add a little zoom-in here if needed   
    }

    public void FindCentre()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (gmScript.inGameChars[i]!= null && !gmScript.inGameChars[i].gameObject.GetComponent<BaseCharacterBehaviour>().alive)
                continue;

            if(players[i] != null)
                averagePos += players[i].position;

            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        if(averagePos.y < 22)
        {
            averagePos.y = 22;
        }

        if(averagePos.y > 33)
        {
            averagePos.y = 33;
        }

        centrePos = averagePos;
    }

    void Shake()
    {
        xOffset = Mathf.PerlinNoise(seed + Time.time * shakeSpeed, 0) * 2 - 1;
        yOffset = Mathf.PerlinNoise(0, seed + Time.time * shakeSpeed) * 2 - 1;
        transform.position = transform.position + new Vector3(xOffset * amount, yOffset * amount, 0); 
    }
}
