using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [Header("Customer Order")]
    [SerializeField] private GameObject CustomerOrderPanel;
    [SerializeField] private KeyCode KEY_CustomerOrderPanel = KeyCode.Q;
    private bool isShowingCustomerOrderPanel;


    [Header("Shop")]
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private KeyCode KEY_ShopPanel = KeyCode.R;
    private bool IsshowingShoppanel;

    [Header("Pause Menu")]
    [SerializeField] private GameObject PauseMenuPanel;

    private bool isPaused = false;

    private void Start()
    {
        HideAllPanel();

        Time.timeScale = 1; 
    }

    private void Update() 
    {
        // Pause on Esc
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) )
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
            CursorManager.Instance.SetCanVibleCursor(isPaused);
                PauseMenuPanel.SetActive(isPaused);
            Debug.Log(isPaused ? "Game Paused" : "Game Resumed");
        }

        if (Input.GetKeyDown(KEY_CustomerOrderPanel))
        {
            if(isShowingCustomerOrderPanel)
            {
                HideAllPanel();
                return;
            }

            SHowCustomerOrderPanel();
            Debug.Log("Customer Panel");
        }
        else if(Input.GetKeyDown(KEY_ShopPanel))
        {

            if(IsshowingShoppanel)
            {
                HideAllPanel();
                return;
            }
            
            ShowShopPanel();
            Debug.Log("Shop Panel");
        }
    }



    
    private void HideAllPanel()
    {
        Debug.Log("Hide");
        CustomerOrderPanel.SetActive(false);
        ShopPanel.SetActive(false);

        CursorManager.Instance.SetCanVibleCursor(false);

        isShowingCustomerOrderPanel = false;
        IsshowingShoppanel = false;
    }

    private void SHowCustomerOrderPanel()
    {
        CustomerOrderPanel.SetActive(true);
        isShowingCustomerOrderPanel = true;
    }

    private void ShowShopPanel()
    {
        ShopPanel.SetActive(true);

        CursorManager.Instance.SetCanVibleCursor(true);
        IsshowingShoppanel = true;
    }

}
