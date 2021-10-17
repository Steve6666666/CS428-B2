using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GJKTool
{
    public static float sqrDistance(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return dx * dx + dz * dz;
    }

    /// 判读点c在ab的哪一侧
    public static int whitchSide(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        float cross = ab.x * ac.z - ab.z * ac.x;
        return cross > 0 ? 1 : (cross < 0 ? -1 : 0);
    }

    /// 获得原点到线段ab的最近点。最近点可以是垂点，也可以是线段的端点。
    public static Vector3 getClosestPointToOrigin(Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        Vector3 ao = Vector3.zero - a;

        float sqrLength = ab.sqrMagnitude;

        // ab点重合了
        if (sqrLength < float.Epsilon)
        {
            return a;
        }

        float projection = Vector3.Dot(ab, ao) / sqrLength;
        if (projection < 0)
        {
            return a;
        }
        else if (projection > 1.0f)
        {
            return b;
        }
        else
        {
            return a + ab * projection;
        }
    }

    /// 获得原点到直线ab的垂点c。(c - o)就是原点到ab的垂线
    public static Vector3 getPerpendicularToOrigin(Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        Vector3 ao = Vector3.zero - a;

        float sqrLength = ab.sqrMagnitude;
        if (sqrLength < float.Epsilon)
        {
            return Vector3.zero;
        }

        float projection = Vector3.Dot(ab, ao) / sqrLength;
        return a + ab * projection;
    }

    public static bool contains(List<Vector3> points, Vector3 point)
    {
        int n = points.Count;
        if (n < 3)
        {
            return false;
        }

        // 先计算出内部的方向
        int innerSide = whitchSide(points[0], points[1], points[2]);

        // 通过判断点是否均在三条边的内侧，来判定单形体是否包含点
        for (int i = 0; i < n; ++i)
        {
            int iNext = (i + 1) % n;
            int side = whitchSide(points[i], points[iNext], point);

            if (side == 0) // 在边界上
            {
                return true;
            }

            if (side != innerSide) // 在外部
            {
                return false;
            }
        }

        return true;
    }
}
