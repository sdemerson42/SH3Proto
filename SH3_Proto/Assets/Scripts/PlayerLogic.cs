using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    public float walkSpeed = 5.0f;

    Animator m_animator;
    Rigidbody2D m_rigidBody;
    CapsuleCollider2D m_capsuleCollider;

    public GameObject m_dagger;

    int m_shootCooldown = 20;
    int m_shootCooldownCounter = 0;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
        Fire();
    }

    void MovePlayer()
    {
        float ix = Input.GetAxis("Horizontal");
        float iy = Input.GetAxis("Vertical");

        float magnitude = 0f;

        if (ix != 0f || iy != 0f)
        {
            magnitude = Mathf.Sqrt(ix * ix + iy * iy);
            ix /= magnitude;
            iy /= magnitude;
        }

        m_animator.SetFloat("WalkSpeed", magnitude);
        ix *= walkSpeed;
        iy *= walkSpeed;

        m_rigidBody.velocity = new Vector2(ix, iy);

    }

    void Fire()
    {
        if (m_shootCooldownCounter > 0)
        {
            m_shootCooldownCounter = (m_shootCooldownCounter + 1) % m_shootCooldown;
            return;
        }

        var x = Input.GetAxis("SHorizontal");
        var y = Input.GetAxis("SVertical");

        if (x == 0f && y == 0f) return;

        Vector2 velocity = new Vector2(x, y);
        velocity.Normalize();

        float angle = Mathf.Asin(velocity.y) * Mathf.Rad2Deg;
        if (velocity.x < 0f)
        {
            angle = 180f - angle;
        }

        m_shootCooldownCounter = 1;

        var dagger = Instantiate(m_dagger, transform.position,
            Quaternion.identity);
        Physics2D.IgnoreCollision(m_capsuleCollider,
            dagger.GetComponent<BoxCollider2D>());

        dagger.transform.Rotate(new Vector3(0f, 0f, angle));
        
        var daggerRb = dagger.GetComponent<Rigidbody2D>();

        daggerRb.velocity = velocity * 15f;

    }
}
