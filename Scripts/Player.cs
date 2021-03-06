﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    public CharacterController playerController;        


    public float speed;                     
    float sprintSpeed;                      
    public float sprintMultiplier;          
    public float gravity;                  
    Vector3 velocity;                       

    public Transform groundCheck;           
    public float groundDistance;            
    public LayerMask groundMask;            
    bool isGrounded;                        

    public GameObject cameraParent;        

    public int maxHealth;                  
    private int currentHealth;

    public float jumpHeight;
    private Manager manager;

    private Transform uiHealthbar;
    


    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();  
        
        currentHealth = maxHealth;  // setze HP gleich Max HP

        if (photonView.IsMine)
        {
            uiHealthbar = GameObject.Find("HUD/Health/Bar").transform;    // HP Bar GameObject
            RefreshHealthbar();                                           // Setze HP Bar auf Max HP
        }
        cameraParent.SetActive(photonView.IsMine);

        if (!photonView.IsMine)
        {
            gameObject.layer = 11;  // Setze gegner treffbar
        }

        if (Camera.main)
        {
            Camera.main.enabled = false; // Wenn Mein Camera aktiv dann deaktiviere
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        GroundCheck();
       
        Jump();
      
        Movement();
      
        Sprint();
     
        RefreshHealthbar();

        if (Input.GetKeyDown(KeyCode.U))
        {
            TakeDamage(25);
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
    }
    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");       //Input X-Achse
        float z = Input.GetAxisRaw("Vertical");         //Input y-Achse
            
        Vector3 move = transform.right * x + transform.forward * z;     


        playerController.Move(move * speed * sprintSpeed * Time.deltaTime);


        velocity.y -= gravity * Time.deltaTime;


        playerController.Move(velocity * Time.deltaTime);
    }

    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) /*&& isGrounded*/)
        {
            sprintSpeed = speed * sprintMultiplier;
        }

        else
        {
            sprintSpeed = speed;
        }
    }
    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); 

        if (isGrounded && velocity.y < 0)                   
        {
            velocity.y = -2f;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }

    }


    public void TakeDamage(int p_damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= p_damage;
            RefreshHealthbar();

            if (currentHealth <= 0)
            {
                manager.Spawn();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    void RefreshHealthbar()
    {
        float t_HealthRatio = (float)currentHealth / (float)maxHealth;
        uiHealthbar.localScale = Vector3.Lerp(uiHealthbar.localScale, new Vector3(t_HealthRatio,1,1), Time.deltaTime * 8f);
    }

}
