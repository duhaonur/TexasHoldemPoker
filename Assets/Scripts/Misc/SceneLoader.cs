using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string NextScene = "Menu";
    public float Delay = 0.1f;

   IEnumerator Start()
    {
        yield return new WaitForSeconds(Delay);
        SceneManager.LoadScene(NextScene);
    }
}
