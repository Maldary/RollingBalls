using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int coins;
    private TextMeshProUGUI coinText;

    void Start()
    {
        coinText = GetComponent<TextMeshProUGUI>();
        coins = PlayerPrefs.GetInt("COINS", 0);
        coinText.text = coins.ToString();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinText.text = coins.ToString();
        PlayerPrefs.SetInt("COINS", coins);
    }

    public int GetCoins()
    {
        return coins;
    }
}