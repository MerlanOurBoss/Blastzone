using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Здоровье")]
    public float maxHealth = 100f;
    private float currentHealth;
    private Renderer _renderer;
    private Color _color;
    
    [Header("UI")]
    public Slider healthBar; // Полоска HP (необязательно)

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _color = _renderer.material.color;
        
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.maxValue = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthBar != null)
            healthBar.value = currentHealth;
        
        StartCoroutine(HitEffect());

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} убит!");
        CoinManager.Instance?.AddCoins(1);
        Destroy(gameObject);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    System.Collections.IEnumerator HitEffect()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            renderer.material.color = _color;
        }
    }
}