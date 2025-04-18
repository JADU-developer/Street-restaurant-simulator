using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour {


    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Transform TrayGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectGrabbable objectGrabbable;
    private Tray tray;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (objectGrabbable == null && tray == null) {
                // Not carrying an object, try to grab
                float pickUpDistance = 4f;

                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask ,QueryTriggerInteraction.Ignore )) 
                {
                    if(raycastHit.transform.TryGetComponent(out tray))
                    {
                        tray.Grab(TrayGrabPointTransform);
                    } 
                    else if (raycastHit.transform.TryGetComponent(out objectGrabbable)) 
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                    }
                }
            } else {
                // Currently carrying something, drop

                if (objectGrabbable != null)
                {
                    objectGrabbable.Drop();
                    objectGrabbable = null;
                }
                else
                {
                    tray.Drop();
                    tray = null;
                }
                
            }
        }
    }
}