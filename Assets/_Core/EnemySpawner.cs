using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] float spawnRadius;

    [SerializeField] GameObject[] enemies;

    [SerializeField] float groundedPercentage = 0.5f;
    [SerializeField] float aerialPercentage = 0.2f;
    [SerializeField] float towerPercentage = 0.3f;

    [SerializeField] float minimumSpawnTime = 5f;
    [SerializeField] float maximumSpawnTime = 10f;

    private float timeUntilNextSpawn;
    private GameObject nextEnemy;

    private int nextEnemyNumber;

    private Vector3 spawnPosition;

    private float[] probabilities;

    private GameObject previousClone;

	// Use this for initialization
	void Start () {
        timeUntilNextSpawn = 0f;
        probabilities = new float[3] { groundedPercentage, aerialPercentage, towerPercentage };
	}
	
	// Update is called once per frame
	void Update () {
        DecreaseSpawnCooldown();
        if (timeUntilNextSpawn <= 0)
            ExecuteSpawn();
	}

    private void ExecuteSpawn()
    {
        nextEnemyNumber = (int)Choose(probabilities);
        previousClone = Instantiate(enemies[nextEnemyNumber], RandomNavmeshLocation(spawnRadius), Quaternion.identity) as GameObject;

        // if grounded enemy, ensure it is on the mesh
        if (previousClone.GetComponent<NavMeshAgent>() != null)
        {
            previousClone.GetComponent<NavMeshAgent>().Warp(RandomNavmeshLocation(spawnRadius));
        }
        
        timeUntilNextSpawn = RenewSpawnTimer();

    }

    private float RenewSpawnTimer()
    {
        float result;
        result = Random.Range(minimumSpawnTime, maximumSpawnTime);
        return result;
    }

    float Choose(float[] probs)
    {

        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    private void DecreaseSpawnCooldown()
    {
        timeUntilNextSpawn = timeUntilNextSpawn - Time.deltaTime;
    }

    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        spawnPosition = finalPosition;
        
        return finalPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
