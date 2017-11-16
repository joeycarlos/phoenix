﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour, IDamageable {


    // ----- EDITABLE VARIABLES -----

    // GENERAL MOVEMENT VARIABLES
    [SerializeField] float horizontalMoveSpeed = 10f;
    [SerializeField] float verticalMoveSpeed = 10f;
    [SerializeField] float backwardsMovementDivisor = 2f;

    // JUMP VARIABLES
    [SerializeField] float minJumpForce = 4f;
    [SerializeField] float maxJumpForce = 8f;
    [SerializeField] float jumpChargeRate = 6f;
    [SerializeField] float chargeJumpSpeedDivisor = 3f;
    [SerializeField] float chargeJumpEnergyCost = 3f;

    // GLIDING VARIABLES
    [SerializeField] float glideDrag = 1000f;
    [SerializeField] float glideEnergyCost = 20f;

    // DODGE VARIABLES
    [SerializeField] float dodgeStateTime = 1f;
    [SerializeField] float dodgeSpeedMultiplier = 4f;
    [SerializeField] float dodgeEnergyCost = 30f;

    // PROJECTILE ATTACK VARIABLES
    [SerializeField] float projectileVerticalOffset = 1.7f;
    [SerializeField] float projectileHorizontalOffset = 1.5f;
    [SerializeField] float initialProjectileForce = 30f;
    [SerializeField] float timeBetweenShots = 0.5f;
    [SerializeField] float damagePerShot = 20f;
    [SerializeField] GameObject projectile;

    // RESOURCE VARIABLES
    [SerializeField] float maxEnergy = 100f;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float energyRegenerationPerSecond = 3f;
    [SerializeField] float healthRegenerationPerSecond = 5f;
    [SerializeField] float lowEnergyPercentageThreshold = 0.01f;

    // ----- PRIVATE VARIABLES -----

    // EXTERNAL REFERENCES
    public Animator animator;
    private Rigidbody rb;
    private Transform cameraTransform;
    private CapsuleCollider capsuleCollider;

    // INPUT AXIS
    private float horizontalInput;
    private float verticalInput;

    // STATE BOOL
    private bool inChargeJumpState;
    private bool inDodgeState;
    private bool isGrounded;

    // STATE FLOAT
    private float timeUntilNextShot;
    private float currentJumpForce;
    private float currentEnergy;
    private float currentHealth;

    // STATE VECTOR
    private Vector3 moveVector;

    // ----- PRIVATE VARIABLES -----

    // EXTERNAL REFERENCES
    private const float GRAVITY_FORCE = 9.81f;

    // ----- MAIN FLOW ----- 

    void Start () {

        // Link external components
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Initialize state variables
        inDodgeState = false;
        inChargeJumpState = false;
        isGrounded = false;
        currentEnergy = maxEnergy;
        currentHealth = maxHealth;

        currentJumpForce = minJumpForce;
        timeUntilNextShot = 0;
        
    }

    private void Update()
    {
        UpdateGrounded();
        DecrementShootingCooldown();
        UpdateResources();

        // Read input
        if (Input.GetKey(KeyCode.Space) && isGrounded && !inDodgeState && GetCurrentEnergyAsPercentage() > lowEnergyPercentageThreshold)    { ChargeJump(); }
        if (Input.GetKeyUp(KeyCode.Space) && isGrounded)                                                                                    { ExecuteJump(); }
        if (Input.GetKey(KeyCode.Mouse0) && timeUntilNextShot <= 0 && !inChargeJumpState && !inDodgeState)                                  { ShootProjectile(); }
        if (Input.GetKeyDown(KeyCode.LeftShift) && !inChargeJumpState && currentEnergy >= dodgeEnergyCost)                                  { EnterDodgeState(); }
        ReadMovementInput();

        // Adjust movement
        if (GetCurrentEnergyAsPercentage() > lowEnergyPercentageThreshold) UpdateAerialDrag();
        ReduceGlidingGravity();
        CalculateMoveVector();

        // Move and rotate the player
        MovePlayer();
        RotatePlayer();

        // Update animator
        UpdateAnimator();
    }


    // ----- MOVE HELPER FUNCTIONS -----

    private void ReadMovementInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void CalculateMoveVector()
    {
        
        float maxVectorMagnitude = verticalMoveSpeed;

        // Calculate horizontal move vector component
        float horizontalMagnitude = horizontalInput * Time.deltaTime * horizontalMoveSpeed;
        Vector3 playerRightDirection = Vector3.Scale(transform.right, new Vector3(1, 0, 1)).normalized;

        // Calculate vertical move vector component
        float verticalMagnitude = verticalInput * Time.deltaTime * verticalMoveSpeed;
        Vector3 playerForwardDirection = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        // Calculate resultant vector
        moveVector = verticalMagnitude * playerForwardDirection + horizontalMagnitude * playerRightDirection;
        moveVector = Vector3.ClampMagnitude(moveVector, maxVectorMagnitude);                                     // Ensure diagonal speed is capped at forward speed

        // Adjust move vector for special cases
        if (verticalInput < 0 && !inDodgeState) moveVector = moveVector / backwardsMovementDivisor;             // Reduce backwards movement
        if (inDodgeState) { moveVector = moveVector * dodgeSpeedMultiplier; }                                   // Increase movement speed if dodging
        if (inChargeJumpState) { moveVector = moveVector / chargeJumpSpeedDivisor; }                            // Reduce movement speed if charging a jump
       
    }

    // Increase drag if descending and not dodging -- to emulate gliding
    private void UpdateAerialDrag()
    {
        
        rb.drag = 0;

        if (!isGrounded && rb.velocity.y < 0 && !inDodgeState)
        {

            if (verticalInput == 1 || verticalInput == -1 || horizontalInput == 1 || horizontalInput == -1)
            {                                    
                rb.drag = glideDrag;
                currentEnergy = Mathf.Clamp(currentEnergy - (glideEnergyCost * Time.deltaTime), 0, maxEnergy);
            }
        }
        
    }

    private void ReduceGlidingGravity()
    {
        if (!isGrounded && inDodgeState)
        {
            rb.AddForce(Vector3.up * GRAVITY_FORCE, ForceMode.Force);
        }
    }

    private void MovePlayer()
    {
        transform.Translate(moveVector, Space.World);   // Move the transform relative to worldspace
    }

    private void RotatePlayer()
    {
        // rotate player to match camera free look rotation
        Quaternion cameraRotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);
        transform.rotation = cameraRotation;
    }


    // ----- DODGE HELPER FUNCTIONS -----

    private void EnterDodgeState()
    {
        inDodgeState = true;
        currentEnergy = currentEnergy - dodgeEnergyCost;
        Invoke("ExitDodgeState", dodgeStateTime);
    }

    private void ExitDodgeState()
    {
        inDodgeState = false;
    }


    // ----- JUMP HELPER FUNCTIONS -----

    private void EnterChargeJumpState()
    {
        inChargeJumpState = true;
    }

    private void ExitChargeJumpState()
    {
        inChargeJumpState = false;
    }

    private void ChargeJump()
    {
        if (inChargeJumpState == false) EnterChargeJumpState();
        currentJumpForce = Mathf.Clamp(currentJumpForce + Time.deltaTime * jumpChargeRate, minJumpForce, maxJumpForce);
        currentEnergy = Mathf.Clamp( currentEnergy - (chargeJumpEnergyCost * Time.deltaTime), 0, maxEnergy);
    }

    private void ExecuteJump()
    {
        currentJumpForce = Mathf.Clamp(currentJumpForce, minJumpForce, maxJumpForce);   // ensure jump force is within bounds
        rb.AddForce(0, currentJumpForce, 0, ForceMode.Impulse);                         // execute jump
        currentJumpForce = minJumpForce;                                                // reset stored jump force
        ExitChargeJumpState();
    }

    public float GetCurrentJumpChargeAsPercentage()
    {
        return (currentJumpForce - minJumpForce) / (maxJumpForce - minJumpForce);
    }


    // ----- PROJECTILE ATTACK HELPER FUNCTIONS -----

    private void ShootProjectile()
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

    // ----- RESOURCE HELPER FUNCTIONS -----

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public float GetCurrentHealthAsPercentage()
    {
        return currentHealth / maxHealth;
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public float GetCurrentEnergyAsPercentage()
    {
        return currentEnergy / maxEnergy;
    }

    private void UpdateResources()
    {
        currentHealth += healthRegenerationPerSecond * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        currentEnergy += energyRegenerationPerSecond * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
    }

    // ----- ANIMATOR HELPER FUNCTIONS -----
    private void UpdateAnimator()
    {
        animator.SetBool("inDodgeState", inDodgeState);
        animator.SetBool("inChargeJumpState", inChargeJumpState);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("horizontalInput", horizontalInput);
        animator.SetFloat("verticalInput", verticalInput);
    }

    // ----- GENERAL HELPER FUNCTIONS -----

    private void UpdateGrounded()
    {
        float radius = capsuleCollider.radius * 0.95f;
        // Check against default layer
        int layerMask = 1 << 0;

        if (Physics.CheckSphere(transform.position + Vector3.up*(radius - 0.2f), radius, layerMask)) { isGrounded = true; }
        else { isGrounded = false; }

        return;
    }

    private void OnDrawGizmosSelected()
    {
        float radius = capsuleCollider.radius * 0.95f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up*(radius - 0.2f), radius);
    }

}