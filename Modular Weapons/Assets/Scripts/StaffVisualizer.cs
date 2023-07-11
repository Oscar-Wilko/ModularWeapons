using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffVisualizer : MonoBehaviour
{
    public RawImage handle_image;
    public RawImage orb_image;
    public RawImage cover_image;
    public RawImage connector_image;
    public void UpdateVisual(StaffInfo staff_data)
    {
        handle_image.texture = Resources.Load<Texture2D>(staff_data.handle.img_filename);
        orb_image.texture = Resources.Load<Texture2D>(staff_data.orb.img_filename);
        cover_image.texture = Resources.Load<Texture2D>(staff_data.cover.img_filename);
        connector_image.texture = Resources.Load<Texture2D>(staff_data.connector.img_filename);
    }
}
