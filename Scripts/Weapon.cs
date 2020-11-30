using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Weapon : MonoBehaviourPunCallbacks
{
    public Gun[] loadOut;

    public Transform weaponParent;
    
    private GameObject currentWeapon;
    private int currentIndex;

    private float currentCooldown;
    public GameObject bulletHolePrefab;
    public LayerMask canBeShot;
    public bool isAiming;

    private Quaternion originRotatation;


    private void Start()
    {
        originRotatation = transform.localRotation;
    }
    void Update()
    {
        
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1))
        {
            photonView.RPC("Equip", RpcTarget.All, 0);
        }

        if (currentWeapon != null)
        {
            if (photonView.IsMine)
            {
                Aim(Input.GetMouseButton(1));

                if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
                {
                    photonView.RPC("Shoot", RpcTarget.All);
                }
                if (currentCooldown > 0)
                {
                    currentCooldown -= Time.deltaTime;
                }
            }

            // weapon position elasticy

            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            currentWeapon.transform.localRotation = Quaternion.Lerp(currentWeapon.transform.localRotation, originRotatation, Time.deltaTime * 4f);

            
        }


    }

    [PunRPC]
    void Equip(int p_Ind)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
       
        currentIndex = p_Ind;

        GameObject t_newWeapon = Instantiate(loadOut[p_Ind].prefab,weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        t_newWeapon.transform.localPosition = Vector3.zero;
        t_newWeapon.transform.localEulerAngles = Vector3.zero;
        

        currentWeapon = t_newWeapon;
        currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
    }

    void Aim( bool p_isAiming)
    {
        Transform t_anchor = currentWeapon.transform.Find("Anchor");
        Transform t_state_ads = currentWeapon.transform.Find("States/Ads");
        Transform t_state_hip = currentWeapon.transform.Find("States/Hip");

        if (p_isAiming)
        {
            //aim
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.deltaTime * loadOut[currentIndex].aimSpeed);
            isAiming = true;
        }
        else
        {
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * loadOut[currentIndex].aimSpeed);
            isAiming = false;
        }

    }

    [PunRPC]
    void Shoot()
    {
        Transform t_spawn = transform.Find("Cameras/FpsCam");

            //bloom
            Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
            t_bloom += Random.Range(-loadOut[currentIndex].bloom, loadOut[currentIndex].bloom) * t_spawn.up;
            t_bloom += Random.Range(-loadOut[currentIndex].bloom, loadOut[currentIndex].bloom) * t_spawn.right;
            t_bloom -= t_spawn.position;
            t_bloom.Normalize();


            // cooldown            
            currentCooldown = loadOut[currentIndex].fireRate;
       
       
        //raycast
        RaycastHit t_hit = new RaycastHit();
        if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
        {
            GameObject t_newHole = Instantiate(bulletHolePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
            t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
            Destroy(t_newHole, 5f);

            if (photonView.IsMine)
            {
                //shooting other player
                if (t_hit.collider.transform.gameObject.layer == 11)
                {
                    t_hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadOut[currentIndex].damage);
                    Destroy(t_newHole);
                }
            }
        }

        //gun fx
        if (photonView.IsMine)
        {
            currentWeapon.transform.Rotate(-loadOut[currentIndex].recoil, 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadOut[currentIndex].kickback;
        }

    }


    [PunRPC]
    private void TakeDamage(int p_damage)
    {
        GetComponent<Player>().TakeDamage(p_damage);
    }

}
