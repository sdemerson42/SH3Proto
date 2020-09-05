using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PerlinNoiseGenerator
{

    public List<List<float>> GeneratePerlinMap(int width, int height, int cellWidth, int cellHeight)
    {
        // Generate random vectors

        Vector2Int gridSize = new Vector2Int(width / cellWidth + 1,
            height / cellHeight + 1);

        var randomVectors = GenerateRandomVectors(gridSize);

        // Generate raw values

        Vector2 rawConstraints = new Vector2(float.MaxValue, float.MinValue);

        List<List<float>> rMap = new List<List<float>>();
        for (int i = 0; i < width; ++i)
        {
            rMap.Add(new List<float>());
            for (int j = 0; j < height; ++j)
            {
                rMap[i].Add(0f);
            }
        }
        
        for (int j = 0; j < height; ++j)
        {
            for (int i = 0; i < width; ++i)
            {
                // Calculate corner vectors
                Vector2Int rvAnchorIndex = new Vector2Int(
                    i / cellWidth, j / cellHeight);
                Vector2Int mapAnchorIndex = new Vector2Int(
                    rvAnchorIndex.x * cellWidth, rvAnchorIndex.y * cellHeight);

                Vector2 tlVec = new Vector2(
                    (float)(i - mapAnchorIndex.x),
                    (float)(j - mapAnchorIndex.y));
                Vector2 trVec = new Vector2(
                    (float)(i - (mapAnchorIndex.x + cellWidth)),
                    (float)(j - mapAnchorIndex.y));
                Vector2 blVec = new Vector2(
                    (float)(i - mapAnchorIndex.x),
                    (float)(j - (mapAnchorIndex.y + cellHeight)));
                Vector2 brVec = new Vector2(
                    (float)(i - (mapAnchorIndex.x + cellWidth)),
                    (float)(j - (mapAnchorIndex.y + cellHeight)));

                // Find dot products

                var rv = randomVectors[rvAnchorIndex.x][rvAnchorIndex.y];
                float r = tlVec.x * rv.x + tlVec.y * rv.y;

                rv = randomVectors[rvAnchorIndex.x + 1][rvAnchorIndex.y];
                float s = trVec.x * rv.x + trVec.y * rv.y;

                rv = randomVectors[rvAnchorIndex.x][rvAnchorIndex.y + 1];
                float t = blVec.x * rv.x + blVec.y * rv.y;

                rv = randomVectors[rvAnchorIndex.x + 1][rvAnchorIndex.y + 1];
                float u = brVec.x * rv.x + brVec.y * rv.y;

                // Find lerp t values

                float tx = Smooth((float)(i % cellWidth) / (float)(cellWidth - 1));
                float ty = Smooth((float)(j % cellHeight) / (float)(cellHeight - 1));

                // Calculate raw value

                float a = Lerp(r, s, tx);
                float b = Lerp(t, u, tx);
                float c = Lerp(a, b, ty);
                rMap[i][j] = c;

                // Modify constraints if necessary

                if (c < rawConstraints.x) rawConstraints.x = c;
                if (c > rawConstraints.y) rawConstraints.y = c;
            }
        }

        NormalizePerlinMap(rMap, rawConstraints);
        return rMap;
    }

    List<List<Vector2>> GenerateRandomVectors(Vector2 size)
    {
        List<List<Vector2>> r = new List<List<Vector2>>();

        for (int i = 0; i < size.x; ++i)
        {
            r.Add(new List<Vector2>());
            for (int j = 0; j < size.y; ++j)
            {
                int x = 0;
                int y = 0;
                while(x == 0 && y == 0)
                {
                    x = Random.Range(-1, 2);
                    y = Random.Range(-1, 2);
                }
                r[i].Add(new Vector2((float)x, (float)y));
            }
        }
        return r;
    }

    float Lerp(float a, float b, float t)
    {
        return b * t + (1f - t) * a;
    }

    float Smooth(float x)
    {
        return 3f * x * x - 2f * x * x * x;
    }

    void NormalizePerlinMap(List<List<float>> map, Vector2 constraints)
    {
        float range = constraints.y - constraints.x;
        float adjust = 0f - constraints.x;

        for (int i = 0; i < map.Count; ++i)
        {
            for (int j = 0; j < map[i].Count; ++j)
            {
                float raw = map[i][j];
                raw += adjust;
                map[i][j] = raw / range;
            }
        }
    }
}
