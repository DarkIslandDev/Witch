using System;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void ExitOnClick()
    {
        Application.Quit();
    }
}