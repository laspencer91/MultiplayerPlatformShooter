using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    [SerializeField] float bulletSpeed = 20;

    protected override void OnReloadBegin()
    {
        Debug.Log("Reload pistol has begun");
    }

    protected override void OnReloadFinish()
    {
        Debug.Log("Reload pistol has finished");
    }

    protected override void PerformShot()
    {
        StandardBullet bullet = (StandardBullet) Instantiate(gunProperties.projectile);
        bullet.transform.position = (m_BulletOrigin != null) ? m_BulletOrigin.position : transform.position;
            
        float dirX      = transform.lossyScale.x;
        float shotAngle = Mathf.Acos(dirX) * Mathf.Rad2Deg;

        bullet.transform.Rotate(Vector3.forward, shotAngle);
        bullet.SetSpeed(bulletSpeed);

        m_Animator.SetBool("ShotFired", true);
        m_Animator.speed = 2;

        AudioManager.Play("Pistol Shot", 0.1f);
    }
}
