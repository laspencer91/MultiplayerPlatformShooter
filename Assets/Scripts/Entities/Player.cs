using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] LayerMask  standardBlockLayer;
    [SerializeField] Gun        m_PrimaryWeapon;
    [SerializeField] Gun        m_SecondaryWeapon;

    [SerializeField] float runSpeed;
    [SerializeField] float jumpSpeed    = 3f;
    [SerializeField] float climbSpeed   = 1f;
    [SerializeField] float deathKick    = 5f;
    [SerializeField] float maxFallSpeed = 17f;

    PlayerState       playerState;
    Rigidbody2D       m_Rigidbody;
    CapsuleCollider2D m_Collider;
    Animator          m_Animator;
    WeaponSystem      m_WeaponSystem;

    float startingGravity;

    void Start ()
    {
        m_Rigidbody    = GetComponent<Rigidbody2D>();
        m_Animator     = GetComponent<Animator>();
        m_Collider     = GetComponent<CapsuleCollider2D>();
        m_WeaponSystem = GetComponentInChildren<WeaponSystem>();

        startingGravity = m_Rigidbody.gravityScale;
        playerState     = PlayerState.Alive;

        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        m_WeaponSystem.SpawnWeaponInSystem(m_PrimaryWeapon);
        m_WeaponSystem.SpawnWeaponInSystem(m_SecondaryWeapon);
    }

	public void ExecuteUpdate (PlayerInput input)
    {
        if (playerState != PlayerState.Alive) { return; }

        Run         (input.HorizontalAxisInput);
        Jump        (input.JumpButtonDown, input.JumpButtonReleased);
        ClimbLadder (input.VerticalAxisInput);
        UseWeapon   (input.Fire1Button);
        CheckWeaponInteractions (input.changeWeaponAction);
        ChooseSpriteFlipX();
        Die();
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody.velocity.y < -maxFallSpeed)
        {
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, -maxFallSpeed, 0);
        }
    }

    private void CheckWeaponInteractions(ChangeWeaponAction action)
    {
        // Check changeweapon action to see if we should change weapon
        switch (action)
        {
            case ChangeWeaponAction.Primary:
                m_WeaponSystem.EquipWeapon(m_PrimaryWeapon.gunProperties.weaponId);
                break;
            case ChangeWeaponAction.Secondary:
                m_WeaponSystem.EquipWeapon(m_SecondaryWeapon.gunProperties.weaponId);
                break;
            default: return;
        }
    }

    private void UseWeapon(bool fireButton)
    {
        if (fireButton) m_WeaponSystem.AttemptAttack();
    }

    public void Die()
    {
        if (m_Collider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            playerState = PlayerState.Dead;
            m_Animator.SetBool("Dead", true);
            m_Rigidbody.velocity = new Vector2(deathKick * -Mathf.Sign(m_Rigidbody.velocity.x), 8);
            m_Rigidbody.gravityScale = startingGravity * 0.75f;
            m_WeaponSystem.DropWeapon(new Vector3(Mathf.Sign(m_Rigidbody.transform.localScale.x) * 3, 4, 0));
        }
    }

    private void ClimbLadder(float verticalInput)
    {
        if (!m_Collider.IsTouchingLayers(LayerMask.GetMask("Climbable")))
        {
            m_Animator.SetBool("Climbing", false);
            m_Rigidbody.gravityScale = startingGravity;
            return;
        }

        m_Animator.SetBool("Climbing", true);
        m_Rigidbody.gravityScale = 0;

        Vector2 climbVelocity = new Vector2(m_Rigidbody.velocity.x, verticalInput * climbSpeed);
        m_Rigidbody.velocity  = climbVelocity;
    }

    private void Run(float horizontalInput)
    {
        Vector2 playerVelocity = new Vector2(horizontalInput * runSpeed, m_Rigidbody.velocity.y);
        m_Rigidbody.velocity   = playerVelocity;

        m_Animator.SetBool("Running", horizontalInput != 0);    // Set Animation
    }

    private void Jump(bool jumpPressed, bool JumpReleased)
    {
        bool grounded = m_Collider.IsTouchingLayers(standardBlockLayer);

        if (jumpPressed && grounded)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            m_Rigidbody.velocity += jumpVelocityToAdd;
        }
        else if (JumpReleased)
        {
            if (m_Rigidbody.velocity.y > 0) m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_Rigidbody.velocity.x * 0.4f, 0);
        }
    }

    private void ChooseSpriteFlipX()
    {
        int moveDirX = (Mathf.Abs(m_Rigidbody.velocity.x) > Mathf.Epsilon) ? (int) Mathf.Sign(m_Rigidbody.velocity.x) : 0;

        if (moveDirX != 0 && transform.localScale.x != moveDirX)
        {
            transform.localScale = new Vector3(moveDirX, 1, 0);
        }
    }

    #region (Not Used) Weapon Pickups
    //if (discardWeaponButtonDown)
    //{
    //    m_WeaponSystem.DropWeapon(new Vector3(Mathf.Sign(m_Rigidbody.transform.localScale.x) * 3, 2, 0));
    //}
    //if (weaponPickupCollision != null && pickupWeaponButtonDown)
    //{
    //    bool pickedUp = m_WeaponSystem.PickupWeapon(weaponPickupCollision.gameObject);
    //    Debug.Log("Picked Up Weapon?? : " + pickedUp);
    //}

    /**
    Collider2D weaponPickupCollision;   // Used for holding collided weapon pickup between frames
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon Pickup"))
        {
            weaponPickupCollision = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon Pickup"))
        {
            weaponPickupCollision = null;
        }
    }*/
    #endregion
}
public enum ChangeWeaponAction : byte { None, Primary, Secondary }
public enum PlayerState : byte { Dead, Alive, Paused }