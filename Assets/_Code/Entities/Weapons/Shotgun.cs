using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    [SerializeField] float bulletSpeed = 20;

    protected override void OnReloadBegin()
    {
        Debug.Log("Reload shotgun has begun");
    }

    protected override void OnReloadFinish()
    {
        Debug.Log("Reload shotgun has finished");
    }

    private void OnEnable()
    {
        m_Animator.ResetTrigger("ShotFired");
    }

    protected override void PerformShot()
    {
        Random.InitState(255);
        for (int i = 0; i < 4; i++)
        {
            StandardBullet bullet = (StandardBullet) Instantiate(gunProperties.projectile);
            Vector3 randomVector = Random.insideUnitCircle.normalized;
            bullet.transform.position = (m_BulletOrigin != null) ? m_BulletOrigin.position : transform.position;
            
            float dirX      = transform.lossyScale.x;
            float shotAngle = Mathf.Acos(dirX) * Mathf.Rad2Deg;

            bullet.transform.Rotate(Vector3.forward, Random.Range(shotAngle - 6, shotAngle + 10));
            bullet.SetSpeed(bulletSpeed * Random.Range(0.8f, 1.2f));
            bullet.SetGravity(Random.Range(0.4f, 0.5f));
        }

        m_Animator.SetTrigger("ShotFired");
        AudioManager.Play("Shotgun Shot");
    }
}
