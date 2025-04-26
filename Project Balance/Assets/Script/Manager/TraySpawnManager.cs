using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TraySpawnManager : MonoBehaviour
{

    [SerializeField] private Transform spawnPoint; 

    [Header("Tray Settings")]
    [SerializeField] private GameObject smalltrayPrefab; 
    [SerializeField] private GameObject mediumTrayPrefab; 
    [SerializeField] private GameObject largeTrayPrefab; 


    [Header("Tray Prices")]
    [SerializeField] private int smallTrayPrice = 20;
    [SerializeField] private int mediumTrayPrice = 50;
    [SerializeField] private int largeTrayPrice = 100;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip MoneySound;
    [SerializeField] private float volume = 1f;


    private void PlayMoneySound()
    {
        soundEffectsManager.instance.playSoundEffectsClip3D(MoneySound, transform, volume);
    }
    
    public void SpawnSmallTray()
    {
        if (MoneyManager.Instance.GetCurrentMoney() >= smallTrayPrice)
        {
            MoneyManager.Instance.SuntarctMoney(smallTrayPrice);
            PlayMoneySound();
            SpawnTray(smalltrayPrefab);
        }

    }
    public void SpawnMediumTray()
    {
        if (MoneyManager.Instance.GetCurrentMoney() >= mediumTrayPrice)
        {
            MoneyManager.Instance.SuntarctMoney(mediumTrayPrice);
            PlayMoneySound();
            SpawnTray(mediumTrayPrefab);
        }
    }
    public void SpawnLargeTray()
    {
        if (MoneyManager.Instance.GetCurrentMoney() >= largeTrayPrice)
        {
            MoneyManager.Instance.SuntarctMoney(largeTrayPrice);
            PlayMoneySound();
            SpawnTray(largeTrayPrefab);
        }
    }

    private void SpawnTray(GameObject trayPrefab)
    {

        GameObject tray = Instantiate(trayPrefab, spawnPoint.position, spawnPoint.rotation);
        tray.transform.SetParent(spawnPoint); // Set the parent to the spawn point
    }

}
