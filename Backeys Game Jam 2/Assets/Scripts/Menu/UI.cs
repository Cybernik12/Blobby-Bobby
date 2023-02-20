using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject projectileIcon;
    [SerializeField] private GameObject meleeIcon;

    private bool isMelee = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            if(!isMelee)
            {

                projectileIcon.SetActive(false);
                meleeIcon.SetActive(true);

                isMelee = true;
            }
            else
            {
                projectileIcon.SetActive(true);
                meleeIcon.SetActive(false);

                isMelee = false;
            }
        }
    }
}
