using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI Элементы")]
    public Image healthBarFill;         // Заполнение полоски HP
    public Image healthBarBackground;   // Фон полоски
    public TextMeshProUGUI healthText;  // Текст HP (необязательно)

    [Header("Цвета полоски HP")]
    public Color highHealthColor  = Color.green;   // > 60%
    public Color midHealthColor   = Color.yellow;  // 30% - 60%
    public Color lowHealthColor   = Color.red;     // < 30%

    [Header("Анимация")]
    public float smoothSpeed = 5f; // Плавное изменение полоски
    private float targetFill = 1f;

    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth не найден!");
            return;
        }

        // Подписываемся на событие изменения HP
        playerHealth.onHealthChanged.AddListener(UpdateHealthBar);
        playerHealth.onPlayerDied.AddListener(OnPlayerDied);

        // Инициализация
        UpdateHealthBar(playerHealth.GetCurrentHealth());
    }

    void Update()
    {
        // Плавное изменение полоски HP
        if (healthBarFill != null)
            healthBarFill.fillAmount = Mathf.Lerp(
                healthBarFill.fillAmount,
                targetFill,
                Time.deltaTime * smoothSpeed
            );
    }

    // ─── Обновить полоску HP ──────────────────────────────────────
    void UpdateHealthBar(float currentHealth)
    {
        float percent = currentHealth / playerHealth.GetMaxHealth();
        targetFill = percent;

        // Меняем цвет в зависимости от HP
        if (healthBarFill != null)
        {
            if (percent > 0.6f)
                healthBarFill.color = highHealthColor;
            else if (percent > 0.3f)
                healthBarFill.color = midHealthColor;
            else
                healthBarFill.color = lowHealthColor;
        }

        // Обновляем текст
        if (healthText != null)
            healthText.text = $"{(int)currentHealth} / {(int)playerHealth.GetMaxHealth()}";
    }

    // ─── Смерть игрока ────────────────────────────────────────────
    void OnPlayerDied()
    {
        if (healthText != null)
            healthText.text = "GAME OVER";
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.RemoveListener(UpdateHealthBar);
            playerHealth.onPlayerDied.RemoveListener(OnPlayerDied);
        }
    }
}