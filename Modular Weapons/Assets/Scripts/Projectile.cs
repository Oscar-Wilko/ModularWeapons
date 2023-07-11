using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    private float proj_speed    = 15.0f;
    private float proj_damage   = 1.0f;
    private float proj_duration = 2.0f;
    private float proj_timer    = 0.0f;
    private float decay_trigger = 1.0f;
    private bool sent_payload   = false;
    private DecayType decay_type = DecayType.Default;

    public GameObject projectile_prefab;
    private List<SpellInfo> modifiers = new List<SpellInfo>();
    private SpellInfo[] spell_payload;

    private Vector2 velocity;
    private Vector3 previous_position;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        previous_position = rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Modifier Functionality
        if (modifiers != null)
        {
            foreach (SpellInfo mod in modifiers)
            {
                switch (mod.name)
                {
                    case "Acceleration":
                        proj_speed *= 1 + (2.0f * Time.deltaTime);
                        break;
                }
            }
        }

        // Decay timer
        proj_timer += Time.deltaTime;
        if (proj_timer >= decay_trigger && decay_type == DecayType.Timer && !sent_payload) 
        { 
            SendPayload(velocity); 
            sent_payload = true; 
        }
        if (proj_timer >= proj_duration)
        {
            DeleteProjectile();
        }
    }

    private void FixedUpdate()
    {   
        previous_position = rb.position;      
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime * proj_speed);
    }

    /// <summary>
    /// Pass through all necessary variables to shoot projectile
    /// </summary>
    /// <returns>Boolean on if the projectile is shot or not able to be generated</returns>
    public bool ShootWithDir(Vector2 fire_dir)
    {
        velocity = fire_dir;
        velocity = velocity.normalized;
        return true;
    }
    /// <summary>
    /// Pass through all necessary variables to shoot projectile
    /// </summary>
    /// <returns>Boolean on if the projectile is shot or not able to be generated</returns>
    public bool ShootAtTarget(Vector2 fire_target)
    {
        velocity = fire_target - rb.position;
        velocity = velocity.normalized;
        return true;
    }
    public bool ShootWithDir(Vector3 fire_dir) { return ShootWithDir(new Vector2(fire_dir.x, fire_dir.y)); }
    public bool ShootAtTarget(Vector3 fire_target) { return ShootAtTarget(new Vector2(fire_target.x, fire_target.y)); }

    public void GiveModifiers(List<SpellInfo> mods) 
    { 
        List<SpellInfo> remaining_modifiers= new List<SpellInfo>();
        // Alter variables based on mods
        foreach(SpellInfo mod in mods)
        {
            // Depending on modifier, can remove from the list to speed up update function
            switch (mod.name)
            {
                case "Speed Up":
                    proj_speed *= 1.5f;
                    break;
                case "Speed Down":
                    proj_speed *= 0.75f;
                    break;
                case "Damage Up":
                    proj_damage += 5;
                    break;
                case "Acceleration":
                    proj_speed *= 0.5f;
                    remaining_modifiers.Add(mod);
                    break;
                default:
                    remaining_modifiers.Add(mod);
                    break;
            }
        }
        modifiers = remaining_modifiers;
    }

    public void GiveSpellPayload(SpellInfo[] new_payload) { spell_payload = new_payload; }    
    public void GiveDecayType(DecayType n_type) { decay_type = n_type; }
    public void AssignTexture(string filename) { this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(filename); }

    private void DeleteProjectile()
    {
        // More lines in future for specific variable removals
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Projectile collsision functionality with object tags
    /// </summary>
    /// <param name="collision">Collision data</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                // Enemy stuff here
                collision.gameObject.GetComponent<Enemy>().TakeDamage(proj_damage);
                if (decay_type == DecayType.Trigger) SendPayload(velocity);
                DeleteProjectile();
                break;
            case "Player":
                // Player stuff here
                Debug.Log("Hit Player");
                break;
            case "Wall":
                // Wall stuff here
                if (decay_type == DecayType.Trigger) SendPayload(collision.contacts[0].normal);
                DeleteProjectile();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Send out payload of modifiers and spells
    /// Multicasts are already calculated so no need to check them
    /// </summary>
    /// <param name="direction">Vector3 direction to shoot projectiles</param>
    private void SendPayload(Vector3 direction)
    {
        List<SpellInfo> cur_modifiers = new List<SpellInfo>();
        for(int i = 0; i < spell_payload.Length; i ++)
        {
            switch (spell_payload[i].type)
            {
                case SpellType.Projectile:
                    // Generate projectile with current modifiers
                    GameObject proj = Instantiate(projectile_prefab, previous_position, Quaternion.identity);
                    proj.name = "Decay Projectile";
                    Projectile proj_comp = proj.GetComponent<Projectile>();
                    proj_comp.AssignTexture(spell_payload[i].img_filename);
                    proj_comp.GiveModifiers(cur_modifiers);
                    proj_comp.ShootWithDir(direction);
                    if (spell_payload[i].decay_type != DecayType.Default)
                    {
                        proj_comp.GiveDecayType(spell_payload[i].decay_type);
                        SpellInfo[] next_payload = Staff.GetNextPayload(i + 1, spell_payload);
                        proj_comp.GiveSpellPayload(next_payload);
                        i += next_payload.Length;
                    }
                    break;

                case SpellType.Modifier:
                    // Stack modifiers
                    cur_modifiers.Add(spell_payload[i]);
                    break;
            }
        }
    }
}
