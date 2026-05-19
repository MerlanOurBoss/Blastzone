using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Здоровье")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Настройки")]
    public float invincibleTime = 0.5f; // Время неуязвимости после удара
    private bool isInvincible = false;

    [Header("События")]
    public UnityEvent<float> onHealthChanged; // Передаёт текущее HP
    public UnityEvent onPlayerDied;           // Вызывается при смерти

    [Header("Эффекты")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        // Обновляем UI сразу при старте
        onHealthChanged?.Invoke(currentHealth);
    }

    // ─── Получить урон ────────────────────────────────────────────
    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Игрок получил {damage} урона! HP: {currentHealth}/{maxHealth}");

        // Обновляем UI
        onHealthChanged?.Invoke(currentHealth);

        // Звук урона
        if (hurtSound != null)
            audioSource?.PlayOneShot(hurtSound);

        // Эффект мигания
        StartCoroutine(InvincibilityFrames());

        if (currentHealth <= 0)
            Die();
    }

    // ─── Восстановить здоровье ────────────────────────────────────
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth);
    }

    // ─── Смерть игрока ────────────────────────────────────────────
    void Die()
    {
        if (deathSound != null)
            audioSource?.PlayOneShot(deathSound);

        Debug.Log("Игрок умер!");
        onPlayerDied?.Invoke();

        // Останавливаем игру (можно заменить на Game Over экран)
        Time.timeScale = 0f;
    }

    // ─── Кадры неуязвимости ───────────────────────────────────────
    System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        // Мигание персонажа
        Renderer rend = GetComponent<Renderer>();
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            if (rend != null)
                rend.enabled = !rend.enabled; // Мигаем

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        if (rend != null)
            rend.enabled = true;

        isInvincible = false;
    }

    // ─── Геттеры ──────────────────────────────────────────────────
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercent() => currentHealth / maxHealth;
}