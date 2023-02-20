using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System;

[RequireComponent(typeof(NodeGrid))] //This script requires a nodegrid instance to work
public class PathFinding : MonoBehaviour
{
    NodeGrid grid;
    //public Transform seeker;
    //public Transform target;

    private void Awake()
    {
        grid = GetComponent<NodeGrid>();
    }

    private void Update()
    {
        //CalculatePath(seeker.position, target.position);
    }

    /*The calculatePath function implements the A* search algorithm as implemented by Sebastian Lague in this video:
     https://www.youtube.com/watch?v=mZfyt03LDH4
     */
    public void BeginPathCalculation(PathRequest pr) 
    {
        StartCoroutine(CalculatePath(pr.pathStart, pr.pathEnd));
    }

    IEnumerator CalculatePath(Vector2 startPos, Vector2 targetPos) 
    {

        Vector2[] pathPoints = new Vector2[0];
        bool pathSuccess = false;
        GridNode startNode = grid.NodeFromWorldPoint(startPos);
        GridNode targetNode = grid.NodeFromWorldPoint(targetPos);

        Debug.Log(startNode.gridXCoord + ", " + startNode.gridYCoord);
        Debug.Log(targetNode.gridXCoord + ", " + targetNode.gridYCoord);
        if (startNode.walkable && targetNode.walkable) 
        {

            //Create a list (could update to use a priority queue for better performance if we have time) of nodes that we have not yet explored
            //An explored node is one that has had all its neighbours visited
            List<GridNode> unexploredNodes = new List<GridNode>();
            //Hashset of nodes that have been visited, but not explored.
            HashSet<GridNode> exploredNodes = new HashSet<GridNode>();

            unexploredNodes.Add(startNode);

            while (unexploredNodes.Count > 0) //while there are still unExplored nodes
            {
                GridNode curNode = unexploredNodes[0];
                for (int i = 1; i < unexploredNodes.Count; i++)
                {
                    if (unexploredNodes[i].fCost < curNode.fCost || unexploredNodes[i].fCost == curNode.fCost && unexploredNodes[i].hCost < curNode.hCost)
                    {
                        curNode = unexploredNodes[i];
                    }
                }
                unexploredNodes.Remove(curNode); //we have now explored the current node with the smallest fCost
                exploredNodes.Add(curNode);


                if (curNode == targetNode)
                {
                    pathSuccess = true;
                    //We've found the target, so return
                    break;
                }
                foreach (GridNode node in grid.GetNeighbours(curNode)) //Loop through all the neighbours of our current node
                {
                    if (!node.walkable || exploredNodes.Contains(node))
                    {
                        continue;
                    }
                    int newMovementCost = curNode.gCost + GetDistance(curNode, node);
                    if (newMovementCost < node.gCost || !unexploredNodes.Contains(node))
                    {
                        node.gCost = newMovementCost;
                        node.hCost = GetDistance(node, targetNode);
                        node.parent = curNode;

                        if (!unexploredNodes.Contains(node))
                        {
                            unexploredNodes.Add(node);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess) 
        {
            pathPoints = RetracePath(startNode, targetNode);
        }
        PathRequestManager.instance.FinishedPathProcessing(pathPoints, pathSuccess);
    }
    
    Vector2[] RetracePath(GridNode start, GridNode end) {
        Debug.Log("I'm being called!");
        List<GridNode> path = new List<GridNode>();
        GridNode curNode = end;
        while (curNode != start) 
        {
            path.Add(curNode);
            curNode = curNode.parent;
        }
        Vector2[] pathPoints = SimplifyPath(path);
        Array.Reverse(pathPoints);
        return pathPoints;
    }

    Vector2[] SimplifyPath(List<GridNode> path) 
    {
        List<Vector2> pathPoints = new List<Vector2>();
        Vector2 oldDirection = Vector2.zero;
        for (int i = 1; i < path.Count; i++) 
        {
            int newDirX = path[i - 1].gridXCoord - path[i].gridXCoord;
            int newDirY = path[i - 1].gridYCoord - path[i].gridYCoord;
            Vector2 newDirection = new Vector2(newDirX, newDirY);
            if (oldDirection != newDirection) //if the path has changed direction, we add this point to our list of path points
            {
                pathPoints.Add(path[i].worldPosition);
            }
            oldDirection = newDirection; 
        }
        return pathPoints.ToArray();
    }
    int GetDistance(GridNode a, GridNode b) 
    {
        //The shortest path will move diagonally as many times as possible. The remaining horizontal or vertical moves depend on if the x or y distance is greater
        int dstX = Mathf.Abs(a.gridXCoord - b.gridYCoord);
        int dstY = Mathf.Abs(a.gridYCoord - b.gridYCoord);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY); //we move diagonally dstY times, and move horizontally dstX - dstY times 
        }
        else {
            return 14 * dstX + 10 * (dstY - dstX);
        }

    }

}
