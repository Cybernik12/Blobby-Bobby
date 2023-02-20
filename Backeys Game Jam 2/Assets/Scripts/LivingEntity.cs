using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LivingEntity : MonoBehaviour
{
    [SerializeField] protected Transform spawnLocation;

    [SerializeField] protected AudioSource audioSource;

    [SerializeField] protected float walkSpeed;
    protected float runSpeed;

    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    protected int stage; // What stage of life the player is in. There will be 5 stages in total.

    // Dodge/Roll variables
    protected bool canDodge;
    protected float dodgePower;
    protected float dodgeTime;
    protected float dodgeCooldown;

    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator animator;

    protected Vector2 thisPosition;

    protected Vector2 movement;

    // Attack Variables

    protected bool isMeleeAttack;

    // Melee
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected LayerMask enemyLayers;

    // Ranged
    [SerializeField] protected GameObject pfProjectile;
    [SerializeField] protected float projectileForce;

    
    // When the character dies, the stage of the character will increase and the stats will change
    public virtual void Death() 
    {
        // Code that runs when entity dies        
    }

    // Decreases health depending on damage taken (At the moment its just set to 1)
    public virtual void Damage(int damage)
    {
        // currentHealth--;
    }

    protected virtual void MeleeAttack(Animator temp)
    {
        // Melee Attack
    }

    protected virtual void RangedAttack(Transform transform, Animator temp)
    {
        // Ranged Attack
    }

    protected virtual void MagicAttack()
    {
        // Magic Attack
    }
    protected virtual void Win()
    {
        SceneManager.LoadScene(2);
    }

    protected virtual void GameOver()
    {
        SceneManager.LoadScene(3);
    }
}
