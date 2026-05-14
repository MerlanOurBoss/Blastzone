using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinText;

    void Start()
    {
        CoinManager.Instance.onCoinCollected.AddListener(UpdateUI);
        CoinManager.Instance.onAllCoinsCollected.AddListener(OnAllCollected);

        UpdateUI(CoinManager.Instance.GetCoins());
    }

    void UpdateUI(int amount)
    {
        coinText.text = $"Монеты: {amount}";
    }

    void OnAllCollected()
    {
        coinText.text = "Все монеты собраны! 🎉";
    }

    void OnDestroy()
    {
        // Отписываемся от событий
        CoinManager.Instance.onCoinCollected.RemoveListener(UpdateUI);
    }
}