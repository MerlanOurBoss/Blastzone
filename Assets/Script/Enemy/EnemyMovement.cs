using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 1.5f; // Дистанция атаки

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stoppingDistance)
        {
            // Идём к игроку
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Смотрим на игрока
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }
}