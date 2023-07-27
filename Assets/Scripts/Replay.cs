using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Replay : MonoBehaviour
{
    public GameObject panel;
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        panel.SetActive(false);
        
    }
}
