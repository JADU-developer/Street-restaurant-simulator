using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    [SerializeField] private float tiltSpeed = 30f;      // Speed of manual tilt
    [SerializeField] private float autoTiltAmount = 5f;  // Max auto tilt angle per wobble
    [SerializeField] private float autoTiltDuration = 0.5f;
    [SerializeField] private float minWobbleInterval = 2f;
    [SerializeField] private float maxWobbleInterval = 5f;

    private bool isAutoTilting = false;

    void Start()
    {
        StartCoroutine(RandomTrayWobble());
    }

    void Update()
    {
        HandleTiltInput();
    }

    void HandleTiltInput()
    {
        float tiltAmount = 0f;

        if (Input.GetMouseButton(0)) // LMB
        {
            tiltAmount = Mathf.Clamp(tiltSpeed * Time.deltaTime , -40 , 40);
        }
        else if (Input.GetMouseButton(1)) // RMB
        {
            tiltAmount =  Mathf.Clamp(-tiltSpeed * Time.deltaTime , -40 , 40);
            // tiltAmount = -tiltSpeed * Time.deltaTime;
        }

        transform.Rotate(0f, 0f, tiltAmount);
    }

    IEnumerator RandomTrayWobble()
    {
        while (true)
        {
            // Wait for a random time before next wobble
            float waitTime = Random.Range(minWobbleInterval, maxWobbleInterval);
            yield return new WaitForSeconds(waitTime);

            if (!isAutoTilting)
            {
                StartCoroutine(AutoTilt());
            }
        }
    }

    public IEnumerator AutoTilt()
    {
        isAutoTilting = true;

        // Choose random direction and amount
        float targetTilt = Random.Range(-autoTiltAmount, autoTiltAmount);
        float elapsed = 0f;

        while (elapsed < autoTiltDuration)
        {
            float step = (targetTilt / autoTiltDuration) * Time.deltaTime;
            transform.Rotate(0f, 0f, step);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isAutoTilting = false;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Plate"))
        {
            other.transform.SetParent(this.transform.parent);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Plate"))
        {
            other.transform.SetParent(null);
        }
    }
}
