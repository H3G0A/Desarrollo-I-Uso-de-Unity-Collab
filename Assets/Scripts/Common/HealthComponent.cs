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

    protected AudioSource audio;

    protected virtual void Start()
    {
        audio = GetComponent<AudioSource>();

        SetHealth();
    }

    protected void SetHealth()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int value)
    {
        audio.PlayOneShot(hurtSound);

        currentHealth -= value;
        if (currentHealth <= 0)
        {
            Invoke(nameof(OnDeath), 0.5f);
        }
        Debug.Log(currentHealth);
    }

    private void OnDeath()
    {
        audio.PlayOneShot(deathSound);
        Destroy(this.gameObject);
    }
}
