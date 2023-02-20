using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask enemy;

    // Start is called before the first frame update
    [SerializeField] int damage;
    void Start()
    {
        damage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LivingEntity entity = collision.collider.transform.gameObject.GetComponent<LivingEntity>();
        Projectile otherProj = collision.collider.transform.gameObject.GetComponent<Projectile>();
        Debug.Log(entity);
        if (entity != null /*&& entity.gameObject.layer == enemy*/) 
        {
            entity.Damage(damage);
            Destroy(this.gameObject);
        }
       /* if (otherProj != null) 
        {
            Destroy(this.gameObject);
        }*/
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
