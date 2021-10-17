using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    public int pointCount = 3;
    public Vector3[] points;

    public float midY, height;

    public GameObject prismObject;
    public Vector2 getFarthestPointInDirection(Vector2 dir)
    {
        float maxDistance = float.MinValue;
        int maxIndex = 0;
        for (int i = 0; i < GetVec2Points().Count; ++i)
        {
            float distance = Vector2.Dot(GetVec2Points()[i], dir);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
            }
        }
        return GetVec2Points()[maxIndex];
    }

    public List<Vector2> GetVec2Points()
    {
        List<Vector2> vec2Points = new List<Vector2>();

        foreach(var point in points)
        {
            vec2Points.Add(new Vector2(point.x, point.z));
        }

        return vec2Points;
    }


}
