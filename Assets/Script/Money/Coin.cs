using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Настройки монеты")]
    public int coinValue = 1;
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;

    [Header("Эффекты")]
    public GameObject collectEffect;
    public AudioClip collectSound;

    private Vector3 startPosition;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        CoinManager.Instance.AddCoins(coinValue);

        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        Destroy(gameObject);
    }
}