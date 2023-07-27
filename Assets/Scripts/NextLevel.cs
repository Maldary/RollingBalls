using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public GameObject panel;
    public void Load()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(index);
        panel.SetActive(false);
    }
}
