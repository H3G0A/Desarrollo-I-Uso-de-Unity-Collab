using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;

    protected void SetHealth()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int value)
    {
        currentHealth -= value;
        if (currentHealth <= 0)
        {
            Invoke(nameof(OnDeath), 0.5f);
        }
        Debug.Log(currentHealth);
    }

    private void OnDeath()
    {
        Destroy(this.gameObject);
    }
}
