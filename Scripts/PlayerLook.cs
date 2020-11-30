using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerLook : MonoBehaviourPunCallbacks
{    

    public Transform player;
    public Transform cams;
    public Transform weapon;

    public float ySensitivity;
    public float xSensitivity;
    public float maxAngle;

    private Quaternion camCenter;

    public static bool cursorLocked = true;
   
    // Start is called before the first frame update
    void Start()
    {
        camCenter = cams.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        SetY();
        SetX();
      
        CursorLock();
    }

    void SetY()
    {
        float t_input = Input.GetAxisRaw("Mouse Y") * ySensitivity;

        Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);

        Quaternion t_delta = cams.localRotation * t_adj;

        if (Quaternion.Angle(camCenter, t_delta) < maxAngle)
        {
            cams.localRotation = t_delta;
            
        }
        weapon.rotation = cams.rotation;
    }

    void SetX()
    {
        float t_input = Input.GetAxisRaw("Mouse X") * xSensitivity;
       
        Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
      
        Quaternion t_delta = player.localRotation * t_adj;
    
        player.rotation = t_delta;
    }

    void CursorLock()
    {
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = true;
            }
        }
    }
}
