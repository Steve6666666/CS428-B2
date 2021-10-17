
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class PrismManager : MonoBehaviour
{
    public int prismCount = 10;
    public float prismRegionRadiusXZ = 5;
    public float prismRegionRadiusY = 5;
    public float maxPrismScaleXZ = 5;
    public float maxPrismScaleY = 5;
    public GameObject regularPrismPrefab;
    public GameObject irregularPrismPrefab;

    private List<Prism> prisms = new List<Prism>();
    private List<GameObject> prismObjects = new List<GameObject>();
    private GameObject prismParent;
    private Dictionary<Prism, bool> prismColliding = new Dictionary<Prism, bool>();

    private const float UPDATE_RATE = 0.5f;


    List<Shapes> shapes;


    #region Unity Functions

    void Start()
    {
        Random.InitState(0);    //10 for no collision
        shapes = new List<Shapes>();

        prismParent = GameObject.Find("Prisms");
        for (int i = 0; i < prismCount; i++)
        {
            var randPointCount = Mathf.RoundToInt(3 + Random.value * 7);
            var randYRot = Random.value * 360;
            var randScale = new Vector3((Random.value - 0.5f) * 2 * maxPrismScaleXZ, (Random.value - 0.5f) * 2 * maxPrismScaleY, (Random.value - 0.5f) * 2 * maxPrismScaleXZ);
            var randPos = new Vector3((Random.value - 0.5f) * 2 * prismRegionRadiusXZ, (Random.value - 0.5f) * 2 * prismRegionRadiusY, (Random.value - 0.5f) * 2 * prismRegionRadiusXZ);
  
            GameObject prism = null;
            Prism prismScript = null;
            if (Random.value < 0.5f)
            {
                prism = Instantiate(regularPrismPrefab, randPos, Quaternion.Euler(0, randYRot, 0));
                prismScript = prism.GetComponent<RegularPrism>();
            }
            else
            {
                prism = Instantiate(irregularPrismPrefab, randPos, Quaternion.Euler(0, randYRot, 0));
                prismScript = prism.GetComponent<IrregularPrism>();
            }
            prism.name = "Prism " + i;
            prism.transform.localScale = randScale;
            prism.transform.parent = prismParent.transform;
            prismScript.pointCount = randPointCount;
            prismScript.prismObject = prism;

            prisms.Add(prismScript);
            prismObjects.Add(prism);
            prismColliding.Add(prismScript, false);
        }

        StartCoroutine(Run());
    }

    void Update()
    {
        #region Visualization

        DrawPrismRegion();
        DrawPrismWireFrames();

#if UNITY_EDITOR
        if (Application.isFocused)
        {
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
#endif

        #endregion
    }

    public class ShapesComparer : IComparer<Shapes>
    {

        public enum sortBy
        {
            left_x,
            left_y,
            left_z
        }

        //Sort two employee Ages  
        public sortBy compareByFields = sortBy.left_x;

        public int Compare(Shapes x, Shapes y)
        {
            switch (compareByFields)
            {
                case sortBy.left_x:
                    return x.getLeftx().CompareTo(y.getLeftx());
                case sortBy.left_y:
                    return x.getLefty().CompareTo(y.getLefty());
                case sortBy.left_z:
                    return x.getLeftz().CompareTo(y.getLeftz());
                default: break;

            }
            return x.getLeftx().CompareTo(y.getLeftx());
        }
    }

    public class Shapes
    {
        int id;
        float left_x;
        float right_x;
        float left_y;
        float right_y;
        float left_z;
        float right_z;

        public Shapes(int id, float left_x, float right_x, float left_y, float right_y, float left_z, float right_z)
        {
            this.id = id;
            this.left_x = left_x - 1.0f;
            this.right_x = right_x + 1.0f;
            this.left_y = left_y - 1.0f;
            this.right_y = right_y + 1.0f;
            this.left_z = left_z - 1.0f;
            this.right_z = right_z + 1.0f;
        }

        public int getId()
        {
            return id;
        }

        public float getLeftx()
        {
            return left_x;
        }
        public float getRightx()
        {
            return right_x;
        }
        public float getLefty()
        {
            return left_y;
        }
        public float getRighty()
        {
            return right_y;
        }
        public float getLeftz()
        {
            return left_z;
        }
        public float getRightz()
        {
            return right_z;
        }
        public void printout()
        {
            print("id=" + id + ",left_x=" + left_x + ",right_x=" + right_x + ",left_z=" + left_z + "right_z=" + right_z);
        }
        public Shapes findById(int id)
        {
            if (this.id == id)
            {
                return this;
            }
            return null;
        }
    }

    IEnumerator Run()
    {
        yield return null;

        while (true)
        {
            foreach (var prism in prisms)
            {
                prismColliding[prism] = false;
            }

            foreach (var collision in PotentialCollisions())
            {
                if (CheckCollision(collision))
                {
                    //print(collision.a.name + " and " + collision.b.name);
                    prismColliding[collision.a] = true;
                    prismColliding[collision.b] = true;

                    ResolveCollision(collision);
                }
            }

            yield return new WaitForSeconds(UPDATE_RATE);
        }
    }
    #endregion

    #region Incomplete Functions
    private Rectangle change(Prism p)
    {
        float maxX=float.MinValue;
        float minX=float.MaxValue;
        float maxY= float.MinValue;
        float minY= float.MinValue;
        for(int i = 0; i < p.points.Count(); i++)
        {
            if (p.points[i].x > maxX)
            {
                maxX = p.points[i].x;
            }else if(p.points[i].x< minX)
            {
                minX = p.points[i].x;
            }else if (p.points[i].y > maxY) 
            {
                maxY = p.points[i].y;
            }else if(p.points[i].y< minY)
            {
                minY = p.points[i].y;
            }
        }

        return new Rectangle(1,2,2,3);
    }

    private IEnumerable<PrismCollision> PotentialCollisions()
    {
        Quadtree quad = new Quadtree(0, new Rectangle(-10, -10, 20,20));
        quad.clear();
        for (int i = 0; i < prisms.Count; i++)
        {
            Rectangle temp = change(prisms[i]);
            quad.insert(temp);
        }

        for (int i = 0; i < prisms.Count; i++)
        {
            for (int j = i + 1; j < prisms.Count; j++)
            {
                var checkPrisms = new PrismCollision();
                checkPrisms.a = prisms[i];
                checkPrisms.b = prisms[j];

                yield return checkPrisms;
            }
        }
     

        /*shapes.Clear();
        foreach (var prism in prisms)
        {
            float left_x = float.MaxValue; float right_x = float.MinValue;
            float left_z = float.MaxValue; float right_z = float.MinValue;
            float left_y = float.MaxValue; float right_y = float.MinValue;
            foreach (Vector3 point in prism.points)
            {
                if (point.x < left_x)
                    left_x = point.x;
                else if (point.x > right_x)
                    right_x = point.x;
                if (point.z < left_z)
                    left_z = point.z;
                else if (point.z > right_z)
                    right_z = point.z;
                if (point.y < left_y)
                    left_y = point.y;
                else if (point.y > right_y)
                    left_y = point.y;
            }
            Shapes a = new Shapes(Int32.Parse(prism.name.Substring(6)), left_x, right_x, left_y,right_y, left_z, right_z);
            shapes.Add(a);
            //a.printout();
        }

        //sort by x
        List<float> activeList = new List<float>();
        List<float[]> collisionPairsX = new List<float[]>();
        List<float[]> collisionPairs = new List<float[]>();
        List<float[]> sortedX = new List<float[]>();
        activeList.Clear();

        foreach (Shapes shape in shapes)
        {
            sortedX.Add(new float[] { shape.getId(), shape.getLeftx() });
            sortedX.Add(new float[] { shape.getId(), shape.getRightx() });
        }
        sortedX.Sort(delegate (float[] x, float[] y)
        {
            return x[1].CompareTo(y[1]);
        });

        foreach (float[] pair in sortedX)
        {
            if (activeList.Contains(pair[0]))
            {
                activeList.Remove(pair[0]);
                //print("removed" + pair[0]);
            }
            else
            {

                foreach (int i in activeList)
                {

                    collisionPairsX.Add(new float[] { i, pair[0] });
                }
                activeList.Add(pair[0]);
                //print("added"+pair[0]);
            }
        }
        //collisionPairsX.ForEach((float[] a) => print("x-axis" + a[0] + " and " + a[1]));

        //sort by z
        List<float[]> sortedZ = new List<float[]>();
        List<float[]> collisionPairsZ = new List<float[]>();
        List<float> activeListZ = new List<float>();
        foreach (Shapes shape in shapes)
        {
            sortedZ.Add(new float[] { shape.getId(), shape.getLeftz() });
            sortedZ.Add(new float[] { shape.getId(), shape.getRightz() });
        }
        sortedZ.Sort(delegate (float[] x, float[] y)
        {
            return x[1].CompareTo(y[1]);
        });
       // print("sortedZ size:" + sortedZ.Count);

        foreach (float[] pair in sortedZ)
        {
            if (activeListZ.Contains(pair[0]))
            {
                activeListZ.Remove(pair[0]);
            }
            else
            {
                foreach (int i in activeListZ)
                {
                    collisionPairsZ.Add(new float[] { i, pair[0] });
                }
                activeListZ.Add(pair[0]);
            }
        }
        //collisionPairsZ.ForEach((float[] a) => print("z-axis" + a[0] + " and " + a[1]));

        //sort by y
        List<float[]> sortedY = new List<float[]>();
        List<float[]> collisionPairsY = new List<float[]>();
        List<float> activeListY = new List<float>();
        activeList.Clear();
        foreach (Shapes shape in shapes)
        {
            sortedY.Add(new float[] { shape.getId(), shape.getLefty() });
            sortedY.Add(new float[] { shape.getId(), shape.getRighty() });
        }
        sortedY.Sort(delegate (float[] x, float[] y)
        {
            return x[1].CompareTo(y[1]);
        });
        // print("sortedZ size:" + sortedZ.Count);

        foreach (float[] pair in sortedY)
        {
            if (activeListY.Contains(pair[0]))
            {
                activeListY.Remove(pair[0]);
            }
            else
            {
                foreach (int i in activeListY)
                {
                    collisionPairsY.Add(new float[] { i, pair[0] });
                }
                activeListY.Add(pair[0]);
            }
        }
        foreach (float[] i in collisionPairsX)
        {
            if ((collisionPairsZ.Any(a => a.SequenceEqual(i)) || collisionPairsZ.Any(a => a.SequenceEqual(new float[] { i[1], i[0] })))&&(collisionPairsY.Any(a => a.SequenceEqual(i)) || collisionPairsY.Any(a => a.SequenceEqual(new float[] { i[1], i[0] }))))
            {
                //print("true collision!");

                var checkPrisms = new PrismCollision();
                checkPrisms.a = prisms[(int)i[0]];
                checkPrisms.b = prisms[(int)i[1]];
                yield return checkPrisms;
            }

        }
        yield break;*/
    }

    private bool CheckCollision(PrismCollision collision)
    {
        var prismA = collision.a;
        var prismB = collision.b;

        GJK gjk = new GJK();

        if(!gjk.queryCollision(prismA, prismB))
            return false;

        //collision.penetrationDepthVectorAB = gjk.penetrationVector;
        collision.penetrationDepthVectorAB.x = gjk.penetrationVector.x+0.2f;
        collision.penetrationDepthVectorAB.y = 0.0f;
        collision.penetrationDepthVectorAB.z = gjk.penetrationVector.y+0.2f;
        //collision.penetrationDepthVectorAB = Vector3.zero;

        return true;
    }
    #endregion

    #region Private Functions

    private void ResolveCollision(PrismCollision collision)
    {
        var prismObjA = collision.a.prismObject;
        var prismObjB = collision.b.prismObject;

        var pushA = -collision.penetrationDepthVectorAB / 2;
        var pushB = collision.penetrationDepthVectorAB / 2;

        for (int i = 0; i < collision.a.pointCount; i++)
        {
            collision.a.points[i] += pushA;
        }
        for (int i = 0; i < collision.b.pointCount; i++)
        {
            collision.b.points[i] += pushB;
        }
        //prismObjA.transform.position += pushA;
        //prismObjB.transform.position += pushB;

        Debug.DrawLine(prismObjA.transform.position, prismObjA.transform.position + collision.penetrationDepthVectorAB, Color.cyan, UPDATE_RATE);
    }

    #endregion

    #region Visualization Functions

    private void DrawPrismRegion()
    {
        var points = new Vector3[] { new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 1) }.Select(p => p * prismRegionRadiusXZ).ToArray();

        var yMin = -prismRegionRadiusY;
        var yMax = prismRegionRadiusY;

        var wireFrameColor = Color.yellow;

        foreach (var point in points)
        {
            Debug.DrawLine(point + Vector3.up * yMin, point + Vector3.up * yMax, wireFrameColor);
        }

        for (int i = 0; i < points.Length; i++)
        {
            Debug.DrawLine(points[i] + Vector3.up * yMin, points[(i + 1) % points.Length] + Vector3.up * yMin, wireFrameColor);
            Debug.DrawLine(points[i] + Vector3.up * yMax, points[(i + 1) % points.Length] + Vector3.up * yMax, wireFrameColor);
        }
    }

    private void DrawPrismWireFrames()
    {
        for (int prismIndex = 0; prismIndex < prisms.Count; prismIndex++)
        {
            var prism = prisms[prismIndex];
            var prismTransform = prismObjects[prismIndex].transform;

            var yMin = prism.midY - prism.height / 2 * prismTransform.localScale.y;
            var yMax = prism.midY + prism.height / 2 * prismTransform.localScale.y;

            var wireFrameColor = prismColliding[prisms[prismIndex]] ? Color.red : Color.green;

            foreach (var point in prism.points)
            {
                Debug.DrawLine(point + Vector3.up * yMin, point + Vector3.up * yMax, wireFrameColor);
            }

            for (int i = 0; i < prism.pointCount; i++)
            {
                Debug.DrawLine(prism.points[i] + Vector3.up * yMin, prism.points[(i + 1) % prism.pointCount] + Vector3.up * yMin, wireFrameColor);
                Debug.DrawLine(prism.points[i] + Vector3.up * yMax, prism.points[(i + 1) % prism.pointCount] + Vector3.up * yMax, wireFrameColor);
            }
        }
    }

    #endregion

    #region Utility Classes

    private class PrismCollision
    {
        public Prism a;
        public Prism b;
        public Vector3 penetrationDepthVectorAB;
    }

    private class Tuple<K, V>
    {
        public K Item1;
        public V Item2;

        public Tuple(K k, V v)
        {
            Item1 = k;
            Item2 = v;
        }
    }

    #endregion
    private class Quadtree
    {

        private int MAX_OBJECTS = 10;
        private int MAX_LEVELS = 5;

        private int level;
        private List<Rectangle> objects;
        private Rectangle bounds;
        private Quadtree[] nodes;

        /*
         * Constructor
         */
        public Quadtree(int pLevel, Rectangle pBounds)
        {
            level = pLevel;
            objects = new List<Rectangle>();
            bounds = pBounds;
            nodes = new Quadtree[4];
        }
        public void clear()
        {
            objects.Clear();

            for (int i = 0; i < nodes.Count(); i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].clear();
                    nodes[i] = null;
                }
            }
        }
        private void split()
        {
            int subWidth = (int)(bounds.getWidth() / 2);
            int subHeight = (int)(bounds.getHeight() / 2);
            int x = (int)bounds.getX();
            int y = (int)bounds.getY();

            nodes[0] = new Quadtree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new Quadtree(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new Quadtree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new Quadtree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }
        
        private int getIndex(Rectangle pRect)
        {
            int index = -1;
            double verticalMidpoint = bounds.getX() + (bounds.getWidth() / 2);
            double horizontalMidpoint = bounds.getY() + (bounds.getHeight() / 2);

            // Object can completely fit within the top quadrants
            bool topQuadrant = (pRect.getY() < horizontalMidpoint && pRect.getY() + pRect.getHeight() < horizontalMidpoint);
            // Object can completely fit within the bottom quadrants
            bool bottomQuadrant = (pRect.getY() > horizontalMidpoint);

            // Object can completely fit within the left quadrants
            if (pRect.getX() < verticalMidpoint && pRect.getX() + pRect.getWidth() < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            // Object can completely fit within the right quadrants
            else if (pRect.getX() > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 0;
                }
                else if (bottomQuadrant)
                {
                    index = 3;
                }
            }

            return index;
        }
        
        
        public void insert(Rectangle pRect)
        {
            if (nodes[0] != null)
            {
                int index = getIndex(pRect);

                if (index != -1)
                {
                    nodes[index].insert(pRect);

                    return;
                }
            }

            objects.Add(pRect);

            if (objects.Count() > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    split();
                }

                int i = 0;
                while (i < objects.Count())
                {
                    int index = getIndex(objects[i]);
                    if (index != -1)
                    {
                        //可能有问题
                        Rectangle ra = nodes[index].objects[i];
                        nodes[index].objects.RemoveAt(i);
                        nodes[index].insert(ra);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }


        public List<Rectangle> retrieve(List<Rectangle> returnObjects, Rectangle pRect)
        {
            int index = getIndex(pRect);
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].retrieve(returnObjects, pRect);
            }

            //returnObjects.addAll(objects);
            //可能有问题
            for(int i = 0; i < objects.Count; i++)
            {
                returnObjects.Add(objects[i]);
            }

            return returnObjects;
        }
    }
    private class Rectangle
    {
        float width;
        float height;
        float x;
        float y;
        public Rectangle(int x,int y,int width,int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public float getX()
        {
            return x;
        }
        public float getY()
        {
            return y;
        }
        public float getWidth()
        {
            return width;
        }
        public float getHeight()
        {
            return height;
        }
    }

}
