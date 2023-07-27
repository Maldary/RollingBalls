using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rotation : MonoBehaviour
{
    public int coinValue = 1;
    public AudioClip collectSound;
    public float rotationSpeed = 160f;
    public TextMeshProUGUI coinText;
    private Animator animator;
    private int coinsCollected;
    private GameManager gameManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.AddCoins(coinValue);
            animator.Play("CoinCollect");
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
            Destroy(gameObject);
            coinsCollected += gameManager.GetCoins();
            coinText.SetText ("" + gameManager.coins);
        }
    }
}