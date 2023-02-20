using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : LivingEntity
{
    private Collider2D collider;

    private void Awake()
    {
        attackRange = 1.0f;
        attackDamage = 1;
        enemyLayers = LayerMask.GetMask("Enemies");

        isMeleeAttack = true;

        projectileForce = 20f;

        collider = this.GetComponent<Collider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        thisPosition = new Vector2(this.gameObject.transform.position.x + Input.GetAxisRaw("Horizontal"), this.gameObject.transform.position.y + Input.GetAxisRaw("Vertical"));

        attackPoint = GameObject.FindGameObjectWithTag("AttackPoint").transform;

        PlayerInput();
        AttackPointPosition();
    }

    private void PlayerInput()
    {
        // Weapon Switch
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (isMeleeAttack)
            {
                case true:
                    isMeleeAttack = false;
                    break;
                case false:
                    isMeleeAttack = true;
                    break;
            }

        }

        // Weapon attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch(isMeleeAttack)
            {
                case true:
                    MeleeAttack(this.gameObject.GetComponent<Animator>());
                    break;
                case false:
                    RangedAttack(attackPoint, this.gameObject.GetComponent<Animator>());
                    break;
            }

        }
    }

    protected override void MeleeAttack(Animator temp)
    {
        base.MeleeAttack(temp);

        // Melee Attack

        // Sets Variables
        animator = temp;

        // Play melee attack animation
        animator.SetTrigger("Attack");
        animator.SetBool("Ranged", false);

        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            
            enemy.GetComponent<LivingEntity>().Damage(attackDamage);

            //Damage(attackDamage);

        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void AttackPointPosition()
    {
        attackPoint.position = thisPosition;
    }

    protected override void RangedAttack(Transform transform, Animator temp)
    {
        base.RangedAttack(transform, temp);


        // Set variables
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        animator = temp;


        // Plays animation
        animator.SetTrigger("Attack");
        animator.SetBool("Ranged", true);


        // Spawn projectile
        if (direction == Vector2.zero) 
        {
            direction = new Vector2(0f, -1);
        }
        
        float rotationOnZAxis = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
        
        GameObject projectile = Instantiate(pfProjectile, attackPoint.position, Quaternion.Euler(0, 0, rotationOnZAxis));

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Collider2D colProjectile = projectile.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(colProjectile, collider);
        rb.AddForce(direction * projectileForce, ForceMode2D.Impulse);
    }
}
