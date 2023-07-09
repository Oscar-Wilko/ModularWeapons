using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Transform focal_point;
    public Transform player_transform;
    private float mouse_influence = 2.0f;
    private float cam_transition_scale = 0.01f; // Between 1.0 -> 0.0
    private Vector3 current_track_point;
    // Start is called before the first frame update
    void Start()
    {
        focal_point = player_transform;
    }

    // Change targeted transform for camera
    public void ChangeFocalPoint(Transform new_point)
    {
        focal_point = new_point;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate offset from camera and mouse position
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - focal_point.position;
        current_track_point = (difference.normalized * mouse_influence) + focal_point.position;
        current_track_point.z = -10;

        // Move camera to position gradually
        this.transform.position = this.transform.position + (current_track_point - this.transform.position) * cam_transition_scale;
    }
}
