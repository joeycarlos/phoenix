using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour {

    [SerializeField] float spawnRadius;
    [SerializeField] GameObject[] powerUpTypes;

    [SerializeField] float minimumSpawnTime = 5f;
    [SerializeField] float maximumSpawnTime = 10f;

    [SerializeField] float healthPercentage = 0.5f;
    [SerializeField] float attackPercentage = 0.2f;
    [SerializeField] float movementPercentage = 0.3f;

    private GameObject previousClone;

    private float timeUntilNextSpawn;
    private int nextPowerUpNumber;
    private float[] probabilities;

    // Use this for initialization
    void Start () {
        timeUntilNextSpawn = RenewSpawnTimer();
        probabilities = new float[3] { healthPercentage, attackPercentage, movementPercentage };
    }
	
	// Update is called once per frame
	void Update () {
        DecreaseSpawnCooldown();
        if (timeUntilNextSpawn <= 0)
            ExecuteSpawn();
    }

    private void ExecuteSpawn()
    {
        nextPowerUpNumber = (int)Choose(probabilities);
        previousClone = Instantiate(powerUpTypes[nextPowerUpNumber], RandomLocationInRadius(), Quaternion.identity) as GameObject;

        timeUntilNextSpawn = RenewSpawnTimer();

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

    private float RenewSpawnTimer()
    {
        float result;
        result = Random.Range(minimumSpawnTime, maximumSpawnTime);
        return result;
    }

    private void DecreaseSpawnCooldown()
    {
        timeUntilNextSpawn = timeUntilNextSpawn - Time.deltaTime;
    }

    private Vector3 RandomLocationInRadius()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;
        return randomDirection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
