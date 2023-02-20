using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public bool walkable;
    public Vector2 worldPosition;
    public int gCost; //shortest distance between current node and start node
    public int hCost; //shortest distance  between current node and goalNode node
    public int gridXCoord;
    public int gridYCoord;
    public GridNode parent;
    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public GridNode(bool walkable, Vector2 worldPosition, int gridXCoord, int gridYCoord)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridXCoord = gridXCoord;
        this.gridYCoord = gridYCoord;
    }
}
