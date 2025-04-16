using UnityEngine;

public class CursorManager : MonoBehaviour
{

    public static CursorManager Instance { get; private set; }

    private bool CanvisableCursor = false;


    private void Awake()
    {
        Instance = this;//////
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (!CanvisableCursor)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void SetCanVibleCursor(bool b)
    {
        CanvisableCursor = b;

        switch (CanvisableCursor)
        {
            case true:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case false:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }


}
