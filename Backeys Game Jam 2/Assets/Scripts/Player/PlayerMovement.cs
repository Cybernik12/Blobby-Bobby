using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : LivingEntity
{

    private bool hasWon;

    public bool HasWon
    {
        get { return hasWon; }
        set { hasWon = value; }
    }

    public int Health
    {
        get { return maxHealth; }
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    private void Awake()
    {
        dodgePower = 5000f; // No idea why it needs to be this high to actually do anything
        dodgeTime = 0.2f;
        dodgeCooldown = 1f;

        hasWon = false;

        this.transform.position = spawnLocation.position;

        LifeStageStats();
    }

    // Update is called once per frame
    void Update()
    {
        animator = this.gameObject.GetComponent<Animator>(); // Attaches Animator Component from the gamobject to the Animator variable on the "Living Entity" script

        animator.SetInteger("Stage", stage);

        if (currentHealth <= 0)
        {
            Death();
        }

        if (hasWon)
        {
            StartCoroutine(LoadWin());
        }

        PlayerInput();
    }

    private void FixedUpdate()
    {
        Move(this.gameObject.GetComponent<Rigidbody2D>()); // Attaches Rigidbody Component from the gamobject to the Rigidbody variable on the "Living Entity" script
    }

    // Takes the players Input
    private void PlayerInput()
    {
        // Gets the direction of the player
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Changes animation depending on direction and speed of player
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // When "Left Shift" is pressed the character will dodge. ("Left Shift" is only temporary for now)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge)
        {
            StartCoroutine(Dodge());
        }
        /*
         * for testing purposes ONLY!!!
         * 
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentHealth--;
        }
        */

    }

    // Moves the entities depending on the Input
    protected virtual void Move(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
        rb.MovePosition(rb.position + movement * walkSpeed * Time.fixedDeltaTime);
    }

    // Dodge function. Applies a force that pushes the character a distance a away. (For some reason the "dodgePower" has to be absurdly high)
    protected IEnumerator Dodge()
    {
        canDodge = false;

        animator.SetTrigger("Dash");

        rb.AddForce(movement * dodgePower);


        yield return new WaitForSeconds(dodgeCooldown);

        canDodge = true;
    }

    public override void Death()
    {
        base.Death();

        this.transform.position = spawnLocation.position;

        if (stage <= 5)
        {
            stage++;
        }

        else
        {
            GameOver();
        }

        LifeStageStats();
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);

        // Plays Hurt animation
        animator.SetTrigger("Hurt");

        currentHealth -= damage;
    }

    private IEnumerator LoadWin()
    {
        yield return new WaitForSeconds(2);

        Win();
    }

    // Change stats depending on what stage the player is in.
    protected virtual void LifeStageStats()
    {
        switch (stage)
        {
            case 1: // Child
                stage = 1;
                maxHealth = 1;
                currentHealth = maxHealth;
                walkSpeed = 3;
                canDodge = false;
                break;
            case 2: // Teen
                stage = 2;
                maxHealth = 3;
                currentHealth = maxHealth;
                walkSpeed = 5;
                canDodge = true;
                break;
            case 3: // Adult
                stage = 3;
                maxHealth = 5;
                currentHealth = maxHealth;
                walkSpeed = 7;
                canDodge = true;
                break;
            case 4: // Middle Aged
                stage = 4;
                maxHealth = 3;
                currentHealth = maxHealth;
                walkSpeed = 5;
                canDodge = true;
                break;
            case 5: // Old
                stage = 5;
                maxHealth = 1;
                currentHealth = maxHealth;
                walkSpeed = 3;
                canDodge = false;
                break;
            case >= 6:
                stage = 6;
                GameOver();
                break;
            default:
                stage = 1;
                maxHealth = 1;
                currentHealth = maxHealth;
                walkSpeed = 3;
                canDodge = false;
                break;
        }
    }
}
