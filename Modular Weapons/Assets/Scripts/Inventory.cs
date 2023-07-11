using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // PREFABS
    public GameObject spell_prefab;
    public GameObject dropped_spell_prefab;
    public GameObject dropped_staff_prefab;

    // EXTERNAL DATA
    private GameData gameData;
    private Transform player_transform;
    private Staff staff_ref;

    // IMPORTANT DATA
    private Queue<GameObject> child_objects = new Queue<GameObject>();
    private KeyCode toggle_inventory = KeyCode.Tab;
    private int max_spell_slots = 16;
    private int max_spell_inv = 8;
    private int max_staff_inv = 4;
    public Vector2 slot_offset;

    // HELD STAFF & SPELL DATA
    public bool holding_staff = false;
    public bool holding_spell = false;
    public GameObject held_staff;
    public StaffInfo held_staff_info;
    public GameObject held_spell;
    public SpellInfo held_spell_info;

    // OBJECT REFERENCES
    public RawImage[] spell_inv_images;
    public StaffVisualizer[] staff_inv_images;
    public GameObject[] middle_staffs;
    public GameObject[,] staff_spell_images;
    public Transform[] staff_spell_anchors;

    // DEBUG
    public Texture2D temp_spell_img;
    public Texture2D temp_staff_img;

    void Awake()
    {
        // Reference generation
        gameData = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameData>();
        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        staff_ref = FindObjectOfType<Staff>();
        for (int i = 0; i < this.transform.childCount; i ++)
        {
            child_objects.Enqueue(this.transform.GetChild(i).gameObject);
        }
        // UI object generation
        GenerateUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Default values assigning
        held_spell_info = gameData.blank_spell;
        held_staff_info = gameData.blank_staff;

        // Update UI with generated values from GameData
        UpdateVisual();
        TextureRefresh();

        // Generate debug dropped spells
        DebugSpellGeneration();
    }

    // Update is called once per frame
    void Update()
    {
        // Inventory input
        if (Input.GetKeyDown(toggle_inventory))
        {
            gameData.in_inventory = !gameData.in_inventory;
            if (holding_staff) { DropHeldStaff(); }
            if (holding_spell) { DropHeldSpell(); }
            UpdateVisual();
        }
        // DEBUG
        if (Input.GetKeyDown(KeyCode.L))
        {
            TextureRefresh();
        }
        // Move held spell or staff
        Vector3 mouse_pos = Input.mousePosition;
        mouse_pos.z = 0;
        held_staff.transform.position = mouse_pos;
        held_spell.transform.position = mouse_pos;
        held_spell.SetActive(holding_spell);
        held_staff.SetActive(holding_staff);
    }

    /// <summary>
    /// Updates the active state of the inventory
    /// </summary>
    public void UpdateVisual()
    {
        foreach (GameObject child in child_objects)
        {
            child.SetActive(gameData.in_inventory);
        }
    }

    /// <summary>
    /// Refreshes the inventory completely
    /// </summary>
    public void TextureRefresh()
    {
        RefreshSpellVisual();
        RefreshStaffVisual();
    }

    /// <summary>
    /// Refreshes the spells on the top of the inventory
    /// </summary>
    private void RefreshSpellVisual()
    {
        for (int i = 0; i < max_spell_inv; i++)
        {
            spell_inv_images[i].texture = Resources.Load<Texture2D>(gameData.spell_inventory[i].img_filename);
        }
    }

    /// <summary>
    /// Refreshes all staffs
    /// </summary>
    private void RefreshStaffVisual()
    {
        for (int i = 0; i < max_staff_inv; i++) { RefreshIndividualStaff(i);}
    }

    /// <summary>
    /// Refreshes a chosen staff
    /// </summary>
    /// <param name="index">Staff number</param>
    private void RefreshIndividualStaff(int index)
    {
        // Update staff object being held by player
        staff_ref.UpdateStaffVisual();
        // Update staff images in UI
        staff_inv_images[index].UpdateVisual(gameData.staff_inventory[index]);
        middle_staffs[index].GetComponent<StaffVisualizer>().UpdateVisual(gameData.staff_inventory[index]);
        // Toggle active state based on it being a blank staff or not
        middle_staffs[index].SetActive(gameData.staff_inventory[index].name != gameData.blank_staff.name);
        // Update all slots active state and texture
        for (int j = 0; j < max_spell_slots; j++)
        {
            if (gameData.staff_inventory[index].spell_inventory.Length <= j)
            {
                staff_spell_images[index, j].SetActive(false);
            }
            else
            {
                staff_spell_images[index, j].SetActive(true);
                staff_spell_images[index, j].transform.GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture2D>(gameData.staff_inventory[index].spell_inventory[j].img_filename);
            }
        }
    }

    /// <summary>
    /// UI button click for spells
    /// </summary>
    /// <param name="index">Spell slot number clicked</param>
    public void ClickSpellInventory(int index)
    {
        if (holding_staff) return;

        // Swap spell info
        SpellInfo temp = held_spell_info;
        held_spell_info = gameData.spell_inventory[index];
        gameData.spell_inventory[index] = temp;

        // Swap image
        held_spell.transform.GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture2D>(held_spell_info.img_filename);
        spell_inv_images[index].texture = Resources.Load<Texture2D>(gameData.spell_inventory[index].img_filename);

        holding_spell = held_spell_info.name != gameData.blank_spell.name;
    }

    /// <summary>
    /// UI button click for staffs
    /// </summary>
    /// <param name="index">Staff slot number clicked</param>
    public void ClickStaffInventory(int index)
    {
        if (holding_spell) return;

        // Swap staff info
        StaffInfo temp = held_staff_info;
        held_staff_info = gameData.staff_inventory[index];
        gameData.staff_inventory[index] = temp;

        // Swap image
        held_staff.GetComponent<StaffVisualizer>().UpdateVisual(held_staff_info);
        staff_inv_images[index].UpdateVisual(temp);

        holding_staff = held_staff_info.name != gameData.blank_staff.name;
        RefreshIndividualStaff(index);
    }

    /// <summary>
    /// UI button clicked for spells within staff inventory
    /// </summary>
    /// <param name="index">Spell slot number</param>
    /// <param name="wand_index">Staff number</param>
    private void StaffSpellClick(int index, int wand_index)
    {
        if (holding_staff) return;

        // Swap spell info
        SpellInfo temp = held_spell_info;
        held_spell_info = gameData.staff_inventory[wand_index].spell_inventory[index];
        gameData.staff_inventory[wand_index].spell_inventory[index] = temp;

        // Swap image
        held_spell.transform.GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture2D>(held_spell_info.img_filename);
        staff_spell_images[wand_index, index].transform.GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture2D>(gameData.staff_inventory[wand_index].spell_inventory[index].img_filename);
        holding_spell = held_spell_info.name != gameData.blank_spell.name;
    }

    /// <summary>
    /// Create a dropped staff with info of held staff
    /// </summary>
    private void DropHeldStaff()
    {
        if (!holding_staff) return;

        // Create prefab and give its data
        GameObject staff = Instantiate(dropped_staff_prefab, player_transform.position, Quaternion.identity);
        staff.name = "Dropped Staff";
        staff.GetComponent<DroppedStaff>().GiveStaffData(held_staff_info);
        // Revert held staff info
        holding_staff = false;
        held_staff_info = gameData.blank_staff;
    }

    /// <summary>
    /// Create a dropped spell with info of held spell
    /// </summary>
    private void DropHeldSpell()
    {
        if (!holding_spell) return;

        // Create prefab and give its data
        GameObject spell = Instantiate(dropped_spell_prefab, player_transform.position, Quaternion.identity);
        spell.name = "Dropped Spell";
        spell.GetComponent<DroppedSpell>().GiveSpellData(held_spell_info);
        // Revert held spell info
        holding_spell = false;
        held_spell_info = gameData.blank_spell;
    }

    /// <summary>
    /// Click back UI panel
    /// </summary>
    public void ClickBackPanel()
    {
        DropHeldSpell();
        DropHeldStaff();
    }

    /// <summary>
    /// Create all spell slots in staff inventory for UI
    /// </summary>
    private void GenerateUI()
    {
        staff_spell_images = new GameObject[max_staff_inv,max_spell_slots];
        for (int i = 0; i < max_staff_inv; i++)
        {
            for (int j = 0; j < max_spell_slots; j++)
            {
                // Create slot prefab
                GameObject generated_slot = Instantiate(spell_prefab, Vector3.zero, Quaternion.identity);
                // Reset it's parent and local position
                generated_slot.transform.SetParent(staff_spell_anchors[i], false);
                generated_slot.transform.localPosition = new Vector3(j%4 * slot_offset.x, -(int)(j/4)*slot_offset.y,0);
                // Make the button component listen to StaffClick function with staff number and spell slot number
                int temp_int_i = i;
                int temp_int_j = j;
                generated_slot.GetComponent<Button>().onClick.AddListener(delegate { StaffSpellClick(temp_int_j, temp_int_i); } );
                staff_spell_images[i,j] = generated_slot;
            }
        }
    }

    /// <summary>
    /// Pick up dropped spell prefab
    /// </summary>
    /// <param name="spell_info">SpellInfo from the dropped spell</param>
    /// <returns>Boolean if it had picked up spell or not</returns>
    public bool PickUpSpell(SpellInfo spell_info)
    {
        // Foreach spell slot
        for (int i = 0; i < max_spell_inv; i ++)
        {
            // If there is a blank space
            if (gameData.spell_inventory[i].name == gameData.blank_spell.name)
            {
                // Then set that space to new spell data and update texture
                gameData.spell_inventory[i] = spell_info;
                spell_inv_images[i].texture = Resources.Load<Texture2D>(spell_info.img_filename);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Pick up dropped staff prefab
    /// </summary>
    /// <param name="spell_info">StaffInfo from the dropped staff</param>
    /// <returns>Boolean if it had picked up staff or not</returns>
    public bool PickUpStaff(StaffInfo staff_info)
    {
        // Foreach staff slot
        for (int i = 0; i < max_staff_inv; i ++)
        {
            // If there is a blank space
            if (gameData.staff_inventory[i].name == gameData.blank_staff.name)
            {
                // THen set that space to new staff data and update texture.
                gameData.staff_inventory[i] = staff_info;
                RefreshIndividualStaff(i);
                return true;
            }
        }
        return false;
    }

    private void DebugSpellGeneration()
    {
        for (int i = 0; i < gameData.spell_list.spell_data.Length; i ++)
        {
            GameObject spell = Instantiate(dropped_spell_prefab, new Vector3((i%5)*2,(int)i/5,0), Quaternion.identity);
            spell.name = "Dropped Spell";
            spell.GetComponent<DroppedSpell>().GiveSpellData(gameData.GenerateSpell(i));
        }
    }
}
