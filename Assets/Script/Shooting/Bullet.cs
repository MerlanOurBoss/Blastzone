using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Настройки")]
    public float speed = 20f;
    public float damage = 25f;
    public float lifetime = 3f; // Время жизни пули

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        
        transform.LookAt(target.position);
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}