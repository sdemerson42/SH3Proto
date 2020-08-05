using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomData : MonoBehaviour
{
    [Serializable]
    public class HookPoint
    {
        public GameObject wallA;
        public GameObject wallB;
        public char dir;
    }

    public HookPoint[] hookPoints;
    public GameObject floor;

    public Bounds GetBounds()
    {
        Bounds bounds = new Bounds();
        Collider2D[] colliders = gameObject.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            bounds.Encapsulate(col.bounds);
        }
        return bounds;
    }
}
