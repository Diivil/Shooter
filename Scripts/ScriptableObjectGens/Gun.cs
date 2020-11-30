using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]

public class Gun : ScriptableObject
{
    public string gunName;
    public int    damage;
    public float  fireRate;
    public float  aimSpeed;
    public float  bloom;
    public float  recoil;
    public float  kickback;
    public GameObject prefab;
}
