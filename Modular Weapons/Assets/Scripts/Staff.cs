using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType
{
    Blank = 0,
    Projectile = 1,
    Modifier = 2,
    Multicast = 3
}    

public struct SpellInfo
{
    public SpellInfo(string _name, SpellType _type, float _cast_d, float _reload_d, int _multicast_bonus, string _img_filename)
    {
        name = _name;
        type = _type;
        cast_delay = _cast_d;
        reload_delay = _reload_d;
        multicast_bonus = _multicast_bonus;
        img_filename = _img_filename;
    }
    public string name;
    public SpellType type;
    public float cast_delay;
    public float reload_delay;
    public int multicast_bonus;
    public string img_filename;
}

public struct StaffInfo
{
    public StaffInfo(string _name, float _cast_d, float _reload_d, int _spell_per, SpellInfo[] _spell_inv, string _img_filename)
    {
        name = _name;
        base_cast_delay = _cast_d;
        base_reload_delay = _reload_d;
        spells_per_shot = _spell_per;
        spell_inventory = _spell_inv;
        img_filename = _img_filename;
    }
    public string name;
    public float base_cast_delay;
    public float base_reload_delay;
    public int spells_per_shot;
    public SpellInfo[] spell_inventory;
    public string img_filename;
}

public class Staff : MonoBehaviour
{
    // REFERENCES
    public GameObject projectile_prefab;        // Prefab for projectile object
    public Transform fire_position;             // Location of where projectiles are shot
    private GameData game_data;

    // FIRING INFO
    private int spell_tracker = 0;              // Tracker for which spell in the staff inventory it is currently on
    private int spells_to_cast = 0;             // Tracker for how many spells are left to cast
    private Queue<SpellInfo> mod_queue = new Queue<SpellInfo>();// Current queue of modifiers for future projectiles
    private float delay_tracker = 0.0f;         // Tracker to tell when to shoot
    private float reload_tracker = 0.0f;

    // ANIMATION VARIABLES
    private float max_dist_x = 1.0f;            // Max X offset of staff around player
    private float max_dist_y = 0.5f;            // Max Y offset of staff around player
    private float right_rot = 0;                // Z Rotation of staff at far right pos
    private float left_rot = 80;                // Z Rotation of staff at far left pos

    // OTHER
    private int selected_staff;                 // Staff number

    // Start is called before the first frame update
    void Start()
    {
        game_data = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameData>();
        UpdateStaffVisual();
    }

    // Update is called once per frame
    void Update()
    {
        // Update Timer
        delay_tracker -= Time.deltaTime;
        if (delay_tracker < 0) delay_tracker = 0;

        // Update position based on mouse position
        UpdateStaffPosition();
        
        // Cycle to next or previous staff
        if (Input.mouseScrollDelta.y != 0) CycleStaff(Input.mouseScrollDelta.y);
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ChangeStaff(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ChangeStaff(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ChangeStaff(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ChangeStaff(3); }
    }

    /// <summary>
    /// Go through the spell inventory as much as it can, adding modifiers and shooting projetiles
    /// </summary>
    public void AttemptToFire()
    {
        if (game_data.staff_inventory[selected_staff].name == "blank") return;
        spells_to_cast = game_data.staff_inventory[selected_staff].spells_per_shot;
        // Keep going through spells until no more spells to cast
        while (spells_to_cast > 0 && delay_tracker <= 0)
        {
            SpellInfo cur_spell = game_data.staff_inventory[selected_staff].spell_inventory[spell_tracker];
            Debug.Log(cur_spell);
            Debug.Log(cur_spell.type);
            // Depending on spell type
            switch (cur_spell.type)
            {
                case SpellType.Blank:
                    // Do nothing (maybe future features)
                    break;

                case SpellType.Projectile:
                    // Generate projectile with current modifiers
                    GameObject proj = Instantiate(projectile_prefab, fire_position.position, Quaternion.identity);
                    proj.GetComponent<Projectile>().GiveModifiers(mod_queue);
                    proj.GetComponent<Projectile>().ShootAtTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    spells_to_cast--;
                    if (spells_to_cast == 0)
                    {
                        mod_queue.Clear();
                    }
                    delay_tracker += game_data.staff_inventory[selected_staff].base_cast_delay;
                    reload_tracker += cur_spell.reload_delay;
                    break;

                case SpellType.Modifier:
                    // Stack modifiers
                    mod_queue.Enqueue(cur_spell);
                    break;

                case SpellType.Multicast:
                    // Update spells to cast
                    spells_to_cast += cur_spell.multicast_bonus - 1;
                    break;
            }

            // End loop depending on reaching final spell
            if (FinalSpell(spell_tracker))
            {
                delay_tracker += game_data.staff_inventory[selected_staff].base_reload_delay + reload_tracker;
                reload_tracker = 0;
                spell_tracker = 0;
            }
            // Or continue onto next spell slot
            else
            {
                spell_tracker++;
            }
        }
    }

    /// <summary>
    /// Calculates if the spell that was just checked is the last non-blank spell in the inventory
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Boolean of if the spell is last in the inventory</returns>
    private bool FinalSpell(int index)
    {
        int temp_tracker = 0;
        for (int i = index+1; i < game_data.staff_inventory[selected_staff].spell_inventory.Length; i ++)
        {
            if (game_data.staff_inventory[selected_staff].spell_inventory[i].type != SpellType.Blank) temp_tracker++;
        }
        return temp_tracker==0;
    }

    /// <summary>
    /// Update gameobject transform of staff based on mouse position
    /// </summary>
    private void UpdateStaffPosition()
    {
        // Get mouse offset from player
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.parent.position;
        // Lock the offset to a circle around the player
        difference.z = 0;
        difference = difference.normalized;
        // Calculate rotation based on its local x position
        float rot_z = (difference.x - 1) * (right_rot - left_rot) * 0.5f;
        this.transform.localEulerAngles= new Vector3(0,0,rot_z);
        this.transform.localPosition = new Vector3(difference.x * max_dist_x, difference.y * max_dist_y,difference.y);
    }

    /// <summary>
    /// Go to next or previous staff based on mouse scroll
    /// </summary>
    /// <param name="delta_scroll"></param>
    private void CycleStaff(float delta_scroll)
    {
        int staff_index = selected_staff;
        // Convert delta scroll into integer for pos/neg
        int direction = delta_scroll > 0 ? 1 : -1 ;

        staff_index+= direction;
        while (staff_index != selected_staff)
        {
            // Contain index within limits
            if (staff_index == 4) staff_index = 0;
            if (staff_index == -1) staff_index = 3;

            // Loop or return depending on blank staff
            if (game_data.staff_inventory[staff_index].name == "blank")
            {
                staff_index+= direction;
            }
            else
            {
                selected_staff = staff_index;
                spell_tracker = 0;
                reload_tracker = 0;
                mod_queue.Clear();
                UpdateStaffVisual();
                return;
            }
        }
    }

    private void ChangeStaff(int new_staff_index)
    {
        selected_staff = new_staff_index;
        spell_tracker = 0;
        reload_tracker = 0;
        mod_queue.Clear();
        UpdateStaffVisual();
    }

    /// <summary>
    /// Update staff if it should not appear based on selected staff type
    /// </summary>
    private void UpdateStaffVisual()
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = (game_data.staff_inventory[selected_staff].name != "blank");
        // Future modular visuals based on staff stats
    }
}
