using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OctreeIndex
{
    LowerLeftFront = 0, //000,
    LowerRightFront = 2, //010
    LowerLeftBack = 1, //001
    LowerRightBack = 3, //011
    UpperLeftFront = 4, //100
    UpperRightFront = 6,//110
    UpperLeftBack = 5, //101
    UpperRightBack = 7, //111
}

public class Octree<NodeType>
{
    private OctreeNode<NodeType> node;
    static int count;

    public Octree (Vector3 position, float size, int layers)
    {
        node = new OctreeNode<NodeType>(position, size);
    }
    public void Insert(Vector3 position, NodeType value, int layer)
    {
        node.Subdivide(position, value, layer);
    }
    public class OctreeNode<nodeType>
    {
        Vector3 position;
        float size;
        float gCost;
        float hCost = 0;
        float weight = 1.5f;
        float fCost;
        int layer;
        bool blocked;
        OctreeNode<nodeType>[] neighbours;
        OctreeNode<nodeType>[] subNodes;
        OctreeNode<nodeType> parentNode;
        OctreeNode<nodeType> connection;

        public OctreeNode<nodeType>[] SubNodes 
        {
            get { return subNodes; }
        }

        public OctreeNode<nodeType>[] Neighbours
        {
            get { return neighbours; }
            set { neighbours = value; }
        }
        public OctreeNode<nodeType> Parent
        {
            get { return parentNode; }
            set { parentNode = value; }
        }

        public OctreeNode<nodeType> Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        public Vector3 Position
        {
            get { return position; }
        }
        public int Layer
        {
            get { return layer; }
            set { layer = value; }
        }
        public float Size
        {
            get { return size; }
        }

        public float GCost
        {
            get { return gCost; }
            set { gCost = value; }
        }

        public float HCost
        {
            get { return hCost; }
            set { hCost = value * weight; }
        }

        public float FCost
        {
            get { return gCost + hCost; }
        }

        public bool Blocked
        {
            get { return blocked; }
            set { blocked = value; }
        }

        public OctreeNode(Vector3 pos, float size)
        {
            position = pos;
            this.size = size;
        }

        public void Subdivide(Vector3 targetPosition, nodeType value, int layers)
        {
            int subdivisionIndex = GetIndexOfPosition1(targetPosition, position);
            if (subNodes == null)
            {
                subNodes = new OctreeNode<nodeType>[8];
                for (int i = 0; i < subNodes.Length; i++)
                {
                    Vector3 newPos = position;
                    if ((i & 4) == 4)
                    {   
                        newPos.y += size * 0.25f;
                    }
                    else
                    {   
                        newPos.y -= size * 0.25f;
                    }

                    if ((i & 2) == 2)
                    {   
                        newPos.x += size * 0.25f;
                    }
                    else
                    {   
                        newPos.x -= size * 0.25f;
                    }

                    if ((i & 1) == 1)
                    {   
                        newPos.z += size * 0.25f;
                    }
                    else
                    {   
                        newPos.z -= size * 0.25f;
                    }
                    Octree<NodeType>.count += 1;
                    subNodes[i] = new OctreeNode<nodeType>(newPos, size * 0.5f);
                    subNodes[i].layer = layers;
                    if (layers > 0 && subdivisionIndex == i)
                    {
                        subNodes[i].Subdivide(targetPosition, value, layers - 1);
                    }
                }
            }
            if (layers > 0)
            {
                subNodes[subdivisionIndex].Subdivide(targetPosition, value, layers - 1);
            }


            if (layers <= 0)
            {
                foreach (var node in subNodes)
                {
                    if (!node.Blocked)
                    {
                        if ((node.Position.x - node.Size / 2) < targetPosition.x && targetPosition.x < (node.Position.x + node.Size / 2))
                        {
                            if ((node.Position.y - node.Size / 2) < targetPosition.y && targetPosition.y < (node.Position.y + node.Size / 2))
                            {
                                if ((node.Position.z - node.Size / 2) < targetPosition.z && targetPosition.z < (node.Position.z + node.Size / 2))
                                {
                                    node.Blocked = true;
                                }
                            }
                        }
                    }
                }
            }
            Debug.Log(Octree<NodeType>.count);
        }

        public bool IsChildlessNode(OctreeNode<nodeType> node)
        {
            if (node.SubNodes == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLeafNode(OctreeNode<nodeType> node)
        {
            if (node.SubNodes == null && node.Layer == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public int GetIndexOfPosition(Vector3 lookupPosition, Vector3 nodePosition)
    {
        int index = 0;
        if (lookupPosition.y > nodePosition.y)
        {
            index += 4;
        }

        if (lookupPosition.x > nodePosition.x)
        {
            index += 2;
        }

        if (lookupPosition.z > nodePosition.z)
        {
            index += 1;
        }
        return index;
    }
    private static int GetIndexOfPosition1(Vector3 lookupPosition, Vector3 nodePosition)
    {
        int index = 0;
        if (lookupPosition.y > nodePosition.y)
        {
            index += 4;
        }

        if (lookupPosition.x > nodePosition.x)
        {
            index += 2;
        }

        if (lookupPosition.z > nodePosition.z)
        {
            index += 1;
        }
        return index;
    }


    public OctreeNode<NodeType> GetRoot()
    {
        return node;
    }
}


