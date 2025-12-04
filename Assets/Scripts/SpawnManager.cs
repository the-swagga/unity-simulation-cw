using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform entitiesParent;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] enemies;

    void Start()
    {
        SpawnEntities();
    }

    void Update()
    {
        
    }

    private void SpawnEntities()
    {
        Instantiate(player, Vector3.zero, Quaternion.identity, entitiesParent);
    }
}
