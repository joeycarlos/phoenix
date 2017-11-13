using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField] float projectileLifeTime = 3f;

    const float DESTROY_DELAY = 0.01f;

    private GameObject shooter;
    private float damageCaused;

    // Use this for initialization
    void Start () {
        Destroy(gameObject, projectileLifeTime);
	}
	
    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    public void SetDamage(float damage)
    {
        damageCaused = damage;
    }

    void OnCollisionEnter(Collision collision)
    {
        var layerCollidedWith = collision.gameObject.layer;
        if (shooter && layerCollidedWith != shooter.layer)
        {
            DamageIfDamageable(collision);
        }

    }

    private void DamageIfDamageable(Collision collision)
    {
        Component damageableComponent = collision.gameObject.GetComponent(typeof(IDamageable));

        if (damageableComponent)
        {
            (damageableComponent as IDamageable).TakeDamage(damageCaused);
        }
        Destroy(gameObject, DESTROY_DELAY);
    }
}
