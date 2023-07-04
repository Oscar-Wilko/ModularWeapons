using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public GameObject staff;
    private float move_speed = 5.0f;
    private float interact_range = 2.0f;
    private GameData gameData;
    // Primary Inputs
    private KeyCode fire_input = KeyCode.Mouse0;
    private KeyCode left_input = KeyCode.A;
    private KeyCode up_input = KeyCode.W;
    private KeyCode right_input = KeyCode.D;
    private KeyCode down_input = KeyCode.S;
    private KeyCode interact_input = KeyCode.E;

    // Secondary Inputs
    private KeyCode s_left_input = KeyCode.LeftArrow;
    private KeyCode s_up_input = KeyCode.UpArrow;
    private KeyCode s_right_input = KeyCode.RightArrow;
    private KeyCode s_down_input = KeyCode.DownArrow;

    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(fire_input))
        {
            if (CanUseStaff())
            {
                // Tell Staff Script to attempt to Fire
                staff.GetComponent<Staff>().AttemptToFire();
            }
        }

        if (CanMove())
        {
            Vector3 move_vec = Vector3.zero;
            if (Input.GetKey(left_input) || Input.GetKey(s_left_input)) move_vec += Vector3.left;
            if (Input.GetKey(right_input) || Input.GetKey(s_right_input)) move_vec += Vector3.right;
            if (Input.GetKey(up_input) || Input.GetKey(s_up_input)) move_vec += Vector3.up;
            if (Input.GetKey(down_input) || Input.GetKey(s_down_input)) move_vec += Vector3.down;
            if (Input.GetKey(interact_input)) { Interact(); }

            this.transform.position += move_vec * move_speed * Time.deltaTime;
        }
    }

    private void Interact()
    {
        GameObject[] dropped_spells = GameObject.FindGameObjectsWithTag("DroppedSpell");
        foreach(GameObject spell in dropped_spells)
        {
            if (Vector3.Distance(transform.position, spell.transform.position) <= interact_range)
            {
                spell.GetComponent<DroppedSpell>().Interact();
            }
        }

        GameObject[] dropped_staffs = GameObject.FindGameObjectsWithTag("DroppedStaff");
        foreach(GameObject staff in dropped_staffs)
        {
            if (Vector3.Distance(transform.position, staff.transform.position) <= interact_range)
            {
                staff.GetComponent<DroppedStaff>().Interact();
            }
        }
    }

    private bool CanUseStaff()
    {
        // External checks in future here
        if (!CanMove()) return false;
        if (gameData.in_inventory) return false;

        return true;
    }

    private bool CanMove()
    {
        // External checks in future here
        if (gameData.paused) return false;
        return true;
    }
}
