using UnityEngine;
using UnityEngine.SceneManagement;

public class Sceneload : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        FirebaseManager.Instance.StartLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
