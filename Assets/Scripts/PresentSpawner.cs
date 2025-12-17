using UnityEngine;
using System.Collections;

public class PresentSpawner : MonoBehaviour
{
    [Header("Present Settings")]
    [SerializeField] private GameObject presentPrefab;
    [SerializeField] private Material[] wrappingPapers; //drag the different 
    [SerializeField] private Vector3 minSize = new Vector3(3f, 3f, 3f);
    [SerializeField] private Vector3 maxSize = new Vector3(5.5f, 5f, 5.5f);

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(6f, 6f, 6f); //size of the area where the presents will spawn in.
    [SerializeField] private int numberOfPresents = 10;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnAllPresents());
    }

    IEnumerator SpawnAllPresents()
    {
        for (int i = 0; i < numberOfPresents; i++)
        {
            SpawnPresent();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void SpawnPresent()
    {
        //Random position within the spawn area
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        Vector3 spawnPosition = spawnPoint.position + randomOffset; // finds a random vector3 from the above in the spawn area to spawn the present

        //Spawn the present
        GameObject present = Instantiate(presentPrefab, spawnPosition, Quaternion.identity);

        //Randomize the size of the present
        float randomScaleX = Random.Range(minSize.x, maxSize.x);
        float randomScaleY = Random.Range(minSize.y, maxSize.y);
        float randomScaleZ = Random.Range(minSize.z, maxSize.z);
        present.transform.localScale = new Vector3(randomScaleX, randomScaleY, randomScaleZ);

        //Select a random Wrapping Paper to put on the present
        if (wrappingPapers.Length > 0)
        {
            Material randomMaterial = wrappingPapers[Random.Range(0, wrappingPapers.Length)];
            present.GetComponent<MeshRenderer>().material = randomMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
