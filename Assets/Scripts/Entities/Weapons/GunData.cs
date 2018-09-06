using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "GunData", menuName = "Game/GunData")]
public class GunData : ScriptableObject
{
    [SerializeField] public WeaponId weaponId;
    [SerializeField] public float    fireRate     = 5f;
    [SerializeField] public float    reloadRate   = 4f;
    [SerializeField] public int      shotsPerClip = 5;
    [SerializeField] public Projectile projectile;
}
