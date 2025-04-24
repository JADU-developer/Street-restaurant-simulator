using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    [SerializeField] private Transform FoodContainer;
    [SerializeField] private float tiltSpeed = 30f;      // Speed of manual tilt
    [SerializeField] private float autoTiltAmount = 5f;  // Max auto tilt angle per wobble
    [SerializeField] private float autoTiltDuration = 0.5f;
    [SerializeField] private float minWobbleInterval = 2f;
    [SerializeField] private float maxWobbleInterval = 5f;


    [Header("Audio Settings")]
    [SerializeField] private AudioClip PickupSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private float volume = 1f;


    private bool IsGrabing = false;

    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    private bool isAutoTilting = false;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(RandomTrayWobble());
    }

    void Update()
    {
        if (IsGrabing) HandleTiltInput();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        soundEffectsManager.instance.playSoundEffectsClip3D(PickupSound, transform, volume);

        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = true;
        IsGrabing = true;

        transform.SetParent(objectGrabPointTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = objectGrabPointTransform.localRotation;
    }

    public void Drop()
    {

        soundEffectsManager.instance.playSoundEffectsClip3D(dropSound, transform, volume);
        
        this.objectGrabPointTransform = null;
        transform.parent = null; // Detach from parent
        objectRigidbody.isKinematic = false;
        objectRigidbody.useGravity = true;
        IsGrabing = false;

        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            // // float lerpSpeed = 10f;
            // Vector3 newPosition = objectGrabPointTransform.position;
            // // Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            // objectRigidbody.rotation = transform.rotation;
            // objectRigidbody.MovePosition(newPosition);

            // Reset rotation to avoid unwanted tilting
        }
    }


    void HandleTiltInput()
    {
        float tiltAmountZ = 0f;
        float tiltAmountX = 0f;

        if (Input.GetMouseButton(0)) // LMB
        {
            tiltAmountZ = tiltSpeed * Time.deltaTime;
        }
        else if (Input.GetMouseButton(1)) // RMB
        {
            tiltAmountZ = -tiltSpeed * Time.deltaTime;
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.01f) // Detect scroll input
        {
            tiltAmountX = scrollInput * tiltSpeed;
        }

        transform.Rotate(tiltAmountX, 0f, tiltAmountZ);
    }

    IEnumerator RandomTrayWobble()
    {
        while (true)
        {
            // Wait for a random time before next wobble
            float waitTime = Random.Range(minWobbleInterval, maxWobbleInterval);
            yield return new WaitForSeconds(waitTime);

            if (!isAutoTilting && IsGrabing)
            {
                StartCoroutine(AutoTilt());
            }
        }
    }

    public IEnumerator AutoTilt()
    {
        if (IsGrabing) yield return null;

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
        if (other.gameObject.CompareTag("Food"))
        {
            // other.GetComponent<Rigidbody>().useGravity = false;
            other.transform.SetParent(FoodContainer);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            other.transform.SetParent(null);
            // other.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
