using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

/*This class ensures that, if we have multiple enemies attempting to move towards our player at once, the 
 path calculations are spread over multiple frames. This will prevent our game from freezing up. This class is a
singleton (I know it's bad to have singletons I'm sorry Nik) to allow our npcs to call it easily*/

public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> queue = new Queue<PathRequest>(); //stores all the requests for paths made by all the npcs in our scene currently moving towards the player
    PathRequest curPathRequest;
    public static PathRequestManager instance;
    PathFinding pathFinding;
    bool currentlyProcessingPath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        pathFinding = GetComponent<PathFinding>();
    }
    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback) 
    {
        PathRequest pr = new PathRequest(pathStart, pathEnd, callback);
        instance.queue.Enqueue(pr); //add the current request for a path to the queue.
        instance.TryProcessNextPath();
    }

    void TryProcessNextPath() 
    {
        //We only try processing the next path if we're not currently processing a task and we have more paths to process.
        if (!currentlyProcessingPath && queue.Count > 0)
        {
            curPathRequest = queue.Dequeue(); //first item is removed from the queue
            currentlyProcessingPath = true;
            pathFinding.BeginPathCalculation(curPathRequest);
        }
    }

    //This will be called, as a callback action, by the pathfinding script once it has finished calculating a path
    public void FinishedPathProcessing(Vector2[] path, bool success) 
    {
        curPathRequest.callback(path, success);
        currentlyProcessingPath = false;
        TryProcessNextPath(); //Once we're done with this path we calculate the next one.
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public struct PathRequest
{
    public Vector2 pathStart;
    public Vector2 pathEnd;
    public Action<Vector2[], bool> callback;

    public PathRequest(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}
