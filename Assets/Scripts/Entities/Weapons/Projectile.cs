using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected Rigidbody2D m_Rigidbody;

    protected void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 vel)
    {
        m_Rigidbody.velocity = vel;
    }
}
