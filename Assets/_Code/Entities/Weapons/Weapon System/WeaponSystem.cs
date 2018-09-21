using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public enum WeaponId : int
{
    Null = -1,
    Pistol,
    Shotgun,
}

public class WeaponSystem : MonoBehaviour
{
    Gun[] ownedWeapons;
    Gun equippedWeapon;

    public readonly int TOTAL_WEAPONS_IN_GAME = Enum.GetNames(typeof(WeaponId)).Length - 1;

    private void Awake()
    {
        ownedWeapons = new Gun[TOTAL_WEAPONS_IN_GAME];   // Initialize Array Correct Size
        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        Gun[] startingWeapons = GetComponentsInChildren<Gun>();

        // Add Each Child Weapon To Our Weapons Array
        foreach (Gun weapon in startingWeapons)
        {
            byte index = (byte) weapon.gunProperties.weaponId;

            if (ownedWeapons[index] == null)
            {
                ownedWeapons[index] = weapon;
                weapon.PutAway();
            }
        }

        WeaponId firstOwnedWeapon = FindFirstOwnedWeaponSlot();
        EquipWeapon(firstOwnedWeapon);
    }

    public WeaponId FindFirstOwnedWeaponSlot()
    {
        for (int i = 0; i < ownedWeapons.Length; i++)
            if (ownedWeapons[i] != null)
                return (WeaponId) i;
        return WeaponId.Null;
    }

    public void SpawnWeaponInSystem(Gun m_PrimaryWeapon)
    {
        GameObject newGun = Instantiate(m_PrimaryWeapon.gameObject, transform);
        PickupWeapon(newGun);
    }

    /// <summary>
    /// Attempt to equip a weapon with the given id
    /// </summary>
    /// <param name="weaponId">Id Of The weapon to try to equip</param>
    /// <returns>If the given weaponId is owned or not, and was properly equipped</returns>
    public bool EquipWeapon(WeaponId weaponId)
    {
        if (weaponId == WeaponId.Null)
            return false;
        if (equippedWeapon != null && weaponId == equippedWeapon.gunProperties.weaponId)
            return false;

        if (ownedWeapons[(int)weaponId] != null)
        {
            if (equippedWeapon != null)
            {
                PutEquippedAway();
            }

            equippedWeapon = ownedWeapons[(int)weaponId];
            equippedWeapon.gameObject.SetActive(true);
            ResetWeaponTransform(equippedWeapon);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Pickup a GameObject containing a GUN component. If the weapon is already owned it will be ignored.
    /// In addition if we are picking it up then its transform and rigidbody is reset, its added to the 
    /// weapon systems' owned items, its collisions are disabled, and it is equipped if we do not have
    /// an equipped weapon.
    /// </summary>
    /// <param name="gameObject">GameObject we are trying to pick up. Must contain a Gun component</param>
    /// <returns>If the weapon was picked up or not</returns>
    public bool PickupWeapon(GameObject gameObject)
    {
        Gun weaponToPickup = gameObject.GetComponentInParent<Gun>();
        if (weaponToPickup == null) { Debug.LogWarning("Trying to pickup weapon that is not a Gun"); return false; }

        WeaponId id = weaponToPickup.gunProperties.weaponId;

        if (AlreadyOwnWeapon(id)) { Debug.Log("Already own a wapon of this type: " + id); return false; };

        ResetWeaponTransform(weaponToPickup);
        weaponToPickup.transform.SetParent(transform, true);

        ownedWeapons[(int) id] = weaponToPickup;

        weaponToPickup.DisableCollisions();

        if (equippedWeapon == null)
        {
            EquipWeapon(id);
        }
        else
        {
            weaponToPickup.gameObject.SetActive(false);
        }

        return true;
    }

    private void ResetWeaponTransform(Gun weap)
    {
        weap.transform.position = transform.position;
        weap.transform.rotation = transform.rotation;
        weap.transform.localScale = transform.localScale;
    }

    private bool AlreadyOwnWeapon(WeaponId weaponId)
    {
        return ownedWeapons[(int) weaponId] != null;
    }

    private void PutEquippedAway()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.PutAway();
            equippedWeapon = null;
        }
    }

    /// <summary>
    /// Throw the equipped weapon away. Use the velocity to chuck the weapon.
    /// </summary>
    /// <param name="throwVel">Velocity to throw the weapon at.</param>
    public void DropWeapon(Vector3 throwVel)
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.DiscardAndThrow(throwVel);                           // Throw Weapon
            ownedWeapons[(int)equippedWeapon.gunProperties.weaponId] = null;    // Empty Slot
            equippedWeapon.transform.parent = null;
            equippedWeapon = null;                                              // Get rid of reference to equipped.

            WeaponId nextWeaponUp = FindFirstOwnedWeaponSlot();
            if (!EquipWeapon(nextWeaponUp))
            {
                Debug.Log("No More Weapons");
                equippedWeapon = null;
            }
        }
    }

    /// <summary>
    /// Attempt to use the weapons Attack. For a Gun this is shooting
    /// </summary>
    /// <returns>If an attack was successfully performed</returns>
    public bool AttemptAttack()
    {
        if (equippedWeapon != null)
            return equippedWeapon.Attack();

        return false;
    }

    public void LogWeaponSystemState()
    {
        Debug.Log("---------- Weapon System State ---------\n" +
                  "Equipped: " + equippedWeapon + "\n");

        for (int i = 0; i < ownedWeapons.Length; i++)
        {
            Debug.Log("Owned Weapon " + ((WeaponId)i)  + " " + (ownedWeapons[i] != null));
        }
    }
}
