using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvexHullGenerator
{
    public static float epsilon = 0.00001f;

    public static List<Vector3> GenConvexHull(Vector3[] vertices)
    {
        int count = vertices.Length;
        if (count < 3)
        {
            return null;
        }

        // 找到最低点，作为原点
        int bottomIndex = 0;
        for (int i = 1; i < count; ++i)
        {
            if (vertices[i].z < vertices[bottomIndex].z)
            {
                bottomIndex = i;
            }
        }

        // 把最低点移出数组
        Vector3 origin = vertices[bottomIndex];
        --count;
        if (bottomIndex != count)
        {
            vertices[bottomIndex] = vertices[count];
        }

        // 将任意点与原点相连，并计算斜率的角度
        float[] angles = new float[count];
        for (int i = 0; i < count; ++i)
        {
            Vector3 delta = vertices[i] - origin;
            angles[i] = Mathf.Atan2(delta.z, delta.x);
        }

        // 将角度从小到大排序
        System.Array.Sort(angles, vertices, 0, count);

        // 扫描算法从最小斜率的点开始。也就是从最右侧的点开始，依次向做扫描。

        List<Vector3> result = new List<Vector3>(vertices.Length);
        result.Add(origin);
        result.Add(vertices[0]);
        for (int i = 1; i < count;)
        {
            Vector3 p0 = result[result.Count - 2];
            Vector3 p1 = result[result.Count - 1];
            Vector3 p2 = vertices[i];

            Vector3 a = p1 - p0;
            Vector3 b = p2 - p0;

            float cross = a.x * b.z - a.z * b.x;
            if (cross < -epsilon)
            {
                // 上一个点(p1)不是凸点
                result.RemoveAt(result.Count - 1);
                if (i + 1 == count)
                {
                    result.Add(p2);
                    ++i;
                }
            }
            else if (cross > epsilon)
            {
                // 可能是凸点
                ++i;
                result.Add(p2);
            }
            else
            {
                // 在同一条线上，取最远的
                if (b.sqrMagnitude > a.sqrMagnitude)
                {
                    result.RemoveAt(result.Count - 1);
                    if (result.Count == 1 || i + 1 == count)
                    {
                        result.Add(p2);
                        ++i;
                    }
                }
                else
                {
                    ++i;
                }
            }
        }

        return result;
    }
}

