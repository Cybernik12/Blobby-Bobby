using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Waiting,
    Old,
    MiddleAged,
    Baby
}

public class Boss : LivingEntity
{
    // Start is called before the first frame update
    [SerializeField] Transform pupils;
    [SerializeField] float viewRadius;
    Collider2D col;
    Transform playerTransform;
    Vector3 playerDisplacement;

    /*Old stage*/
    [SerializeField] float anticipateTime;
    [SerializeField] float waitTime;
    [SerializeField] int minOldHealth;

    /*Mid stage*/
    [SerializeField] Transform leftMostPosition;
    [SerializeField] Transform rightMostPosition;
    [SerializeField] AnimationCurve velocityCurve;
    [SerializeField] float midWaitTime = 0.5f;
    [SerializeField] float travelTime = 2f;
    [SerializeField] GameObject deathParticleSystem;
    bool movingLeft = true;
    
    BossState state;

    [Header("Audio")]
    [SerializeField] private AudioClip bossTheme;
    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        state = BossState.Waiting;
        currentHealth = maxHealth;
        col = GetComponent<Collider2D>();
    }

    public override void Damage(int damage)
    {
        currentHealth--;

        StopAllCoroutines();
        if (state == BossState.Old)
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(BossOldFight());
        }
        else if (state == BossState.MiddleAged) {
            animator.SetTrigger("MidHurt");
            StartCoroutine(BossMidFight());
        }
        if (currentHealth <= 0) 
        {
            Death();
        }

    }
    private void Update()
    {
        Debug.Log("update?");
        playerDisplacement = playerTransform.position - transform.position;
        if(state == BossState.Waiting && playerDisplacement.magnitude < viewRadius) 
        {

            audioSource.clip = bossTheme;
            audioSource.Play();

            animator.SetTrigger("Old"); //This is where the boss is activated
            state = BossState.Old;
            StartCoroutine(BossOldFight());
        }
    }
    private void FireProjectile(Vector3 playerDir) 
    {
        float zAxisRot = Mathf.Atan2(-playerDir.x, playerDir.y) * Mathf.Rad2Deg;
        GameObject slimeProjectile = Instantiate(pfProjectile, transform.position, Quaternion.Euler(0f, 0f, zAxisRot));
        
        Rigidbody2D rbProjectile = slimeProjectile.GetComponent<Rigidbody2D>();
        Collider2D colProjectile = slimeProjectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(colProjectile, col);
        if (rbProjectile != null) 
        {
            rbProjectile.AddForce(playerDir * projectileForce, ForceMode2D.Impulse);
        }
    }

    IEnumerator FireProjectiles(float numProjectiles) 
    {
        Vector3 playerDir = playerDisplacement.normalized;
        Vector3 maxDir = Quaternion.AngleAxis(30f, Vector3.forward) * playerDir;
        Vector3 minDir = Quaternion.AngleAxis(-30f, Vector3.forward) * playerDir;
        float curAngle = 0f;
        for (int i = 0; i < numProjectiles; i++) 
        {
            Vector3 currentDir = Vector3.Lerp(minDir, maxDir, i / (numProjectiles - 1f));
            FireProjectile(currentDir);
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator BossOldFight() 
    {
        while (currentHealth >= minOldHealth) 
        {
            animator.SetBool("Anticipate", true);
            yield return new WaitForSeconds(anticipateTime);
            animator.SetBool("Anticipate", false);
            animator.SetTrigger("Fire");
            FireProjectile(playerDisplacement.normalized);
            yield return new WaitForSeconds(waitTime);
        }
        Debug.Log("leaving coroutine");
        state = BossState.MiddleAged;
        StopAllCoroutines();
        yield return StartCoroutine(BossMidFight());
    }

    IEnumerator Move(Vector3 targetPos) 
    {
        Vector3 startPos = transform.position;
        float maxDist = (transform.position - targetPos).magnitude;
        float dist = maxDist;
        float curTime = 0f;
        while (dist > 0.1)
        {

            animator.SetBool("MidMoving", true);
            dist = (transform.position - leftMostPosition.position).magnitude;
            transform.position = Vector3.Lerp(startPos, leftMostPosition.position, velocityCurve.Evaluate(curTime / travelTime));
            curTime += Time.deltaTime;
            if (dist < 1) animator.SetBool("MidMoving", false);
            yield return null;
        }
    }
    IEnumerator BossMidFight()
    {
        animator.SetTrigger("Mid");
        yield return new WaitForSeconds(midWaitTime);
        while (currentHealth > 0)
        {
            Vector3 firstPosition = (movingLeft) ? leftMostPosition.position : rightMostPosition.position;
            Vector3 secondPosition = (movingLeft) ? rightMostPosition.position : leftMostPosition.position;
            Vector3 startPos = transform.position;
            float maxDist = (transform.position - firstPosition).magnitude;
            float dist = maxDist;
            float curTime = 0f;
            while(dist > 0.1)
            {
                
                animator.SetBool("MidMoving", true);
                dist = (transform.position - firstPosition).magnitude;
                transform.position = Vector3.Lerp(startPos, firstPosition, velocityCurve.Evaluate(curTime / travelTime));
                curTime += Time.deltaTime;
                if(dist < 1) animator.SetBool("MidMoving", false);
                yield return null;
            }
            movingLeft = !movingLeft;
            yield return new WaitForSeconds(midWaitTime);
            animator.SetTrigger("Fire");
            yield return FireProjectiles(10);
            yield return new WaitForSeconds(midWaitTime);
            maxDist = (transform.position - secondPosition).magnitude;
            dist = maxDist;
            curTime = 0f;
            startPos = transform.position;
            while (dist > 0.1)
            {
                animator.SetBool("MidMoving", true);
                dist = (transform.position - secondPosition).magnitude;
                transform.position = Vector3.Lerp(startPos, secondPosition, velocityCurve.Evaluate(curTime / travelTime));
                curTime += Time.deltaTime;
                if (dist < 1) animator.SetBool("MidMoving", false);
                yield return null;
            }
            movingLeft = !movingLeft;
            yield return new WaitForSeconds(midWaitTime);
            animator.SetTrigger("Fire");
            yield return FireProjectiles(10);
            yield return new WaitForSeconds(midWaitTime);
        }
        
    }
    public override void Death() 
    {

        playerMovement.HasWon = true;

        GameObject particleSystem = Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
        Destroy(this.gameObject);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
