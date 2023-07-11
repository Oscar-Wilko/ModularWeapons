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
    public class HandleData
    {
        public string name;
        public float cast_delay;
        public float reload_delay;
        public string img_filename;
    }
    [System.Serializable]
    public class OrbData
    {
        public string name;
        public int capacity;
        public string img_filename;
    }
    [System.Serializable]
    public class CoverData
    {
        public string name;
        public int spells_per_shot;
        public string img_filename;
    }
    [System.Serializable]
    public class ConnectorData
    {
        public string name;
        public string img_filename;
    }

    [System.Serializable]
    public class StaffDataList
    {
        public HandleData[] handle_data;
        public OrbData[] orb_data;
        public CoverData[] cover_data;
        public ConnectorData[] connector_data;
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
    public SpellInfo blank_spell                = new SpellInfo("blank", SpellType.Blank, 0.0f, 0.0f, 0, "Sprites/blank", DecayType.Default);
    public StaffHandleInfo blank_handle         = new StaffHandleInfo("blank", 0, 0, "Sprites/blank");
    public StaffOrbInfo blank_orb               = new StaffOrbInfo("blank", 0, "Sprites/blank");
    public StaffCoverInfo blank_cover           = new StaffCoverInfo("blank", 0, "Sprites/blank");
    public StaffConnectorInfo blank_connector   = new StaffConnectorInfo("blank", "Sprites/blank");
    public StaffInfo blank_staff;

    /// <summary>
    /// Generate default values and inventory
    /// </summary>
    void Awake()
    {
        blank_staff = new StaffInfo("blank", new SpellInfo[0], blank_handle, blank_orb, blank_cover, blank_connector);

        LoadJsonValues();

        // Generate spell inventory of blanks
        spell_inventory = BlankSpells(8);

        // Generate staff inventory of blanks
        for (int i = 0; i < 4; i ++)
        {
            staff_inventory[i] = blank_staff;
        }

        // Give player starting staffs and spells
        staff_inventory[0] = GenerateStaff(0,0,0,0);
        staff_inventory[1] = GenerateStaff(0,1,0,1);
        staff_inventory[2] = GenerateStaff(1,0,1,1);
        for (int i = 0; i < 8;i ++)
        {
            spell_inventory[i] = GenerateSpell(i);
        }
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
    public StaffInfo GenerateStaff(int handle_ID, int orb_ID, int cover_ID, int connector_ID)
    {
        if (handle_ID < 0 || handle_ID > staff_list.handle_data.Length) return blank_staff;
        if (orb_ID < 0 || orb_ID > staff_list.handle_data.Length) return blank_staff;
        if (cover_ID < 0 || cover_ID > staff_list.handle_data.Length) return blank_staff;
        if (connector_ID < 0 || connector_ID > staff_list.handle_data.Length) return blank_staff;

        StaffHandleInfo handle = GenerateHandle(handle_ID);
        StaffOrbInfo orb = GenerateOrb(orb_ID);
        StaffCoverInfo cover = GenerateCover(cover_ID);
        StaffConnectorInfo connector = GenerateConnector(connector_ID);
        return new StaffInfo("Staff", BlankSpells(orb.capacity), handle, orb, cover, connector);
    }

    /// <summary>
    /// Generate staff handle based on JSON values
    /// </summary>
    /// <param name="ID">Handle ID</param>
    /// <returns>StaffHandleInfo of newly generated handle</returns>
    public StaffHandleInfo GenerateHandle(int ID)
    {
        if (ID < 0 || ID > staff_list.handle_data.Length) return blank_handle;
        HandleData data = staff_list.handle_data[ID];
        return new StaffHandleInfo(data.name, data.cast_delay, data.reload_delay, "Sprites/" + data.img_filename);
    }

    /// <summary>
    /// Generate staff orb based on JSON values
    /// </summary>
    /// <param name="ID">Orb ID</param>
    /// <returns>StaffOrbInfo of newly generated handle</returns>
    public StaffOrbInfo GenerateOrb(int ID)
    {
        if (ID < 0 || ID > staff_list.orb_data.Length) return blank_orb;
        OrbData data = staff_list.orb_data[ID];
        return new StaffOrbInfo(data.name, data.capacity, "Sprites/" + data.img_filename);
    }

    /// <summary>
    /// Generate staff cover based on JSON values
    /// </summary>
    /// <param name="ID">Cover ID</param>
    /// <returns>StaffCoverInfo of newly generated handle</returns>
    public StaffCoverInfo GenerateCover(int ID)
    {
        if (ID < 0 || ID > staff_list.cover_data.Length) return blank_cover;
        CoverData data = staff_list.cover_data[ID];
        return new StaffCoverInfo(data.name, data.spells_per_shot, "Sprites/" + data.img_filename);
    }

    /// <summary>
    /// Generate staff connector based on JSON values
    /// </summary>
    /// <param name="ID">Connector ID</param>
    /// <returns>StaffConnectorInfo of newly generated handle</returns>
    public StaffConnectorInfo GenerateConnector(int ID)
    {
        if (ID < 0 || ID > staff_list.connector_data.Length) return blank_connector;
        ConnectorData data = staff_list.connector_data[ID];
        return new StaffConnectorInfo(data.name, "Sprites/" + data.img_filename);
    }

    /// <summary>
    /// Generate spell based on JSON values
    /// </summary>
    /// <param name="ID">ID of spell</param>
    /// <returns>SpellInfo of newly generated spell</returns>
    public SpellInfo GenerateSpell(int ID)
    {
        if (ID < 0 || ID > spell_list.spell_data.Length) return blank_spell;
        SpellData data = spell_list.spell_data[ID];
        return new SpellInfo(data.name, (SpellType)data.type, data.cast_delay, data.reload_delay, data.multicast_bonus, "Sprites/"+data.img_filename, (DecayType)data.decay_type);
    }

    /// <summary>
    /// Generate spell based on JSON values
    /// </summary>
    /// <param name="name">Name of spell</param>
    /// <returns>SpellInfo of newly generated spell</returns>
    public SpellInfo GenerateSpell(string name)
    {
        return GenerateSpell(SpellNameToID(name));
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
