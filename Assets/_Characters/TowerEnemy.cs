using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEnemy : MonoBehaviour, IDamageable {

    [SerializeField] float maxHealth = 100f;
    [SerializeField] GameObject target;

    [SerializeField] float projectileVerticalOffset = 1.7f;
    [SerializeField] float projectileHorizontalOffset = 1.5f;
    [SerializeField] float initialProjectileForce = 0.1f;
    [SerializeField] float timeBetweenShots = 0.5f;
    [SerializeField] float damagePerShot = 20f;

    [SerializeField] GameObject projectile;
    [SerializeField] float attackRadius = 30f;

    private float currentHealth;
    private float timeUntilNextShot;

    // Use this for initialization
    void Start () {
        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
        DecrementShootingCooldown();
        if (timeUntilNextShot <= 0 && TargetWithinAttackRadius()) { ShootProjectileAtTarget(target); }
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        if (currentHealth <= 0) { Destroy(gameObject); }
    }

    private bool TargetWithinAttackRadius()
    {
        return Vector3.Magnitude(target.transform.position - transform.position) <= attackRadius;
    }

    private void ShootProjectileAtTarget(GameObject currentTarget)
    {

        // Define projectile spawn point relative to character
        Vector3 projectileSpawnPoint = 
            transform.position 
            + Vector3.Scale(target.transform.position - this.transform.position, new Vector3(1, 0, 1)).normalized * projectileHorizontalOffset 
            + transform.up * projectileVerticalOffset;

        // Define projectile shoot direction
        Vector3 projectileDirection = Vector3.Normalize(target.transform.position - projectileSpawnPoint);

        // Create projectile
        GameObject clone = Instantiate(projectile, projectileSpawnPoint, Quaternion.FromToRotation(Vector3.up, projectileDirection)) as GameObject;
        clone.GetComponent<Projectile>().SetDamage(damagePerShot);
        clone.GetComponent<Projectile>().SetShooter(gameObject);

        // Shoot projectile
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        rb.AddForce(projectileDirection * initialProjectileForce, ForceMode.Impulse);

        // Reset shot recharge countdown
        timeUntilNextShot = timeBetweenShots;
    }

    private void DecrementShootingCooldown()
    {
        timeUntilNextShot -= Time.deltaTime;
    }

    public float GetCurrentHealthAsPercentage()
    {
        return currentHealth / maxHealth;
    }
}
