using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected int maxHealth;
    protected int health;

    protected void SetHealth()
    {
        health = maxHealth;
    }

    public void TakeDamage(int value)
    {
        health -= value;
        if (health <= 0)
        {
            Invoke(nameof(OnDeath), 0.5f);
        }
        Debug.Log(health);
    }

    private void OnDeath()
    {
        Destroy(this.gameObject);
    }
}
