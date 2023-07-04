using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float health;
    private float max_health = 10;
    private float move_speed;

    // Start is called before the first frame update
    void Start()
    {
        health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        // Future AI stuff
    }

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

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
