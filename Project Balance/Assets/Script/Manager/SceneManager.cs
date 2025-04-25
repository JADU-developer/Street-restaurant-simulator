using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{    
    public void LoadScene(int BuildIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(BuildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
