using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{

    public GameObject NPC;
    public Transform[] Obstacles = new Transform[0];
    public Transform StartPos;
    public Transform EndPos;
    public float size;
    public int layers;
    public int moveSpeed;
    public bool start;
    Octree<bool> octree;
    List<Octree<bool>.OctreeNode<bool>> OpenList;
    List<Octree<bool>.OctreeNode<bool>> ClosedList;
    Octree<bool>.OctreeNode<bool> startNode;
    Octree<bool>.OctreeNode<bool> endNode;
    Octree<bool>.OctreeNode<bool> currentNode;
    Octree<bool>.OctreeNode<bool> currentNeighbour;
    List<Octree<bool>.OctreeNode<bool>> finalPath;
    int index;
    int pathindex;
    int turningSpeed;
    bool pathFollowed;
    bool pathIsSimple;
    bool pathcalculated;
    bool destinationReached;
    private void CreateTree()
    {
        octree = new Octree<bool>(this.transform.position, size, layers);
    }
    private void Start()
    {
        CreateTree();
        octree.GetRoot().Layer = layers;
        foreach (var obstacle in Obstacles)
        {
            octree.Insert(obstacle.position, true, layers - 1);
        }
        AddParent(octree.GetRoot());
        SetNeighbours(octree.GetRoot());
        startNode = PositionToNode(octree.GetRoot(), StartPos.position);
        endNode = PositionToNode(octree.GetRoot(), EndPos.position);
        OpenList = new List<Octree<bool>.OctreeNode<bool>>();
        ClosedList = new List<Octree<bool>.OctreeNode<bool>>();
        startNode.GCost = 0;
        startNode.HCost = Vector3.Distance(startNode.Position, endNode.Position);
        NPC.transform.position = StartPos.position;
        pathindex = 0;
        OpenList.Add(startNode);
        OpenList = OpenList.OrderBy(node => node.FCost).ToList();
        finalPath = new List<Octree<bool>.OctreeNode<bool>>();
        if (startNode == endNode)
        {
            pathIsSimple = true;
        }
        turningSpeed = 5;
    }

    private void Update()
    {
        if (start)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            finalPath = Pathfinder();
            stopWatch.Stop();
            Debug.Log(stopWatch.ElapsedMilliseconds);
            start = false;
            pathcalculated = true;
        }
        if (pathcalculated && !pathFollowed)
        {
            if (pathIsSimple)
            {
                Vector3 targetDirection = EndPos.position - NPC.transform.position;
                NPC.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(NPC.transform.forward, targetDirection, turningSpeed * Time.deltaTime, 0.0f));
                NPC.transform.position = Vector3.MoveTowards(NPC.transform.position, EndPos.position, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(NPC.transform.position, endNode.Position) < 0.01f)
                {
                    pathFollowed = true;
                    destinationReached = true;
                }
            }
            else
            {
                Vector3 targetDirection = finalPath[pathindex].Position - NPC.transform.position;
                NPC.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(NPC.transform.forward, targetDirection, turningSpeed * Time.deltaTime, 0.0f));
                NPC.transform.position = Vector3.MoveTowards(NPC.transform.position, finalPath[pathindex].Position, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(NPC.transform.position, finalPath[pathindex].Position) < 0.01f)
                {
                    if (pathindex == finalPath.Count - 1)
                    {
                        pathFollowed = true;
                    }
                    pathindex += 1;
                }
            }
        
        }
        if (pathFollowed && !destinationReached)
        {
            Vector3 targetDirection = EndPos.position - NPC.transform.position;
            NPC.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(NPC.transform.forward, targetDirection, turningSpeed * Time.deltaTime, 0.0f));
            NPC.transform.position = Vector3.MoveTowards(NPC.transform.position, EndPos.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(NPC.transform.position, EndPos.position) < 0.01f)
            {
                destinationReached = true;
            }
        }
    }
    private List<Octree<bool>.OctreeNode<bool>> Pathfinder()
    {
        while (OpenList.Count > 0)
        {
            OpenList = OpenList.OrderBy(node => node.FCost).ToList();
            currentNode = OpenList[0];

            if (currentNode == endNode)
            {
                break;
            }

            CheckNeighbours(currentNode.Neighbours, currentNode);
            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

        }
        
        if (currentNode != endNode)
        {
            return null;
        }
        else
        {
            List<Octree<bool>.OctreeNode<bool>> path = new List<Octree<bool>.OctreeNode<bool>>();
            path.Add(currentNode);
            while (currentNode != startNode)
            {
                path.Add(currentNode.Connection);
                currentNode = currentNode.Connection;
            }
            path.Reverse();
            return path;
        }
    }

    private void CheckNeighbours(Octree<bool>.OctreeNode<bool>[] neighbours, Octree<bool>.OctreeNode<bool> node)
    {
        foreach (var neighbour in neighbours)
        {
            if (neighbour != null)
            {
                if (neighbour.Blocked)
                {
                    continue;
                }
                if (neighbour.SubNodes != null)
                {
                    int index = FindIndex(node.Neighbours, neighbour);
                    Octree<bool>.OctreeNode<bool>[] subNeighbours = new Octree<bool>.OctreeNode<bool>[4];
                    if (index != 20)
                    {
                        switch (index)
                        {
                                case 0:
                                        subNeighbours[0] = neighbour.SubNodes[2];
                                        subNeighbours[1] = neighbour.SubNodes[3];
                                        subNeighbours[2] = neighbour.SubNodes[6];
                                        subNeighbours[3] = neighbour.SubNodes[7];
                                    break;
                                case 1:
                                        subNeighbours[0] = neighbour.SubNodes[0];
                                        subNeighbours[1] = neighbour.SubNodes[1];
                                        subNeighbours[2] = neighbour.SubNodes[4];
                                        subNeighbours[3] = neighbour.SubNodes[5];
                                    break;
                                case 2:
                                        subNeighbours[0] = neighbour.SubNodes[4];
                                        subNeighbours[1] = neighbour.SubNodes[5];
                                        subNeighbours[2] = neighbour.SubNodes[6];
                                        subNeighbours[3] = neighbour.SubNodes[7];
                                    break;
                                case 3:
                                        subNeighbours[0] = neighbour.SubNodes[0];
                                        subNeighbours[1] = neighbour.SubNodes[1];
                                        subNeighbours[2] = neighbour.SubNodes[2];
                                        subNeighbours[3] = neighbour.SubNodes[3];
                                
                                    break;
                                case 4:
                                        subNeighbours[0] = neighbour.SubNodes[1];
                                        subNeighbours[1] = neighbour.SubNodes[3];
                                        subNeighbours[2] = neighbour.SubNodes[5];
                                        subNeighbours[3] = neighbour.SubNodes[7];
                                
                                    break;
                                case 5:
                                        subNeighbours[0] = neighbour.SubNodes[0];
                                        subNeighbours[1] = neighbour.SubNodes[2];
                                        subNeighbours[2] = neighbour.SubNodes[4];
                                        subNeighbours[3] = neighbour.SubNodes[6];
                                    break;
                                case 6:
                                        subNeighbours[0] = neighbour.SubNodes[0];
                                        subNeighbours[1] = neighbour.SubNodes[1];
                                    break;
                                case 7:
                                        subNeighbours[0] = neighbour.SubNodes[4];
                                        subNeighbours[1] = neighbour.SubNodes[5];
                                    break;
                                case 8:
                                        subNeighbours[0] = neighbour.SubNodes[6];
                                        subNeighbours[1] = neighbour.SubNodes[7];
                                    break;
                                case 9:
                                        subNeighbours[0] = neighbour.SubNodes[2];
                                        subNeighbours[1] = neighbour.SubNodes[3];
                                    break;
                                case 10:
                                        subNeighbours[0] = neighbour.SubNodes[1];
                                        subNeighbours[1] = neighbour.SubNodes[3];
                                    break;
                                case 11:
                                        subNeighbours[0] = neighbour.SubNodes[5];
                                        subNeighbours[1] = neighbour.SubNodes[7];
                                    break;
                                case 12:
                                        subNeighbours[0] = neighbour.SubNodes[4];
                                        subNeighbours[1] = neighbour.SubNodes[6];
                                    break;
                                case 13:
                                        subNeighbours[0] = neighbour.SubNodes[0];
                                        subNeighbours[1] = neighbour.SubNodes[2];
                                    break;
                                case 14:
                                        subNeighbours[0] = neighbour.SubNodes[0];
                                        subNeighbours[1] = neighbour.SubNodes[4];
                                    break;
                                case 15:
                                        subNeighbours[0] = neighbour.SubNodes[1];
                                        subNeighbours[1] = neighbour.SubNodes[5];
                                    break;
                                case 16:
                                        subNeighbours[0] = neighbour.SubNodes[3];
                                        subNeighbours[1] = neighbour.SubNodes[7];
                                    break;
                                case 17:
                                        subNeighbours[0] = neighbour.SubNodes[2];
                                        subNeighbours[1] = neighbour.SubNodes[6];
                                    break;
                            default:
                                    break;
                        }
                    }
                    CheckNeighbours(subNeighbours, node);
                    continue;
                }

                    Vector3 diff = neighbour.Position - node.Position;
                    if (Mathf.Abs(diff.x) > node.Size / 2 && Mathf.Abs(diff.y) > node.Size / 2 || Mathf.Abs(diff.x) > node.Size / 2 && Mathf.Abs(diff.z) > node.Size / 2 || Mathf.Abs(diff.y) > node.Size / 2 && Mathf.Abs(diff.z) > node.Size / 2)
                    {
                        bool check = false;
                        float offset = 0;
                        if (diff.x != 0 && diff.y != 0)
                        {

                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(-0.1f, -0.1f, 0)) + new Vector3(Mathf.Abs(diff.x / 10), 0, 0);
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(0, 0, offset));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(-0.1f, -0.1f, 0)) + new Vector3(Mathf.Abs(diff.x / 10), 0, 0);
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(0, 0, -offset));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(-0.1f, -0.1f, 0)) + new Vector3(0, Mathf.Abs(diff.y / 10), 0);
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(0, 0, offset));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(-0.1f, -0.1f, 0)) + new Vector3(0, Mathf.Abs(diff.y / 10), 0);
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(0, 0, -offset));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                        }
                        else if (diff.x != 0 && diff.z != 0)
                        {
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(-0.1f, 0, -0.1f)) + new Vector3(Mathf.Abs(diff.x / 10), 0, 0);
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(0, offset, 0));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(-0.1f, 0, -0.1f)) + new Vector3(Mathf.Abs(diff.x / 10), 0, 0);
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(0, -offset, 0));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(-0.1f, 0, -0.1f)) + new Vector3(0, 0, Mathf.Abs(diff.z / 10));
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(0, offset, 0));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(-0.1f, 0, -0.1f)) + new Vector3(0, 0, Mathf.Abs(diff.z / 10));
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(0, -offset, 0));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                        }
                        else if (diff.y != 0 && diff.z != 0)
                        {
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(0, -0.1f, -0.1f)) + new Vector3(0, Mathf.Abs(diff.y / 10), 0);
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(offset, 0, 0));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(0, -0.1f, -0.1f)) + new Vector3(0, Mathf.Abs(diff.y / 10), 0);
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(-offset, 0, 0));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }

                            offset = 0;
                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(0, -0.1f, -0.1f)) + new Vector3(0, 0, Mathf.Abs(diff.z / 10));
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(offset, 0, 0));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                            offset = 0;

                            for (int i = 0; i < node.Size / (octree.GetRoot().Size / (Mathf.Pow(2, layers) / 2)); i++)
                            {
                                Vector3 newPosition = node.Position + ((diff / 2) - new Vector3(0, -0.1f, -0.1f)) + new Vector3(0, 0, Mathf.Abs(diff.z / 10));
                                Octree<bool>.OctreeNode<bool> newNode = PositionToNode(octree.GetRoot(), newPosition + new Vector3(-offset, 0, 0));
                                if (!newNode.Blocked)
                                {
                                    check = true;
                                    break;
                                }
                                offset += octree.GetRoot().Size / (Mathf.Pow(2, layers));
                            }
                        }
                        if (!check)
                        {
                            continue;
                        }
                    }

                neighbour.GCost = node.GCost + Vector3.Distance(neighbour.Position, node.Position);
                neighbour.HCost = Vector3.Distance(neighbour.Position, endNode.Position);

                if (ClosedList.Contains(neighbour))
                {
                    currentNeighbour = ClosedList.Find(Node => Node.Position == neighbour.Position);
                    if (currentNeighbour.GCost <= neighbour.GCost)
                    {
                        continue;
                    }

                    ClosedList.Remove(currentNeighbour);
                }
                else if (OpenList.Contains(neighbour))
                {
                    currentNeighbour = OpenList.Find(Node => Node.Position == neighbour.Position);

                    if (currentNeighbour.GCost <= neighbour.GCost)
                    {
                        continue;
                    }
                }
                else
                {
                    currentNeighbour = neighbour;
                }

                currentNeighbour.GCost = neighbour.GCost;
                currentNeighbour.Connection = node;

                if (!OpenList.Contains(neighbour))
                {
                    OpenList.Add(currentNeighbour);
                }
            }
            else
            {
                continue;
            }
        }
    }

    private int FindIndex(Octree<bool>.OctreeNode<bool>[] neighbours, Octree<bool>.OctreeNode<bool> node)
    {
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != null)
            {
                if (neighbours[i] == node)
                {
                    return i;
                }
            }
        }
        return 20;
    }
    private void OnDrawGizmos()
    {
        if (octree != null)
        {
            DrawNode(octree.GetRoot());
        }
        if (finalPath != null)
        {
            foreach (var item in finalPath)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(item.Position, Vector3.one * item.Size);
            }
        }
        
    }

    public Octree<bool>.OctreeNode<bool> PositionToNode(Octree<bool>.OctreeNode<bool> node, Vector3 position)
    {
        index = octree.GetIndexOfPosition(position, node.Position);
        if (position.x > size / 2 || position.x < -size / 2 || position.y > size / 2 || position.y < -size / 2 || position.z > size / 2 || position.z < -size / 2)
        {
            return null;
        }
        else if (!node.IsChildlessNode(node))
        {
            if (node != null)
            {
                Vector3 nodePosition = node.SubNodes[index].Position;
                node = node.SubNodes[index];
                node = PositionToNode(node, position);
                return node;
            }
            return node;
        }
        else
        {
            return node;
        }
    }

    private void SetNeighbours(Octree<bool>.OctreeNode<bool> node)
    {
        Octree<bool>.OctreeNode<bool>[] nodes;
        nodes = node.SubNodes;
        for (int i = 0; i < nodes.Length; i++)
        {
            Octree<bool>.OctreeNode<bool>[] neighbours = new Octree<bool>.OctreeNode<bool>[18];
            for (int j = 0; j < neighbours.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x - nodes[i].Size, nodes[i].Position.y, nodes[i].Position.z));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 1:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x + nodes[i].Size, nodes[i].Position.y, nodes[i].Position.z));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 2:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x, nodes[i].Position.y - nodes[i].Size, nodes[i].Position.z));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 3:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x, nodes[i].Position.y + nodes[i].Size, nodes[i].Position.z));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 4:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x, nodes[i].Position.y, nodes[i].Position.z - nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 5:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x, nodes[i].Position.y, nodes[i].Position.z + nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 6:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x + nodes[i].Size, nodes[i].Position.y + nodes[i].Size, nodes[i].Position.z));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 7:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x + nodes[i].Size, nodes[i].Position.y - nodes[i].Size, nodes[i].Position.z));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 8:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x - nodes[i].Size, nodes[i].Position.y - nodes[i].Size, nodes[i].Position.z));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 9:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x - nodes[i].Size, nodes[i].Position.y + nodes[i].Size, nodes[i].Position.z));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 10:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x, nodes[i].Position.y + nodes[i].Size, nodes[i].Position.z - nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 11:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x, nodes[i].Position.y - nodes[i].Size, nodes[i].Position.z - nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 12:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x, nodes[i].Position.y - nodes[i].Size, nodes[i].Position.z + nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 13:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x, nodes[i].Position.y + nodes[i].Size, nodes[i].Position.z + nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 14:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x + nodes[i].Size, nodes[i].Position.y, nodes[i].Position.z + nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 15:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x + nodes[i].Size, nodes[i].Position.y, nodes[i].Position.z - nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 16:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x - nodes[i].Size, nodes[i].Position.y, nodes[i].Position.z - nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;
                    case 17:
                        neighbours[j] = PositionToNode(octree.GetRoot(), new Vector3(nodes[i].Position.x - nodes[i].Size, nodes[i].Position.y, nodes[i].Position.z + nodes[i].Size));
                        if (neighbours[j] != null)
                        {
                            while (neighbours[j].Layer < nodes[i].Layer)
                            {
                                neighbours[j] = neighbours[j].Parent;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            nodes[i].Neighbours = neighbours;
            if (nodes[i].SubNodes != null)
            {
                SetNeighbours(nodes[i]);
            }
        }
    }

    private void DrawNode(Octree<bool>.OctreeNode<bool> node)
    {
        if (!node.IsChildlessNode(node))
        {
            foreach (var subnode in node.SubNodes)
            {
                if (subnode != null)
                {
                    DrawNode(subnode);
                }
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);

        }
        else if (node.IsChildlessNode(node))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        }

    }

    private void AddParent(Octree<bool>.OctreeNode<bool> node)
    {
        if (!node.IsChildlessNode(node))
        {
            foreach (var subnode in node.SubNodes)
            {
                if (subnode != null)
                {
                    subnode.Parent = node;
                    AddParent(subnode);
                }
            }
            
        }
    }
}
