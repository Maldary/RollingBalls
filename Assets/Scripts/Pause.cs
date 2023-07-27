using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{ 
    public GameObject audioSource;
    public GameObject panel;
    public void pause()
    {
        panel.SetActive(true);
        audioSource.SetActive(false);
        Time.timeScale = 0f;
    }
}
