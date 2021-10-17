using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinkowskiTool
{
    // 生成闵可夫斯基差集
    public static List<Vector2> computeMinkowskiSet(Prism prismA, Prism prismB)
    {
        List<Vector2> vertices = new List<Vector2>();
        foreach (var v1 in prismA.GetVec2Points())
        {
            foreach (var v2 in prismB.GetVec2Points())
            {
                vertices.Add(v1 - v2);
            }
        }
        return ConvexHullGenerator.GenConvexHull(vertices.ToArray());
    }

}
