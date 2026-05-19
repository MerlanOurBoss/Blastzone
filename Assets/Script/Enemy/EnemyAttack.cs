using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Атака")]
    public float damage = 10f;
    public float attackRate = 1f;       // Атак в секунду
    public float attackRange = 1.5f;    // Дистанция атаки

    private float attackCooldown = 0f;
    private Transform player;
    private PlayerHealth playerHealth;

    void Start()
    {
        // Находим игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null) return;

        attackCooldown -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        // Атакуем если игрок в зоне атаки
        if (distance <= attackRange && attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = 1f / attackRate;
        }
    }

    // ─── Атака ────────────────────────────────────────────────────
    void Attack()
    {
        playerHealth.TakeDamage(damage);
        Debug.Log($"{gameObject.name} атакует игрока на {damage} урона!");
    }

    // ─── Визуализация зоны атаки ──────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}