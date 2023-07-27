using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int coins;
    public TextMeshPro coinText;

    void Start()
    {
        coinText = GetComponent<TextMeshPro>();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public int GetCoins()
    {
        return coins;
    }
}