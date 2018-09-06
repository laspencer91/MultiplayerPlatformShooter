using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;

    Rigidbody2D       m_Rigidbody;
    BoxCollider2D     m_BoxCollider;
    CapsuleCollider2D m_CapsuleCollider;
    SpriteRenderer    m_Renderer;

    int moveDir = 1;

	void Start ()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Renderer  = GetComponent<SpriteRenderer>();
	}
	
	void Update ()
    {
        m_Rigidbody.velocity = new Vector2(moveSpeed * moveDir, 0f);
	}

    private void OnTriggerExit2D(Collider2D collision)
    {
        moveDir *= -1;
        transform.localScale = new Vector2(moveDir, 1f);
    }
}
