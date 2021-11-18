using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public GameObject BoidPrefab;
    public int NumberToSpawnAtStart;
    public int SpawnRadius;
    public Flock MyFlock;
    // Start is called before the first frame update
    void Start()
    {
        SpawnBoids(NumberToSpawnAtStart);
    }

    public void SpawnBoids(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector3 pos = transform.position + UnityEngine.Random.insideUnitSphere * SpawnRadius;
            Quaternion rot = UnityEngine.Random.rotation;
            GameObject boid = Instantiate(BoidPrefab, pos, rot);
            MyFlock.AddBoid(boid.GetComponent<Boid>());
        }
    }
}
