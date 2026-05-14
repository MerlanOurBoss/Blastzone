using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Стрельба")]
    public GameObject bulletPrefab;       // Префаб пули
    public Transform firePoint;           // Точка выстрела (дуло)
    public float detectionRadius = 20f;   // Радиус обнаружения врагов
    public float fireRate = 1f;           // Выстрелов в секунду
    public LayerMask enemyLayer;          // Слой врагов

    [Header("Визуализация")]
    public bool showRadius = true;        // Показывать радиус в редакторе

    private float fireCooldown = 0f;
    private Transform currentTarget;

    void Update()
    {
        FindTarget();
        HandleShooting();
        RotateToTarget();
    }
    
    void FindTarget()
    {
        // Ищем все коллайдеры в радиусе с нужным слоем
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        if (hits.Length == 0)
        {
            currentTarget = null;
            return;
        }

        // Находим ближайшего врага
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = hit.transform;
            }
        }

        currentTarget = closestEnemy;
    }
    
    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;

        if (currentTarget != null && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate; // Сброс кулдауна
        }
    }
    
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Создаём пулю
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
            bullet.SetTarget(currentTarget); // Передаём цель пуле

        Debug.Log($"Выстрел в {currentTarget.name}!");
    }
    
    void RotateToTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0f; // Не наклоняем игрока вверх/вниз

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showRadius) return;
        Gizmos.color = currentTarget != null ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}