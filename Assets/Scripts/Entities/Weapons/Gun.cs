using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Gun : Weapon, IShootable
{
    protected enum WeaponState { Active, Reloading, NotOwned }

    [SerializeField] public GunData gunProperties;

    protected Rigidbody2D   m_Rigidbody;
    protected BoxCollider2D m_Collider;
    protected BoxCollider2D m_ChildTrigger;
    protected Transform     m_BulletOrigin;

    protected abstract void PerformShot();
    protected abstract void OnReloadBegin();
    protected abstract void OnReloadFinish();

    protected int   currentShotsInClip;
    protected float currentShotTimer;

    protected WeaponState currentState = WeaponState.NotOwned;

    void Awake()
    {
        currentShotsInClip = gunProperties.shotsPerClip;
        m_Rigidbody        = GetComponent<Rigidbody2D>();
        m_Collider         = GetComponent<BoxCollider2D>();
        m_ChildTrigger     = GetComponentInChildren<BoxCollider2D>(); // TODO POSSIBLE CHANGE THIS?
        m_BulletOrigin     = transform.Find("Bullet Origin");
    }

    private void Update()
    {
        if (currentShotTimer > 0) currentShotTimer -= Time.deltaTime;   // Reduce by 1 per second
    }

    public void DiscardAndThrow(Vector3 vel)
    {
        EnableCollisions();
        m_Rigidbody.velocity = vel;
        m_Rigidbody.AddTorque(-(vel.x / 2));
    }

    public void DisableCollisions()
    {
        m_Rigidbody.isKinematic     = true;
        m_Rigidbody.velocity        = Vector2.zero;
        m_Rigidbody.angularVelocity = 0;
        m_Collider.enabled          = false;
        m_ChildTrigger.enabled      = false;
    }

    public void EnableCollisions()
    {
        m_Rigidbody.isKinematic = false;
        m_Collider.enabled      = true;
        m_ChildTrigger.enabled  = true;
    }

    private IEnumerator Reload()
    {
            yield return new WaitForSeconds(gunProperties.reloadRate);

            Debug.Log("Shotgun Reload Finished");
            currentShotsInClip = gunProperties.shotsPerClip;
            currentState       = WeaponState.Active;
    }

    internal void PutAway()
    {
        gameObject.SetActive(false);
    }

    public override void Attack()
    {
        if (currentShotTimer > 0) return;

        if (currentShotsInClip > 0)
        {
            currentShotTimer = gunProperties.fireRate;
            currentShotsInClip -= 1;
            PerformShot();
        }
        else if (currentState != WeaponState.Reloading)
        {
            currentState = WeaponState.Reloading;
            OnReloadBegin();
            StartCoroutine(Reload());
        }
    }
}
