using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip deathSound;

    protected AudioSource audioSource;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();

        SetHealth();
    }

    protected void SetHealth()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int value)
    {
        audioSource.PlayOneShot(hurtSound);

        currentHealth -= value;
        if (currentHealth <= 0)
        {
            Invoke(nameof(OnDeath), 0.5f);
        }
        Debug.Log(currentHealth);
    }

    private void OnDeath()
    {
        audioSource.PlayOneShot(deathSound);
        Destroy(this.gameObject);
    }
}
