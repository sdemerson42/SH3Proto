using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Flicker : MonoBehaviour
{
    public float intensityVariance = .5f;
    public int changeFrames = 5; 

    float m_startingIntensity;
    Light2D m_light;
    int m_changeCounter = 0;

    // Start is called before the first frame update
    void Awake()
    {
        m_light = GetComponent<Light2D>();
        m_startingIntensity = m_light.intensity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_changeCounter = ++m_changeCounter % changeFrames;

        if (m_changeCounter == 0)
        {
            float intensity = Random.Range(m_startingIntensity - intensityVariance,
                m_startingIntensity + intensityVariance);
            m_light.intensity = intensity;
        }
    }
}
