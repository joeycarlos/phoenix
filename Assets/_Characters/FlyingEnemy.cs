using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour, IDamageable {

    [SerializeField] float maxHealth = 100f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] GameObject target;

    [SerializeField] float projectileVerticalOffset = 1.7f;
    [SerializeField] float projectileHorizontalOffset = 1.5f;
    [SerializeField] float initialProjectileForce = 0.1f;
    [SerializeField] float timeBetweenShots = 0.5f;
    [SerializeField] float damagePerShot = 20f;
    [SerializeField] GameObject projectile;
    [SerializeField] float attackRadius = 30f;
    [SerializeField] float chaseRadius = 30f;
    [SerializeField] float stoppingDistance = 10f;

    private Rigidbody rb;
    private float currentHealth;
    private Vector3 moveVector;
    private float timeUntilNextShot;

    void Start () {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player");
    }
	
	void Update () {
        DecrementShootingCooldown();
        if (timeUntilNextShot <= 0 && TargetWithinAttackRadius()) { ShootProjectileAtTarget(target); }

        LookAtTarget();
        if (CheckPathToTarget() && TargetWithinChaseRadius() && TargetBeyondStoppingDistance()) ChaseTarget();
    }

    private bool CheckPathToTarget()
    {
        Vector3 targetDirection = target.transform.position - this.transform.position;

        int layerMask;
        RaycastHit hit;
        float distanceToPlayer = Mathf.Infinity;
        float distanceToObstacle = Mathf.Infinity;

        // raycast against player
        layerMask = 1 << 8;
        if (Physics.Raycast(this.transform.position, targetDirection, out hit, 50f, layerMask))
            distanceToPlayer = hit.distance;
            
        // raycast against default obstacles
        layerMask = 1 << 0;
        if (Physics.Raycast(this.transform.position, targetDirection, out hit, 50f, layerMask))
            distanceToObstacle = hit.distance;

        if (distanceToObstacle > distanceToPlayer)
        {
            return true;
        }
        else return false;

            // compare distances
            // if player is closer, then return true
            // if obstacle is closer, then return false
            // if neither, return false
    }

    private void LookAtTarget()
    {
        transform.LookAt(target.transform);
    }

    private void ChaseTarget()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private bool TargetWithinAttackRadius()
    {
        return Vector3.Magnitude(target.transform.position - transform.position) <= attackRadius;
    }

    private bool TargetWithinChaseRadius()
    {
        return Vector3.Magnitude(target.transform.position - transform.position) <= chaseRadius;
    }

    private bool TargetBeyondStoppingDistance()
    {
        return Vector3.Magnitude(target.transform.position - transform.position) >= stoppingDistance;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        if (currentHealth <= 0) { Destroy(gameObject); }
    }

    public float GetCurrentHealthAsPercentage()
    {
        return currentHealth / maxHealth;
    }

    private void ShootProjectileAtTarget(GameObject currentTarget)
    {
        // Define projectile spawn point relative to character
        Vector3 projectileSpawnPoint = transform.position + (transform.forward.normalized * projectileHorizontalOffset) + transform.up * projectileVerticalOffset;

        // Create projectile
        GameObject clone = Instantiate(projectile, projectileSpawnPoint, Quaternion.FromToRotation(Vector3.up, transform.forward)) as GameObject;
        clone.GetComponent<Projectile>().SetDamage(damagePerShot);
        clone.GetComponent<Projectile>().SetShooter(gameObject);

        // Shoot projectile
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * initialProjectileForce, ForceMode.Impulse);

        // Reset shot recharge countdown
        timeUntilNextShot = timeBetweenShots;
    }

    private void DecrementShootingCooldown()
    {
        timeUntilNextShot -= Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}
