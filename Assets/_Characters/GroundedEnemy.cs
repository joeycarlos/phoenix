using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemy : MonoBehaviour, IDamageable {

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

    private Rigidbody rb;
    private float currentHealth;
    private Vector3 moveVector;
    private float timeUntilNextShot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player"); ;
    }

    void Update()
    {
        DecrementShootingCooldown();
        if (timeUntilNextShot <= 0 && TargetWithinRadius()) { ShootProjectileAtTarget(target); }
    }

    void LateUpdate()
    {
        ChaseTarget();
    }

    private void ChaseTarget()
    {
        Vector3 lookAtGoal = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);

        // create a unit vector to check if path ahead is grounded
        Vector3 unitVectorToGoal = new Vector3(target.transform.position.x - this.transform.position.x, 0, target.transform.position.z - this.transform.position.z);
        unitVectorToGoal = Vector3.Normalize(unitVectorToGoal);

        if (IsGrounded(unitVectorToGoal*2 + this.transform.position))
        {
            transform.LookAt(lookAtGoal);
            this.transform.Translate(0, 0, moveSpeed * Time.deltaTime);
        } 

    }

    private bool TargetWithinRadius()
    {
        return Vector3.Magnitude(target.transform.position - transform.position) <= attackRadius;
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

    private bool IsGrounded(Vector3 targetMovement)
    {
        float radius = 0.95f;
        int layerMask = 1 << 0;

        if (Physics.CheckSphere(targetMovement, radius, layerMask)) { print("true!");  return true; }
        else return false;
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5F);
        Vector3 unitVectorToGoal = new Vector3(target.transform.position.x - this.transform.position.x, 0, target.transform.position.z - this.transform.position.z);
        unitVectorToGoal = Vector3.Normalize(unitVectorToGoal);
        Gizmos.DrawSphere(unitVectorToGoal*2 + this.transform.position, 0.95f);
    }
    */
}
