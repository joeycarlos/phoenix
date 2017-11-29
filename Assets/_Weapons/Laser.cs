using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    [SerializeField] float laserLifeTime = 10f;
    [SerializeField] float laserIncreaseSpeed = 1f;
    [SerializeField] GameObject target;

    private Vector3 startingPosition;
    private Vector3 laserDirection;

    private bool hitStaticObject;

    private float damagePerSecond;

    LineRenderer lineRenderer;

    float laserEndMarker;

    // Use this for initialization
    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        Destroy(gameObject, laserLifeTime);
        laserEndMarker = 0;
        hitStaticObject = false;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update() {
        if (CheckLaserCollision()) SendDamage();
        SetupLine();
        if (!hitStaticObject) UpdateLaserEndMarker();
    }

    private void SetupLine()
    {
        lineRenderer.sortingLayerName = "OnTop";
        lineRenderer.sortingOrder = 5;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startingPosition);
        lineRenderer.SetPosition(1, startingPosition + laserDirection * laserEndMarker);
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = true;
    }

    private bool CheckLaserCollision()
    {
        int layerMask = 1 << 0;
        RaycastHit hit;

        if (Physics.Raycast(startingPosition, laserDirection, out hit, laserEndMarker, layerMask))
            hitStaticObject = true;
            
        layerMask = 1 << 8;

        if (Physics.Raycast(startingPosition, laserDirection, out hit, laserEndMarker, layerMask))
        {
            return true;
        }
            
        else return false;
    }

    public void SetStartingPosition(Vector3 vector)
    {
        startingPosition = vector;
    }

    public void SetLaserDirection(Vector3 vector)
    {
        laserDirection = vector;
    }

    public void SetDamagePerSecond(float damage)
    {
        damagePerSecond = damage;
    }

    public void UpdateLaserEndMarker()
    {
        laserEndMarker += laserIncreaseSpeed * Time.deltaTime;
    }

    private void SendDamage()
    {
        target.GetComponent<Player>().TakeDamage(damagePerSecond * Time.deltaTime);
    }

}
