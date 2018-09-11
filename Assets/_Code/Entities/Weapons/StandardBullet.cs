using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBullet : Projectile
{
    readonly RaycastHit2D[] collisions = new RaycastHit2D[20];

    private new void Awake()
    {
        base.Awake();
    }

    private void FixedUpdate()
    {
        Vector3 singlePixelInDirectionVector = transform.right.normalized;
        Vector3 end = transform.position + singlePixelInDirectionVector * (m_Rigidbody.velocity.magnitude / 32);

        int hitCount = Physics2D.LinecastNonAlloc(transform.position, end, collisions);

        for (int i = 0; i < hitCount; i++)
        {
            if (collisions[i].collider.gameObject.layer == LayerMaskIndex.StandardBlock)
            {
                Destroy(gameObject); // Late destroy fixed spacing bug in collision
                break;
            }
        }
    }

    /// <summary>
    /// Destroy after 1 frame. this worked in getting rid of bullet and spacing after collision check
    /// </summary>
    IEnumerator LateDestroy()
    {
        yield return 0;
        Destroy(gameObject);
    }

    internal void SetGravity(float v)
    {
        m_Rigidbody.gravityScale = v;
    }

    internal void SetSpeed(float bulletSpeed)
    {
        m_Rigidbody.velocity = m_Rigidbody.transform.right * bulletSpeed;
    }
}
