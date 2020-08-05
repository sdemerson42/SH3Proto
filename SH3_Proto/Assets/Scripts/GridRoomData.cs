using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;
using UnityEngine.Tilemaps;

public class GridRoomData : MonoBehaviour
{
    [Serializable]
    public class HookPoint
    {
        public int position;
        public char direction;
    }

    public HookPoint[] hookPoints;
    public TileBase floor;
}
