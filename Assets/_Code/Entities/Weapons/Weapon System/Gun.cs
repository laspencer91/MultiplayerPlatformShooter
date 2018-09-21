using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Gun : Weapon, IShootable
{
    protected enum WeaponState { Active, Reloading, NotOwned }

    [SerializeField] public GunData gunProperties;

    protected WeaponState currentState = WeaponState.NotOwned;

    // Save the needed components. Not all of these have to be used.
    protected Rigidbody2D    m_Rigidbody;
    protected BoxCollider2D  m_Collider;
    protected BoxCollider2D  m_ChildTrigger;
    protected Transform      m_BulletOrigin;
    protected Animator       m_Animator;

    // These must be implemented, but it's up to the child on what they do.
    protected abstract void PerformShot();
    protected abstract void OnReloadBegin();
    protected abstract void OnReloadFinish();

    private int   currentShotsInClip;
    private float currentShotTimer;

    void Awake()
    {
        currentShotsInClip = gunProperties.shotsPerClip;
        m_Rigidbody        = GetComponent<Rigidbody2D>();
        m_Collider         = GetComponent<BoxCollider2D>();
        m_ChildTrigger     = GetComponentInChildren<BoxCollider2D>(); // TODO POSSIBLE CHANGE THIS?
        m_BulletOrigin     = transform.Find("Bullet Origin");
        m_Animator         = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentShotTimer > 0) currentShotTimer -= Time.deltaTime;   // Reduce by 1 per second
    }

    /// <summary>
    /// Discard weapon and set its rigidbodies' velocity
    /// </summary>
    /// <param name="vel">Velocity to set rigidbody to</param>
    public void DiscardAndThrow(Vector3 vel)
    {
        EnableCollisions();
        m_Rigidbody.velocity = vel;
        m_Rigidbody.AddTorque(-(vel.x / 2));
    }

    // Disables Collisions, at the same time we make sure velocity is reset
    // So that if this was disabled while having a high velocity it will not continue
    // flying upon re-enabling.
    public void DisableCollisions()
    {
        m_Rigidbody.isKinematic     = true;
        m_Rigidbody.velocity        = Vector2.zero;
        m_Rigidbody.angularVelocity = 0;
        m_Collider.enabled          = false;
        m_ChildTrigger.enabled      = false;
    }

    // Enable collisions, this generally should happen upon this gun getting reactivated.
    public void EnableCollisions()
    {
        m_Rigidbody.isKinematic = false;
        m_Collider.enabled      = true;
        m_ChildTrigger.enabled  = true;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(gunProperties.reloadRate);

        Debug.Log("Reload Finished");
        currentShotsInClip = gunProperties.shotsPerClip;
        currentState       = WeaponState.Active;
    }

    /// <summary>
    /// Action to take when the weapon is told to put away, set to inactive
    /// </summary>
    internal void PutAway()
    {
        DisableCollisions();
        currentState = WeaponState.Active; // Take it off of reloading
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Attempt to shoot, but this will take into acount fire rate and reloading. Won't
    /// shoot if it's not supposed to
    /// </summary>
    public override bool Attack()
    {
        if (currentShotTimer > 0) return false;

        if (currentShotsInClip > 0)
        {
            currentShotTimer = gunProperties.fireRate;
            currentShotsInClip -= 1;
            PerformShot();
            return true;
        }
        else if (currentState != WeaponState.Reloading)
        {
            currentState = WeaponState.Reloading;
            OnReloadBegin();
            StartCoroutine(Reload());
        }

        return false;
    }
}
