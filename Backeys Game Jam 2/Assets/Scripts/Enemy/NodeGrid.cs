
using System.Collections.Generic;
using UnityEngine;
/*Help from here: https://www.youtube.com/watch?v=mZfyt03LDH4&t=526s*/

/*This class creates a grid of nodes to allow our enemy npcs to perform A* pathfinding. In our algorithm, we will set the distance between two horizontally or
 vertically adjacent nodes to be 10. Hence, by pythagoras, the distance between two diagonal nodes is sqrt(1o^2 + 10^2) = roughly 14*/

public class NodeGrid : MonoBehaviour
{
    Transform player;
    public LayerMask obstacleLayerMask;
    public Vector2 gridWorldSize;
    GridNode[,] grid; //jagged array of nodes
    public float nodeWidth;
    int gridSizeX;
    int gridSizeY;
    SpriteRenderer groundSprite;
    //public List<GridNode> path;
    void Awake()
    {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeWidth);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeWidth);
        CreateGrid();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void CreateGrid()
    {

        /*The for loop below generates a grid of nodes, each of which is defined as walkable or unwalkable depending on its layer.
         * The for loop starts at -gridSize/2 so that the grid
         * is generated such that the transform.position of the ground plane is at the centre of the grid.
         */
        grid = new GridNode[gridSizeX, gridSizeY];
        for (int x = -gridSizeX / 2; x < gridSizeX / 2; x++)
        {
            for (int y = -gridSizeY / 2; y < gridSizeY / 2; y++)
            {
                Vector2 worldPoint = Vector2.right * (x * nodeWidth) + Vector2.up * (y * nodeWidth); //Get the actual position of the node in world space
                int nodeGridX = x + gridSizeX / 2; //add gridSize/2 since we need to index into the jagged array. You can't have negative indices
                int nodeGridY = y + gridSizeY / 2;
                Collider2D[] cols = Physics2D.OverlapCircleAll(worldPoint, nodeWidth / 2, obstacleLayerMask); //Check if there are any colliders over a given node
                bool walkable = (cols.Length == 0 || cols == null); //if no colliders are detected then the node is walkable.
                Debug.Log(nodeGridX + ", " + nodeGridY);
                grid[nodeGridX, nodeGridY] = new GridNode(walkable, worldPoint, nodeGridX, nodeGridY);
            }
        }
    }

    /*This method returns all the neighbouring nodes of a given node. A node can have a maximum of 8 neighbours (2 horizontal, 2 vertical, 4 diagonal)*/

    public List<GridNode> GetNeighbours(GridNode node) 
    {
        List<GridNode> neighbours = new List<GridNode>();
        int nodeX = node.gridXCoord;
        int nodeY = node.gridYCoord;
        for (int x = -1; x <= 1; x++) 
        {
            for (int y = -1; y <= 1; y++) 
            {
                if (x == 0 && y == 0) continue; //A node cannot be its own neighbour.
                int curX = nodeX + x;
                int curY = nodeY + y;
                if (curX >= 0 && curX < gridSizeX && curY >= 0 && curY < gridSizeY) 
                {
                    neighbours.Add(grid[curX, curY]);
                }
            }
        }
        return neighbours;
    }

    //Get the node's coordinate in the grid from a given position in world space. This is used to locate the player's position in the grid for pathfinding by the ai.
    public GridNode NodeFromWorldPoint(Vector3 worldPos) 
    {
        float percentX = Mathf.Clamp01((worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x); 
        float percentY = Mathf.Clamp01((worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y);

        int nodeCoordX = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int nodeCoordY = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        Debug.Log(gridSizeX + ", " + gridSizeY);
        return grid[nodeCoordX, nodeCoordY];
    }

    /*DrawGizmos just helps to visualise the nodes. Red nodes are obstacles, the blue node is the player.*/
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0f));
       /* if (grid != null)
        {
            GridNode playerNode = NodeFromWorldPoint(player.position);
            foreach (GridNode node in grid)
            {
                if (node != null) 
                {
                    if (node == playerNode)
                    {
                        Gizmos.color = Color.cyan;
                        Debug.Log(path);
                    }
                    else if (path != null && path.Contains(node)) 
                    {
                        Debug.Log("hey hey heeey!");
                        Gizmos.color = Color.black;
                    }
                    else
                    {
                        Gizmos.color = (node.walkable) ? Color.white : Color.red;
                    }
                        

                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeWidth - .1f));
                }
                
            }
        }*/
    }
    // Update is called once per frame
    void Update()
    {

    }
}