using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonLogic : MonoBehaviour
{

    Rigidbody2D m_rigidBody;

    int m_moveCounter = 0;
    int m_moveCoolDown = 0;
    char m_dir = '-';
    Vector2 m_velocity = new Vector2();
    float m_speed = 2.0f;

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        SelectNewDirection();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MakeMove();
    }

    private void MakeMove()
    {
        m_moveCounter = (m_moveCounter + 1) % m_moveCoolDown;
        if (m_moveCounter == 0) SelectNewDirection();

        m_rigidBody.velocity = m_velocity;
    }

    void SelectNewDirection()
    {
        if (m_dir == 'n' || m_dir == 's')
            m_dir = Random.Range(0, 2) == 0 ? 'w' : 'e';
        else if (m_dir == 'w' || m_dir == 'e')
            m_dir = Random.Range(0, 2) == 0 ? 'n' : 's';
        else
        {
            switch(Random.Range(0, 4))
            {
                case 0:
                    m_dir = 'n';
                    break;
                case 1:
                    m_dir = 's';
                    break;
                case 2:
                    m_dir = 'w';
                    break;
                case 3:
                    m_dir = 'e';
                    break;
            }
        }

        m_velocity.x = 0f;
        m_velocity.y = 0f;

        switch(m_dir)
        {
            case 'n':
                m_velocity.y = m_speed;
                break;
            case 's':
                m_velocity.y = -1f * m_speed;
                break;
            case 'w':
                m_velocity.x = -1f * m_speed;
                break;
            case 'e':
                m_velocity.x = m_speed;
                break;
        }

        m_moveCounter = 0;
        m_moveCoolDown = Random.Range(20, 51);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerProjectile")
        {
            Destroy(gameObject);
            return;
        }

        SelectNewDirection();
    }
}
