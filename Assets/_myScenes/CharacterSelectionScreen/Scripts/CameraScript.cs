using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameManagerScript gmScript;

    private Vector3 startPos;
    public float dampTime = 0.5f;

    [HideInInspector]
    public float shakeAmount;
    [HideInInspector]
    public float shakeSpeed;
    public float shakeTime = 0.4f;
    [HideInInspector]
    public bool shaking = false;

    float xOffset;
    float yOffset;
    float seed;
    [HideInInspector]
    public bool gameOver = false;

    private Transform[] players = new Transform[4];

    private Vector3 centrePos;
    private Vector3 moveVelocity;
    private Vector3 cameraStartPos;

    private Transform furthestLeft;
    private Transform furthestRight;
    private Transform furthestTop;
    private Transform furthestBottom;
    bool canZoom;

    public float maxDistanceX;
    public float minDistanceX;

    public float distanceBetweenX;
    public float distanceBetweenY;
    public float dampTimeChange;
    

    #region Singleton
    public static CameraScript instance;
    void Awake()
    {
        if (instance == null)
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
            if (gmScript.inGameChars[i] != null)
                players[i] = gmScript.inGameChars[i].transform;
        }
    }

    public void Update()
    {
        if (shaking)
        {
            Shake();
        }

        if (gmScript.GetGameState() == GameManagerScript.GameState.mainMenu)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(0, 27, 27), ref moveVelocity, dampTime);
        }

        else if (gmScript.GetGameState() == GameManagerScript.GameState.charSelect)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(0, 27, -50), ref moveVelocity, dampTime);
        }

        else if (gmScript.GetGameState() == GameManagerScript.GameState.inGame)
        {
            FindCentre();
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(centrePos.x, centrePos.y, transform.position.z), ref moveVelocity, dampTime);
        }

        else if (gmScript.GetGameState() == GameManagerScript.GameState.victoryScreen)
        {
            centrePos = new Vector3(0, 35, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(centrePos.x, centrePos.y, transform.position.z), ref moveVelocity, dampTime);
        }

        if (gmScript.GetGameState() == GameManagerScript.GameState.inGame)
        {
            //if there's only one character alive keep the camera from zooming, so it doesn't go crazy
            if (gmScript.CountCharacters() < 2)
                canZoom = false;
            else
                canZoom = true;

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null && players[i].GetComponent<BaseCharacterBehaviour>().alive)
                {
                    if (furthestLeft == null)
                    {
                        //populate the furthest transforms with the first character we check for
                        furthestLeft = players[i].transform;
                        furthestRight = players[i].transform;

                        furthestBottom = players[i].transform;
                        furthestTop = players[i].transform;
                    }

                    //then after that, compare all their positions on the y and the x to find who's where
                    else if (players[i].transform.position.x < furthestLeft.position.x)
                        furthestLeft = players[i].transform;

                    else if (players[i].transform.position.x > furthestRight.position.x)
                        furthestRight = players[i].transform;

                    if (players[i].transform.position.y < furthestBottom.position.y)
                        furthestBottom = players[i].transform;

                    else if (players[i].transform.position.y > furthestTop.position.y)
                        furthestTop = players[i].transform;
                }
            }

            //calculate the distance between the two furthest characters
            distanceBetweenX = furthestRight.position.x - furthestLeft.position.x;
            distanceBetweenY = furthestTop.position.y - furthestBottom.position.y;

            if (canZoom)
            {
                //move camera on the Z axis according to the distance on the x, within certain limits
                if (distanceBetweenX > minDistanceX && distanceBetweenX < maxDistanceX)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, transform.position.y, (distanceBetweenX * -1)), ref moveVelocity, dampTimeChange);
                }
                //if we are outside those limits, have the camera get closer to its limits so that it doesn't snap back and forth
                //whenever the distance on the x is going outside borders
                else
                {
                    float x = distanceBetweenX < minDistanceX ? minDistanceX : maxDistanceX;
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, transform.position.y, (x * -1)), ref moveVelocity, dampTimeChange);
                }
                //also if people are close on the x, see if they are also close on the Y
                //this means that if we are close on the x but far away on the Y we are still gonna make the camera move back
                if (distanceBetweenX < minDistanceX && distanceBetweenY > minDistanceX)
                {
                    if (distanceBetweenY > 45)
                        distanceBetweenY = 45;
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, transform.position.y, (distanceBetweenY * -1.15f)), ref moveVelocity, dampTimeChange);
                }
            }
        }
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
            averagePos.y += transform.position.y / 2.7f;

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
}
