using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class Starter: MonoBehaviour
    {
        public void Start()
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("LVL", 1));
        }
    }
}