using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldMapBuilder : MonoBehaviour
{

    public GameObject grid;

    // Tile settings

    public TileBase tileGrass;
    public TileBase tileWater;
    public TileBase tileShrubLight;
    public TileBase tileShrubDense;
    public TileBase tileDesert;
    public TileBase tileSwamp;
    public TileBase tileForestLight;
    public TileBase tileForestDense;
    public TileBase tileHills;
    public TileBase tileMountainLight;
    public TileBase tileMountainDense;
    public TileBase tileCactus;
    public TileBase tileShallowWater;

   List<Tilemap> m_gridLayers;

    enum GridLayerIndex
    {
        Floor = 0, Wall = 1, Obj = 2
    }

    private void Awake()
    {
        m_gridLayers = new List<Tilemap>();
        m_gridLayers.Add(grid.transform.
            GetChild((int)GridLayerIndex.Floor).gameObject.GetComponent<Tilemap>());
        m_gridLayers.Add(grid.transform.
             GetChild((int)GridLayerIndex.Wall).gameObject.GetComponent<Tilemap>());
        m_gridLayers.Add(grid.transform.
            GetChild((int)GridLayerIndex.Obj).gameObject.GetComponent<Tilemap>());
    }

    public void BuildMap()
    {
        PerlinNoiseGenerator png = new PerlinNoiseGenerator();
        const int width = 256;
        const int height = 256;

        // Lay down grass and shrub foundation

        var pMap = png.GeneratePerlinMap(width, height, width / 8, height / 8);
        for (int i = 0; i < pMap.Count; ++i)
        {
            for (int j = 0; j < pMap[i].Count; ++j)
            {
                TileBase tile;
                float val = pMap[i][j];
                if (val < .6f) tile = tileGrass;
                else if (val < .85f) tile = tileShrubLight;
                else tile = tileShrubDense;

                m_gridLayers[(int)GridLayerIndex.Floor].SetTile(
                    new Vector3Int(i, j, 0), tile);
            }
        }

        // Generate topo and moisture maps

        var tMap = png.GeneratePerlinMap(width, height, width / 8, height / 8);
        var mMap = png.GeneratePerlinMap(width, height, width / 8, height / 8);

        // Mountains

        for (int i = 0; i < tMap.Count; ++i)
        {
            for (int j = 0; j < tMap[i].Count; ++j)
            {
                float val = tMap[i][j];
                TileBase tile = null;
                int layerIndex = (int)GridLayerIndex.Floor;

                if (val > .6f) tile = tileHills;
                if (val > .7f) tile = tileMountainLight;
                if (val > .8f)
                {
                    tile = tileMountainDense;
                    layerIndex = (int)GridLayerIndex.Wall;
                }

                if (tile != null)
                {
                    m_gridLayers[layerIndex].SetTile(
                                        new Vector3Int(i, j, 0), tile);
                }
            }
        }

        // Roll a one-time topo map and create water (erodes mountains)

        var wMap = png.GeneratePerlinMap(width, height, width / 8, height / 8);
        for (int i = 0; i < tMap.Count; ++i)
        {
            for (int j = 0; j < tMap[i].Count; ++j)
            {
                float val = wMap[i][j];
                if (val < .25f)
                    m_gridLayers[(int)GridLayerIndex.Wall].SetTile(
                                        new Vector3Int(i, j, 0), tileWater);

            }
        }

        // Create forests based on topo and moisture. Can cover hills
        // but NOT mountains or water

        for (int i = 0; i < tMap.Count; ++i)
        {
            for (int j = 0; j < tMap[i].Count; ++j)
            {
                float mVal = mMap[i][j];
                TileBase tile = null;
                if (mVal > .5f) tile = tileForestLight;
                if (mVal > .65f) tile = tileForestDense;

                if (tile != null)
                {
                    // Check for no-go tiles on floor layers...
                    var floorTile = m_gridLayers[(int)GridLayerIndex.Floor].GetTile(
                        new Vector3Int(i, j, 0));
                    if (floorTile == tileMountainLight) continue;
                    m_gridLayers[(int)GridLayerIndex.Floor].SetTile(
                        new Vector3Int(i, j, 0), tile);
                }

            }
        }

        // Create swamps / deserts based on moisture map and topo map
        // Does not erode other features

        for (int i = 0; i < tMap.Count; ++i)
        {
            for (int j = 0; j < tMap[i].Count; ++j)
            {
                float moistureVal = mMap[i][j];
                float topoVal = tMap[i][j];
                TileBase tile = null;

                if (moistureVal > .4f && topoVal < .3f) tile = tileSwamp;
                if (moistureVal < .2f) tile = tileDesert;

                if (tile == null) continue;

                var floorTile = m_gridLayers[(int)GridLayerIndex.Floor].GetTile(
                    new Vector3Int(i, j, 0));
                var wallTile = m_gridLayers[(int)GridLayerIndex.Wall].GetTile(
                    new Vector3Int(i, j, 0));

                // No-go zones

                if (floorTile == tileForestDense || floorTile == tileMountainLight) 
                    continue;
               
                // Random decoration tiles

                if (Random.Range(0, 15) == 0)
                {
                    if (tile == tileSwamp) tile = tileShallowWater;
                    if (tile == tileDesert) tile = tileCactus;
                }

                m_gridLayers[(int)GridLayerIndex.Floor].SetTile(
                    new Vector3Int(i, j, 0), tile);
                
            }
        }
    }
}
