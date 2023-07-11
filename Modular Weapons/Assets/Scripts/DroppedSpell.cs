using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedSpell : MonoBehaviour
{
    private SpellInfo spellInfo;
    private GameData gameData;
    private Inventory inventory;

    // Start is called before the first frame update
    void Awake()
    {
        gameData = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameData>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    public void GiveSpellData(SpellInfo new_info)
    {
        spellInfo = new_info;
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spellInfo.img_filename);
    }

    public bool Interact()
    {
        if (inventory.PickUpSpell(spellInfo))
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
