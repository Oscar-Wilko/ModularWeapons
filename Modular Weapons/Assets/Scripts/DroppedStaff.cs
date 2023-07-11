using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedStaff : MonoBehaviour
{
    private StaffInfo staffInfo;
    private GameData gameData;
    private Inventory inventory;
    public SpriteRenderer handle;
    public SpriteRenderer orb;
    public SpriteRenderer cover;
    public SpriteRenderer connector;

    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameData>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    public void GiveStaffData(StaffInfo new_info)
    {
        staffInfo = new_info;
        handle.sprite = Resources.Load<Sprite>(staffInfo.handle.img_filename);
        orb.sprite = Resources.Load<Sprite>(staffInfo.orb.img_filename);
        cover.sprite = Resources.Load<Sprite>(staffInfo.cover.img_filename);
        connector.sprite = Resources.Load<Sprite>(staffInfo.connector.img_filename);
    }

    public bool Interact()
    {
        if (inventory.PickUpStaff(staffInfo))
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
