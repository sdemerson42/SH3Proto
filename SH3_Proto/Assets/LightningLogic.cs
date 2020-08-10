using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightningLogic : MonoBehaviour
{

    Light2D m_light;
    int m_flashCounter = 0;
    int m_flashTime = 0;

    // Start is called before the first frame update
    void Awake()
    {
        m_light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_flashCounter > 0)
        {
            Flash();
            return;
        }

        if (Random.Range(0, 300) == 0)
        {
            m_flashCounter = 1;
            m_flashTime = Random.Range(3, 15);
            m_light.enabled = true;
        }
    }

    void Flash()
    {
        if (++m_flashCounter == m_flashTime)
        {
            m_flashCounter = 0;
            m_light.enabled = false;
        }

        float intensity = Random.Range(0.5f, 0.65f);
        m_light.intensity = intensity;
    }
}
