using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float dampTime = 0.5f;
    public float decreaseAmount = 1.0f;
    public float amount;
    public float shakeSpeed;
    public float distanceApart;

    public int activeTime = 0;

    public bool shaking = false;

    Vector3 current;

    float xOffset;
    float yOffset;
    float seed;

    public Transform[] players;

    private Camera cam;
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
            transform.position = transform.position + (endPosition - transform.position) * (1 - (1 - t) * (1 - t));
            if (t >= 1)
            {
                transform.position = endPosition;
            }
            yield return null;
        }
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
        seed = Random.Range(0, 100000);
        current = transform.position;
        cameraStartPos = transform.position;
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

        FindCentre();

        transform.position = Vector3.SmoothDamp(transform.position, centrePos, ref moveVelocity, dampTime);

        if (players[0].position.x < players[1].position.x)
        {
            distanceApart = players[0].position.x - players[1].position.x;
        }
        else
        {
            distanceApart = players[1].position.x - players[0].position.x;
        }

        if (distanceApart < 10)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, distanceApart - 5);
        }
        else if (distanceApart < 20)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, distanceApart / 4);
        }
        else if (distanceApart < 30)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, distanceApart / 8);
        }
        else
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, distanceApart / 12);
        }

        if (cam.transform.position.z > -10)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -10);
        }

    }

    public void FindCentre()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].gameObject.activeSelf)
                continue;

            averagePos += players[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = 0;

        centrePos = averagePos;
    }

    void Shake()
    {
        xOffset = Mathf.PerlinNoise(seed + Time.time * shakeSpeed, 0) * 2 - 1;
        yOffset = Mathf.PerlinNoise(0, seed + Time.time * shakeSpeed) * 2 - 1;
        transform.position = transform.position + new Vector3(xOffset * amount, yOffset * amount, 0); 
    }
}
