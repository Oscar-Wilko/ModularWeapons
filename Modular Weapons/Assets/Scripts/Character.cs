using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Character : MonoBehaviour
{
    public GameObject staff;
    private float move_speed = 5.0f;
    private float interact_range = 1.0f;
    private GameData gameData;
    private Rigidbody2D rb;
    private Vector2 movement_input;
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
        rb = this.GetComponent<Rigidbody2D>();
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
            movement_input = Vector2.zero;
            if (Input.GetKey(left_input) || Input.GetKey(s_left_input)) movement_input += Vector2.left;
            if (Input.GetKey(right_input) || Input.GetKey(s_right_input)) movement_input += Vector2.right;
            if (Input.GetKey(up_input) || Input.GetKey(s_up_input)) movement_input += Vector2.up;
            if (Input.GetKey(down_input) || Input.GetKey(s_down_input)) movement_input += Vector2.down;
            if (Input.GetKeyDown(interact_input)) { Interact(); }
        }
    }

    private void FixedUpdate()
    {
        if (CanMove())
        {
            rb.MovePosition(rb.position + movement_input.normalized * move_speed * Time.fixedDeltaTime);
        }
    }

    private void Interact()
    {
        GameObject closest_interactable;

        // FOR DROPPED SPELLS
        closest_interactable = FindClosestObject(GameObject.FindGameObjectsWithTag("DroppedSpell"), interact_range);
        // If there is an object in range
        if (closest_interactable != null)
        {
            // Interact and exit
            if (closest_interactable.GetComponent<DroppedSpell>().Interact()) return;
        }

        // FOR DROPPED STAFFS
        closest_interactable = FindClosestObject(GameObject.FindGameObjectsWithTag("DroppedStaff"), interact_range);
        // If there is an object in range
        if (closest_interactable != null)
        {
            // Interact and exit
            if (closest_interactable.GetComponent<DroppedStaff>().Interact()) return;
        }
    }

    private GameObject FindClosestObject(GameObject[] objects, float range)
    { 
        float lowest_distance = -1;
        GameObject closest_object = null;
        // For each object in current pool
        foreach (GameObject obj in objects)
        {
            // If within range of interaction
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance <= interact_range)
            {
                // And the closest object so far
                if (distance <= lowest_distance || lowest_distance == -1)
                {
                    // Update new distance and object to this
                    lowest_distance = distance;
                    closest_object = obj;
                }
            }
        }
        // Return object (even if null)
        return closest_object;
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
