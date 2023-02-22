using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneBGM : MonoBehaviour
{
    private void Awake()
    {
        var obj = FindObjectsOfType<MainSceneBGM>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("Hello?");
    }

}
