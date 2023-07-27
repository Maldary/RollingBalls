using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Continue : MonoBehaviour
{
    public GameObject panel;
    public GameObject audioSource;
    public void continueGame()
    {
        Time.timeScale = 1f;
        audioSource.SetActive(true);
        panel.SetActive(false);
    }
}
