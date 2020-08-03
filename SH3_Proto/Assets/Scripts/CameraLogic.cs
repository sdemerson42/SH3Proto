using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    GameObject m_target;

    public float pathSmoothing = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        m_target = GameObject.Find("Coifman");
    }

    // Update is called once per frame
    void Update()
    {
        var destination = m_target.transform.position;
        float z = transform.position.z;
        var newPosition = Vector3.Lerp(transform.position, destination, pathSmoothing);
        newPosition.z = z;
        transform.position = newPosition;
    }
}
