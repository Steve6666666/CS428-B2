using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinkowskiTool
{
    // 生成闵可夫斯基差集
    public static List<Vector3> computeMinkowskiSet(Prism prismA, Prism prismB)
    {
        List<Vector3> vertices = new List<Vector3>();
        foreach (var v1 in prismA.GetVec3Points())
        {
            foreach (var v2 in prismB.GetVec3Points())
            {
                vertices.Add(v1 - v2);
            }
        }
        return ConvexHullGenerator.GenConvexHull(vertices.ToArray());
    }

}
