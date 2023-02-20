using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private int health;
    private int numOfHearts;

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite fullHearts;
    [SerializeField] private Sprite emptyHearts;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        health = playerMovement.CurrentHealth;

        numOfHearts = playerMovement.Health;


        for (int i = 0; i < hearts.Length; i++)
        {
            if(i < health)
            {
                hearts[i].sprite = fullHearts;
            }
            else
            {
                hearts[i].sprite = emptyHearts;
            }

            if(i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
