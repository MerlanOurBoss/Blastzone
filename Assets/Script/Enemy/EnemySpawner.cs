using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Враг")]
    public GameObject enemyPrefab;

    [Header("Настройки кольца")]
    public float spawnRadius = 15f;      // Радиус кольца спавна
    public float minRadius = 10f;        // Минимальный радиус (ширина кольца)

    [Header("Настройки волн")]
    public int enemiesPerWave = 5;       // Врагов за волну
    public float timeBetweenWaves = 5f;  // Время между волнами
    public float timeBetweenSpawns = 0.3f; // Время между спавном каждого врага
    public int maxWaves = 10;            // Максимум волн (0 = бесконечно)

    [Header("Масштабирование сложности")]
    public int extraEnemiesPerWave = 2;  // +N врагов каждую волну
    public float waveSpeedMultiplier = 1.1f; // Скорость врагов растёт

    [Header("Визуализация")]
    public bool showGizmos = true;

    private int currentWave = 0;
    private bool isSpawning = false;
    private Transform player;

    void Start()
    {
        // Находим игрока автоматически
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Игрок не найден! Назначь тег Player");
            return;
        }

        StartCoroutine(StartWaves());
    }

    // ─── Запуск волн ──────────────────────────────────────────────
    IEnumerator StartWaves()
    {
        while (maxWaves == 0 || currentWave < maxWaves)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++;
            Debug.Log($"Волна {currentWave} началась!");

            yield return StartCoroutine(SpawnWave());
        }

        Debug.Log("Все волны завершены!");
    }

    // ─── Спавн одной волны ────────────────────────────────────────
    IEnumerator SpawnWave()
    {
        if (isSpawning) yield break;
        isSpawning = true;

        // Кол-во врагов растёт с каждой волной
        int enemiesToSpawn = enemiesPerWave + (currentWave - 1) * extraEnemiesPerWave;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
    }

    // ─── Спавн одного врага ───────────────────────────────────────
    void SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomRingPosition();

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // Масштабируем скорость врага с каждой волной
        var enemyMove = enemy.GetComponent<EnemyMovement>();
        if (enemyMove != null)
            enemyMove.moveSpeed *= Mathf.Pow(waveSpeedMultiplier, currentWave - 1);
    }

    // ─── Получить случайную точку на кольце ───────────────────────
    Vector3 GetRandomRingPosition()
    {
        // Случайный угол на окружности
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Случайный радиус между minRadius и spawnRadius (ширина кольца)
        float radius = Random.Range(minRadius, spawnRadius);

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Спавним относительно игрока
        Vector3 spawnPos = player.position + new Vector3(x, 0f, z);

        // Подгоняем по высоте земли через Raycast
        if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
            spawnPos.y = hit.point.y;

        return spawnPos;
    }

    // ─── Публичные методы ─────────────────────────────────────────
    public void SkipToNextWave()
    {
        StopAllCoroutines();
        StartCoroutine(StartWaves());
    }

    public int GetCurrentWave() => currentWave;

    // ─── Визуализация в редакторе ─────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Vector3 center = player != null ? player.position : transform.position;

        // Внешнее кольцо (красное)
        Gizmos.color = Color.red;
        DrawCircle(center, spawnRadius, 36);

        // Внутреннее кольцо (жёлтое)
        Gizmos.color = Color.yellow;
        DrawCircle(center, minRadius, 36);
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        float step = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);

        for (int i = 1; i <= segments; i++)
        {
            angle = i * step * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}