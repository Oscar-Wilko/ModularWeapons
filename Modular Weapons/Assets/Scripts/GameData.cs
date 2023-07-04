using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameData : MonoBehaviour
{
    // ----------------------------- JSON DATA ------------------------------------
    [System.Serializable]
    public class SpellData
    {
        public int ID;
        public string name;
        public float cast_delay;
        public float reload_delay;
        public int multicast_bonus;
        public string img_filename;
        public string type;
    }

    [System.Serializable]
    public class SpellDataList
    {
        public SpellData[] spell_data;
    }

    [System.Serializable]
    public class StaffData
    {
        public int ID;
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


    public TextAsset staff_json;
    public TextAsset spell_json;
    public StaffDataList staff_list = new StaffDataList();
    public SpellDataList spell_list = new SpellDataList();

    public bool in_inventory = false;
    public bool paused = false;
    public SpellInfo[] spell_inventory = new SpellInfo[8];
    public StaffInfo[] staff_inventory = new StaffInfo[4];
    readonly public SpellInfo blank_spell = new SpellInfo("blank", SpellType.Blank, 0.0f, 0.0f, 0, "Sprites/blank");
    readonly public StaffInfo blank_staff = new StaffInfo("blank", 0, 0, 0, new SpellInfo[4], "Sprites/blank");

    void Awake()
    {
        LoadJsonValues();

        spell_inventory = BlankSpells(8);
        for (int i = 0; i < 4; i ++)
        {
            staff_inventory[i] = GenerateStaff(StaffNameToID("Staff"));
        }

        spell_inventory[3] = new SpellInfo("default", SpellType.Projectile, 0.1f, 0.1f, 0, "Sprites/projectile_1");
    }

    void Update()
    {

    }

    public int StaffNameToID(string name)
    {
        foreach (StaffData data in staff_list.staff_data)
        {
            if (data.name == name) return data.ID;
        }
        return 0;
    }

    public StaffInfo GenerateStaff(int ID)
    {
        StaffData data = staff_list.staff_data[ID];
        SpellInfo[] spell_inventory = BlankSpells(data.inventory_size);
        return new StaffInfo(data.name, data.cast_delay, data.reload_delay, data.spells_per_shot, spell_inventory, "Sprites/"+data.img_filename);
    }

    public StaffInfo GenerateStaff(string name)
    {
        StaffData data = staff_list.staff_data[StaffNameToID(name)];
        SpellInfo[] spell_inventory = BlankSpells(data.inventory_size);
        return new StaffInfo(data.name, data.cast_delay, data.reload_delay, data.spells_per_shot, spell_inventory, "Sprites/"+data.img_filename);
    }

    private void LoadJsonValues()
    {
        staff_list = JsonUtility.FromJson<StaffDataList>(staff_json.text);
        spell_list = JsonUtility.FromJson<SpellDataList>(spell_json.text);
    }

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

    private SpellInfo[] BlankSpells(int size)
    {
        SpellInfo[] spells = new SpellInfo[size];
        for (int i = 0; i < size; i ++) spells[i] = blank_spell;
        return spells;
    }
}
