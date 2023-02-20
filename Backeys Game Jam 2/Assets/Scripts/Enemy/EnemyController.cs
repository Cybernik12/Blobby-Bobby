using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Roaming,
    Chasing,
    Attacking

}
public class EnemyController : LivingEntity
{
    public float waitTime;
    public float startAttackRange;
    [SerializeField] float attackTime = 0.5f;
    float timeSinceLastAttack = 0f;
    [SerializeField] Transform pathHolder; //This object will have a number of children whose positions define the path that the enemy will roam along
    [SerializeField] float viewRadius; //If the player enters within this radius, the enemy will move towards the player
    SpriteRenderer sr;
    EnemyState state;
    Transform playerTransform;
    private Vector3 startPos;
    bool travellingAway;
    float curSpeed;
    Vector3 playerDisplacement;
    Vector3 dirToPlayer;



    private new void Awake()
    {
        currentHealth = maxHealth;
        //startAttackRange = attackRange * 2;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        state = EnemyState.Roaming;
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0;
        Vector3[] wayPoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(FollowPath(wayPoints));
    }

    /*This allows the enemy to follow a path determined by a set of empty transforms. Useful for patrolling before having seen the player*/
    IEnumerator FollowPath(Vector3[] wayPoints)
    {
        transform.position = wayPoints[0];
        int targetIndex = 1;
        Vector3 targetWayPoint = wayPoints[targetIndex];

        while (true)
        {
            //transform.position = Vector3.MoveTowards(transform.position, targetWayPoint, walkSpeed * Time.deltaTime);

            rb.velocity = (targetWayPoint - transform.position).normalized * walkSpeed;
            if (Vector3.Distance(transform.position, targetWayPoint) < 0.1f)
            {
                targetIndex = (targetIndex + 1) % wayPoints.Length;
                targetWayPoint = wayPoints[targetIndex];
                rb.velocity = Vector3.zero;
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
    }
    public override void Damage(int damage)
    {
        base.Damage(damage);
        animator.SetTrigger("Hurt");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public override void Death()
    {
        GameObject.Destroy(this.gameObject);
    }

    protected void StartAttack() 
    {
        if (timeSinceLastAttack > attackTime)
        {
            animator.SetTrigger("Attack");
        }
    }
    protected void Attack()
    {
        timeSinceLastAttack = 0;
        rb.velocity = Vector3.zero;
        animator.SetTrigger("Attack");

        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<LivingEntity>().Damage(attackDamage);
        }

    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        animator.SetFloat("Speed", rb.velocity.magnitude * 2);

        if (playerTransform != null)
        {
            playerDisplacement = playerTransform.position - transform.position;
            dirToPlayer = playerDisplacement.normalized;
        }
        //Flip the sprite depending on what side of the enemy the player is on
        float angleToPlayer = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        bool facingRight = (Mathf.Abs(angleToPlayer) < 90) ? true : false;
        Vector3 localScale = transform.localScale;
        if (!facingRight) localScale.x = -Mathf.Abs(localScale.x);
        else localScale.x = Mathf.Abs(localScale.x);
        transform.localScale = localScale;

        if (state == EnemyState.Roaming)
        {

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, viewRadius);
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.tag == "Player")
                {
                    playerTransform = col.gameObject.transform;
                    state = EnemyState.Chasing;
                    StopAllCoroutines();
                    break;
                }
            }
        }
        else if (state == EnemyState.Chasing)
        {
            Debug.Log("chasing");
            //rb.MovePosition(transform.position + dirToPlayer * runSpeed * Time.deltaTime);
            rb.velocity = dirToPlayer * walkSpeed;
            float dstToPlayer = playerDisplacement.magnitude;
            if (dstToPlayer <= startAttackRange)
            {
                state = EnemyState.Attacking;
            }
        }
        else if (state == EnemyState.Attacking)
        {
            StartAttack();
            float dstToPlayer = (playerTransform.position - transform.position).magnitude;
            sr.color = Color.red;
            if (dstToPlayer > startAttackRange)
            {
                state = EnemyState.Chasing;
                sr.color = Color.white;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, startAttackRange);
    }





    /* public Transform target;
     Vector2[] path;
     int targetIndex;
     Vector3 prevPos;
     void Start()
     {
         target = GameObject.FindGameObjectWithTag("Player").transform;
         Debug.Log(transform.position + ", " + target.position);
         PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
         prevPos = target.position;
     }

     private void Update()
     {
         if (Vector2.Distance(prevPos, target.position) < 0.01f)
         {
             PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
         }
         prevPos = target.position;
     }

     //once we get our new path from the pathrequestmanager, we call this method as a callback
     public void OnPathFound(Vector2[] newPath, bool pathSuccess)
     {
         Debug.Log(pathSuccess);
         if (pathSuccess)
         {
             path = newPath;
             Debug.Log("path length: "+path.Length);
             StopAllCoroutines();//destroy any old, unneeded paths
             StartCoroutine(FollowPath());
         }
     }
     IEnumerator FollowPath()
     {
         Vector2 currentWayPoint = path[0];

         while (true)
         {
             if (transform.position.x == currentWayPoint.x && transform.position.y == currentWayPoint.y)
             {
                 targetIndex++; //If we've reached a given waypoint, start moving to the next one
                 if (targetIndex >= path.Length)
                 {
                     yield break; //exit out of the coroutine - the path has finished being followed
                 }
                 currentWayPoint = path[targetIndex];
             }
             transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, walkSpeed * Time.deltaTime);
             yield return null;
         }
     }

     public void OnDrawGizmos()
     {
         if (path != null)
         {
             for (int i = targetIndex; i < path.Length; i++)
             {
                 Gizmos.color = Color.black;
                 Gizmos.DrawCube(path[i], Vector3.one);
                 if (i == targetIndex)
                 {
                     Gizmos.DrawLine(transform.position, path[i]);
                 }
             }
         }
     }*/
    // Update is called once per frame
}
