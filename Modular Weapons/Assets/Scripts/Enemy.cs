using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float health;
    private float max_health = 10;
    private float move_speed;

    void Awake()
    {
        health = max_health;
    }

    void Update()
    {
        // Future AI stuff
    }

    // Damage fucntionality
    public bool TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    // Death / removal functionality
    private void Die()
    {
        Destroy(this.gameObject);
    }
}
