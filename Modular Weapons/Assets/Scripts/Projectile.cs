using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float proj_speed    = 15.0f;
    private float proj_damage   = 1.0f;
    private float proj_duration = 5.0f;
    private float proj_timer    = 0.0f;

    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        proj_timer += Time.deltaTime;
        this.transform.position += velocity * Time.deltaTime * proj_speed;
        if (proj_timer >= proj_duration)
        {
            DeleteProjectile();
        }
    }

    /// <summary>
    /// Pass through all necessary variables to shoot projectile
    /// </summary>
    /// <returns>Boolean on if the projectile is shot or not able to be generated</returns>
    public bool ShootWithDir(Vector3 fire_dir)
    {
        velocity = fire_dir;
        return true;
    }
    /// <summary>
    /// Pass through all necessary variables to shoot projectile
    /// </summary>
    /// <returns>Boolean on if the projectile is shot or not able to be generated</returns>
    public bool ShootAtTarget(Vector3 fire_target)
    {
        velocity = fire_target - this.transform.position;
        velocity.z = 0;
        velocity = velocity.normalized;
        return true;
    }

    public void GiveModifiers(Queue<SpellInfo> modifiers)
    {
        foreach (SpellInfo mod in modifiers)
        {
            Debug.Log(mod.name);
        }
    }

    private void DeleteProjectile()
    {
        // More lines in future for specific variable removals
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
                // Enemy stuff here
                collision.gameObject.GetComponent<Enemy>().TakeDamage(proj_damage);
                DeleteProjectile();
                break;
            case "Player":
                // Player stuff here
                break;
            case "Wall":
                // Wall stuff here
                DeleteProjectile();
                break;
            default:
                break;

        }
        Debug.Log(collision.tag + " Collision With Projectile");
    }
}
