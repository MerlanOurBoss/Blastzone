using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinManager : MonoBehaviour
{

    public static CoinManager Instance { get; private set; }

    [Header("Настройки")]
    public int totalCoins = 0;     // Всего монет собрано
    public int targetCoins = 10;   // Цель (необязательно)
    
    public UnityEvent<int> onCoinCollected; // Вызывается при сборе монеты
    public UnityEvent onAllCoinsCollected;  // Вызывается когда все собраны

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Debug.Log($"Монет собрано: {totalCoins}");
        
        onCoinCollected?.Invoke(totalCoins);

        if (totalCoins >= targetCoins)
            onAllCoinsCollected?.Invoke();
    }

    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            onCoinCollected?.Invoke(totalCoins);
            return true;
        }

        Debug.Log("Недостаточно монет!");
        return false;
    }
    
    public void ResetCoins()
    {
        totalCoins = 0;
        onCoinCollected?.Invoke(totalCoins);
    }
    
    public int GetCoins() => totalCoins;
}
