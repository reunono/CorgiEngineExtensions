using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ItemDrop : MonoBehaviour
 
{
    public GameObject[] itemDrops;
    public int dropSucessRate = 15;
    public Vector3 SpawnDestination;
 
    // Start is called before the first frame update
    void Start()
    {
        int randomChance = Random.Range(0, 100);
 
        if(randomChance < dropSucessRate)
        {
            int randomPick = Random.Range(0, itemDrops.Length);
            Instantiate(itemDrops[randomPick], transform.position + SpawnDestination, transform.rotation);
        } 
    } 
}