using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDeestroy : MonoBehaviour
{ 
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }   
}
