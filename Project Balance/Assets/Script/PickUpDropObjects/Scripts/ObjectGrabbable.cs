using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour 
{

    [SerializeField] private SOFood foodSO;

    [SerializeField] private float tiltSpeed = 30f; 

    [Header("Audio")]
    [SerializeField] private AudioClip PickupAudio;
    [SerializeField] private AudioClip DropAudio;
    [SerializeField] private float  Volume = 1f;

    private bool IsGrabing = false;

    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;


    private void Awake() 
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(IsGrabing) HandleTiltInput();
    }

    public void Grab(Transform objectGrabPointTransform) 
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        IsGrabing = true;
        transform.SetParent(null);

        soundEffectsManager.instance.playSoundEffectsClip3D(PickupAudio , transform , Volume);
    }

    public void Drop() 
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        IsGrabing = false;

        soundEffectsManager.instance.playSoundEffectsClip3D(DropAudio , transform , Volume);
    }

    private void FixedUpdate() 
    {
        if (objectGrabPointTransform != null) {
            float lerpSpeed = 10f;
            // Vector3 newPosition = objectGrabPointTransform.position;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.rotation = transform.rotation;
            objectRigidbody.MovePosition(newPosition);
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


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.enabled = false;
        }
    }


    public SOFood GetFoodSO() 
    {
        return foodSO;
    }


}