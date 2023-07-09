using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameData : MonoBehaviour
{
    // ----------------------------- JSON DATA ------------------------------------
    [System.Serializable]
    public class SpellData
    {
        public string name;
        public float cast_delay;
        public float reload_delay;
        public int multicast_bonus;
        public string img_filename;
        public int type;
        public int decay_type;
    }

    [System.Serializable]
    public class SpellDataList
    {
        public SpellData[] spell_data;
    }

    [System.Serializable]
    public class StaffData
    {
        public string name;
        public float cast_delay;
        public float reload_delay;
        public int spells_per_shot;
        public int inventory_size;
        public string img_filename;
    }

    [System.Serializable]
    public class StaffDataList
    {
        public StaffData[] staff_data;
    }
    // ----------------------------- JSON DATA ------------------------------------

    // Spell and staff data
    public TextAsset staff_json;
    public TextAsset spell_json;
    public StaffDataList staff_list = new StaffDataList();
    public SpellDataList spell_list = new SpellDataList();

    // Tracked data
    public bool in_inventory = false;
    public bool paused = false;

    // Inventory data
    public SpellInfo[] spell_inventory = new SpellInfo[8];
    public StaffInfo[] staff_inventory = new StaffInfo[4];

    // Default variables
    public SpellInfo blank_spell = new SpellInfo("blank", SpellType.Blank, 0.0f, 0.0f, 0, "Sprites/blank", DecayType.Default);
    public StaffInfo blank_staff = new StaffInfo("blank", 0, 0, 0, new SpellInfo[4], "Sprites/blank");

    /// <summary>
    /// Generate default values and inventory
    /// </summary>
    void Awake()
    {
        LoadJsonValues();

        // Generate spell inventory of blanks
        spell_inventory = BlankSpells(8);

        // Generate staff inventory of blanks
        for (int i = 0; i < 4; i ++)
        {
            staff_inventory[i] = blank_staff;
        }

        // Give player starting staffs and spells
        staff_inventory[0] = GenerateStaff(0);
        for (int i = 0; i < 8;i ++)
        {
            spell_inventory[i] = GenerateSpell(i);
        }
    }

    /// <summary>
    /// Converts staff name to it's ID value
    /// </summary>
    /// <param name="name">Name of staff</param>
    /// <returns>ID of staff</returns>
    public int StaffNameToID(string name)
    {
        for (int i =0; i < staff_list.staff_data.Length; i++)
        {
            if (staff_list.staff_data[i].name == name) return i;
        }
        return -1;
    }
    
    /// <summary>
    /// Converts spell name to it's ID value
    /// </summary>
    /// <param name="name">Name of spell</param>
    /// <returns>ID of staff</returns>
    public int SpellNameToID(string name)
    {
        for (int i =0; i < spell_list.spell_data.Length; i++)
        {
            if (spell_list.spell_data[i].name == name) return i;
        }
        return -1;
    }

    /// <summary>
    /// Generate staff based on JSON values
    /// </summary>
    /// <param name="ID">ID of staff</param>
    /// <returns>StaffInfo of newly generated staff</returns>
    public StaffInfo GenerateStaff(int ID)
    {
        if (ID < 0 || ID > staff_list.staff_data.Length) return blank_staff;
        StaffData data = staff_list.staff_data[ID];
        SpellInfo[] spell_inventory = BlankSpells(data.inventory_size);
        return new StaffInfo(data.name, data.cast_delay, data.reload_delay, data.spells_per_shot, spell_inventory, "Sprites/"+data.img_filename);
    }

    /// <summary>
    /// Generate staff based on JSON values
    /// </summary>
    /// <param name="name">Name of staff</param>
    /// <returns>StaffInfo of newly generated staff</returns>
    public StaffInfo GenerateStaff(string name)
    {
        return GenerateStaff(StaffNameToID(name));
    }
    
    /// <summary>
    /// Generate staff based on JSON values
    /// </summary>
    /// <param name="ID">ID of staff</param>
    /// <returns>StaffInfo of newly generated staff</returns>
    public SpellInfo GenerateSpell(int ID)
    {
        if (ID < 0 || ID > spell_list.spell_data.Length) return blank_spell;
        SpellData data = spell_list.spell_data[ID];
        return new SpellInfo(data.name, (SpellType)data.type, data.cast_delay, data.reload_delay, data.multicast_bonus, "Sprites/"+data.img_filename, (DecayType)data.decay_type);
    }

    /// <summary>
    /// Generate staff based on JSON values
    /// </summary>
    /// <param name="name">Name of staff</param>
    /// <returns>StaffInfo of newly generated staff</returns>
    public StaffInfo GenerateSpell(string name)
    {
        return GenerateStaff(SpellNameToID(name));
    }

    /// <summary>
    /// Load spell and staff data from JSON file
    /// </summary>
    private void LoadJsonValues()
    {
        staff_list = JsonUtility.FromJson<StaffDataList>(staff_json.text);
        spell_list = JsonUtility.FromJson<SpellDataList>(spell_json.text);
    }

    /// <summary>
    /// Inventory console output
    /// </summary>
    private void DebugOutput()
    {
        string debug_str = "";
        for (int i = 0; i < 4; i++)
        {
            debug_str += staff_inventory[i].name + ": ";
            for (int j = 0; j < 4; j++)
            {
                debug_str += staff_inventory[i].spell_inventory[j].name + " ";
            }
            debug_str += ".\n";
        }
        Debug.Log(debug_str);
    }

    /// <summary>
    /// Generates array of spells that are blank for staffs
    /// </summary>
    /// <param name="size">Size of intended array</param>
    /// <returns>Array of SpellInfo that hold the spells</returns>
    private SpellInfo[] BlankSpells(int size)
    {
        SpellInfo[] spells = new SpellInfo[size];
        for (int i = 0; i < size; i ++) spells[i] = blank_spell;
        return spells;
    }
}
