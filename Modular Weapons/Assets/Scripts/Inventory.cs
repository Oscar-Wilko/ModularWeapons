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
    public RawImage[] staff_inv_images;
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
        TextureUpdate();
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
            TextureUpdate();
        }
        // Move held spell or staff
        Vector3 mouse_pos = Input.mousePosition;
        mouse_pos.z = 0;
        held_staff.transform.position = mouse_pos;
        held_spell.transform.position = mouse_pos;
        held_spell.SetActive(holding_spell);
        held_staff.SetActive(holding_staff);
    }

    public void UpdateVisual()
    {
        foreach (GameObject child in child_objects)
        {
            child.SetActive(gameData.in_inventory);
        }
    }

    public void TextureUpdate()
    {
        UpdateSpellVisual();
        UpdateStaffVisual();
    }

    private void UpdateSpellVisual()
    {
        for (int i = 0; i < max_spell_inv; i++)
        {
            spell_inv_images[i].texture = Resources.Load<Texture2D>(gameData.spell_inventory[i].img_filename);
        }
    }

    private void UpdateStaffVisual()
    {
        for (int i = 0; i < max_staff_inv; i++)
        {
            UpdateIndividualStaff(i);
        }
    }

    private void UpdateIndividualStaff(int index)
    {
        staff_inv_images[index].texture = Resources.Load<Texture2D>(gameData.staff_inventory[index].img_filename);
        middle_staffs[index].GetComponent<RawImage>().texture = Resources.Load<Texture2D>(gameData.staff_inventory[index].img_filename);
        middle_staffs[index].SetActive(gameData.staff_inventory[index].name != gameData.blank_staff.name);
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

    public void ClickStaffInventory(int index)
    {
        if (holding_spell) return;

        // Swap staff info
        StaffInfo temp = held_staff_info;
        held_staff_info = gameData.staff_inventory[index];
        gameData.staff_inventory[index] = temp;

        // Swap image
        held_staff.transform.GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture2D>(held_staff_info.img_filename);
        staff_inv_images[index].texture = Resources.Load<Texture2D>(gameData.staff_inventory[index].img_filename);
        holding_staff = held_staff_info.name != gameData.blank_staff.name;

        UpdateIndividualStaff(index);
    }

    private void StaffClick(int index, int wand_index)
    {
        if (holding_staff) return;

        // Swap spell info
        SpellInfo temp = held_spell_info;
        held_spell_info = gameData.staff_inventory[wand_index].spell_inventory[index];
        gameData.staff_inventory[wand_index].spell_inventory[index] = temp;

        // Swap image
        held_staff.transform.GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture2D>(held_staff_info.img_filename);

        staff_spell_images[wand_index, index].transform.GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture2D>(gameData.staff_inventory[wand_index].spell_inventory[index].img_filename);
        holding_spell = held_spell_info.name != gameData.blank_spell.name;
    }

    private void DropHeldStaff()
    {
        if (!holding_staff) return;

        GameObject staff = Instantiate(dropped_staff_prefab, player_transform.position, Quaternion.identity);
        staff.GetComponent<DroppedStaff>().GiveStaffData(held_staff_info);
        holding_staff = false;
        held_staff_info = gameData.blank_staff;
    }

    private void DropHeldSpell()
    {
        if (!holding_spell) return;

        GameObject spell = Instantiate(dropped_spell_prefab, player_transform.position, Quaternion.identity);
        spell.GetComponent<DroppedSpell>().GiveSpellData(held_spell_info);
        holding_spell = false;
        held_spell_info = gameData.blank_spell;
    }

    public void ClickBackPanel()
    {
        DropHeldSpell();
        DropHeldStaff();
    }

    private void GenerateUI()
    {
        staff_spell_images = new GameObject[max_staff_inv,max_spell_slots];
        for (int i = 0; i < max_staff_inv; i++)
        {
            for (int j = 0; j < max_spell_slots; j++)
            {
                GameObject generated_slot = Instantiate(spell_prefab, Vector3.zero, Quaternion.identity);
                generated_slot.transform.SetParent(staff_spell_anchors[i], false);
                generated_slot.transform.localPosition = new Vector3(j%4 * slot_offset.x, -(int)(j/4)*slot_offset.y,0);
                int temp_int_i = i;
                int temp_int_j = j;
                generated_slot.GetComponent<Button>().onClick.AddListener(delegate { StaffClick(temp_int_j, temp_int_i); } );
                staff_spell_images[i,j] = generated_slot;
            }
        }
    }

    public bool PickUpSpell(SpellInfo spell_info)
    {
        for (int i = 0; i < max_spell_inv; i ++)
        {
            if (gameData.spell_inventory[i].name == gameData.blank_spell.name)
            {
                gameData.spell_inventory[i] = spell_info;
                spell_inv_images[i].texture = Resources.Load<Texture2D>(spell_info.img_filename);
                return true;
            }
        }
        return false;
    }

    public bool PickUpStaff(StaffInfo staff_info)
    {
        for (int i = 0; i < max_staff_inv; i ++)
        {
            if (gameData.staff_inventory[i].name == gameData.blank_staff.name)
            {
                gameData.staff_inventory[i] = staff_info;
                staff_inv_images[i].texture = Resources.Load<Texture2D>(staff_info.img_filename);
                UpdateIndividualStaff(i);
                return true;
            }
        }
        return false;
    }
}
