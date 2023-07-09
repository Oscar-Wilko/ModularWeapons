using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedStaff : MonoBehaviour
{
    private StaffInfo staffInfo;
    private GameData gameData;
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameData>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    public void GiveStaffData(StaffInfo new_info)
    {
        staffInfo = new_info;
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(staffInfo.img_filename);
    }

    public void Interact()
    {
        if (inventory.PickUpStaff(staffInfo))
        {
            Destroy(gameObject);
        }
    }
}
