using System;
using System.Collections;
using UnityEngine;

public class PresentSpawner : MonoBehaviour
{
    [Header("Present Settings")]
    [SerializeField] private GameObject presentPrefab;
    [SerializeField] private Material[] wrappingPapers; //drag the different 
    [SerializeField] private Vector3 minSize = new Vector3(.2f, .25f, .2f);
    [SerializeField] private Vector3 maxSize = new Vector3(.5f, .5f, .5f);

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    //[SerializeField] private Vector3 spawnAreaSize = new Vector3(6f, 6f, 6f); //size of the area where the presents will spawn in.
    [SerializeField] private int numberOfPresents = 10;

    private GameObject[] presents;
    private Bounds bounds;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bounds = GetComponent<Collider>().bounds;
        StartCoroutine(SpawnAllPresents());
    }

    IEnumerator SpawnAllPresents()
    {
        presents = new GameObject[numberOfPresents];
        for (int i = 0; i < numberOfPresents; i++)
        {
            SpawnPresent(i);
            yield return new WaitForFixedUpdate();
        }
        spawnPoint.GetComponent<Collider>().enabled = false; // Disable the collider component because it can block raycasts. It may also ignore the physics layer specifically made for ignoring raycasts. Not sure why but some components ignore that so this is a fallback to if having it set to the layer doesn't work.
    }

    void SpawnPresent(int presentNumber)
    {
        //Random position within the spawn area
        Vector3 randomOffset = Utilities.GetRandomPointInBounds(spawnPoint.gameObject.GetComponent<Collider>().bounds);

        // bounds.center is relative to the world positiion
        Vector3 spawnPosition = bounds.center + randomOffset; // finds a random vector3 from the above in the spawn area to spawn the present
        
        //Randomize the size of the present
        float randomScaleX = UnityEngine.Random.Range(minSize.x, maxSize.x);
        float randomScaleY = UnityEngine.Random.Range(minSize.y, maxSize.y);
        float randomScaleZ = UnityEngine.Random.Range(minSize.z, maxSize.z);
        Vector3 randomScale = new Vector3(randomScaleX, randomScaleY, randomScaleZ);

        if (presentNumber != 0)
            StartCoroutine(SpawnCheck(spawnPosition, Utilities.Vec3Average(randomScale))); // NOTE: This is a local coroutine

        //Spawn the present
        GameObject present = Instantiate(presentPrefab, spawnPosition, Quaternion.identity);
        presents[presentNumber] = present;

        present.transform.localScale = randomScale;

        Weight presentWeight = present.GetComponent<Weight>();

        if (presentWeight != null)
        {
            // Multiply the weight value by the scale of the object (Average between the three axis, X+Y+Z/3)
            presentWeight.weight *= Utilities.Vec3Average(present.transform.localScale);
        }

        //Select a random Wrapping Paper to put on the present
        //Select a random Wrapping Paper to put on the present
        if (wrappingPapers.Length > 0)
        {
            Material randomMaterial = wrappingPapers[UnityEngine.Random.Range(0, wrappingPapers.Length)];
            //Debug.Log($"Selected material: {randomMaterial.name}");
            present.GetComponent<MeshRenderer>().material = randomMaterial;
            //Debug.Log($"Material applied: {present.GetComponent<MeshRenderer>().material.name}");
            present.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            present.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1,1) * UnityEngine.Random.Range(1f,4f);
        }

        /// <summary>
        /// Checks if the current position and scale of the present would spawn inside of an existing one and chooses a new spawn point.
        /// 
        /// Originally this was a Vector3 return method but running the while loop over and over again until it gets a spawn point value
        ///  slowed down the game to a hault and stopping the application from responding altogether.
        /// So I changed this to a Coroutine since it has wait methods, realized it can't really return a value,
        ///  then changed it to a local Coroutine within this method. -V
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="scale"></param>
        /// <returns>The new spawn point value</returns>
        IEnumerator SpawnCheck(Vector3 spawnPos, float scale)
        {
            if (presents[0] != null) // Is there even a present available in the first place
            {
                // local value to store the size and distance of the present being checked in proximity to this present
                float dist, size;
                bool goodToGo = false;
                int spawnedPresents; // Amount of elements to actually check in the array

                for (spawnedPresents = 0; spawnedPresents < presents.Length; spawnedPresents++)
                {
                    if (presents[spawnedPresents] == null) break; // No more presents beyond this point of the array
                }

                bool[] conflict = new bool[spawnedPresents];
                Predicate<bool> hasConflicts = n => n == false; // local property used for an Array check in the While loop, making sure that all boolean values are false
                                                                // Note: this property uses the System namespace, which conflicts with certain UnityEngine methods

                int iteration = 0; // Failsafe: All the presents were too big for any more to spawn next to each other, check if this has been running for too long.
                while (!goodToGo || iteration < 256)
                {
                    // Check for all presents currently in the scene
                    for (int i = 0; i < spawnedPresents; i++)
                    {
                        dist = Mathf.Abs((presents[i].transform.position - spawnPos).sqrMagnitude);
                        size = Utilities.Vec3Average(presents[i].transform.localScale);

                        if (dist < size + scale) // Is this object spawning near/inside the closest object?
                        {
                            //If the next object is spawning inside the last object that was spawning, 
                            while (dist < size)
                            {
                                spawnPos = Utilities.GetRandomPointInBounds(spawnPoint.gameObject.GetComponent<Collider>().bounds);
                                // Recalculate the dist after setting a new spawnPos
                                dist = Mathf.Abs((presents[i].transform.position - spawnPos).sqrMagnitude);

                                yield return new WaitForFixedUpdate();
                            }
                            conflict[i] = true;
                        }
                        else
                            conflict[i] = false;
                    }

                    // If there are no conflicts whatsoever or that the iteration has gone for too long, exit this while loop
                    if (Array.TrueForAll<bool>(conflict, hasConflicts))
                        goodToGo = true;
                    iteration++;

                    yield return new WaitForSeconds(0.01f);
                }
            }

            yield return spawnPosition = bounds.center + spawnPos;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
