using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    public int pointCount = 3;
    public Vector3[] points;

    public float midY, height;

    public GameObject prismObject;
    public Vector3 getFarthestPointInDirection(Vector3 dir)
    {
        float maxDistance = float.MinValue;
        int maxIndex = 0;
        for (int i = 0; i < GetVec3Points().Count; ++i)
        {
            float distance = Vector3.Dot(GetVec3Points()[i], dir);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
            }
        }
        return GetVec3Points()[maxIndex];
    }

    public List<Vector3> GetVec3Points()
    {
        List<Vector3> vec3Points = new List<Vector3>();

        foreach(var point in points)
        {
            vec3Points.Add(new Vector3(point.x, 0, point.z));
        }

        return vec3Points;
    }


}
