using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    public float walkSpeed = 5.0f;

    Animator m_animator;
    Rigidbody2D m_rigidBody;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
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
}
