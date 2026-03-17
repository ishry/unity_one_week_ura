using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnItem(GameObject prefabToSpawn)
    {
        Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
    }
}
