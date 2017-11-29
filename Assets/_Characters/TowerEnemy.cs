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
    [SerializeField] float minimumTargetDistance = 10f;
    [SerializeField] float damagePerSecond = 50f;

    [SerializeField] GameObject laser;
    [SerializeField] float attackRadius = 30f;

    private float currentHealth;
    private float timeUntilNextShot;

    // Use this for initialization
    void Start () {
        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player");
        timeUntilNextShot = 0;
    }
	
	// Update is called once per frame
	void Update () {
        DecrementShootingCooldown();
        if (timeUntilNextShot <= 0 && TargetWithinAttackRadius() && TargetBeyondMinimumDistance()) { ShootProjectileAtTarget(target); }
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

    private bool TargetBeyondMinimumDistance()
    {
        return Vector3.Magnitude(target.transform.position - transform.position) >= minimumTargetDistance;
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

        // Create projectile laser marker
        // Constantly shoot raycasts from the start position to the marker against the player layer
        GameObject clone = Instantiate(laser, projectileSpawnPoint, Quaternion.FromToRotation(Vector3.up, projectileDirection), this.transform) as GameObject;
        clone.GetComponent<Laser>().SetStartingPosition(projectileSpawnPoint);
        clone.GetComponent<Laser>().SetLaserDirection(projectileDirection);
        clone.GetComponent<Laser>().SetDamagePerSecond(damagePerSecond);

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
