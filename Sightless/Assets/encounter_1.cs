using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class encounter_1 : MonoBehaviour
{
    public GameObject Player;
    public GameObject spawnRock;
    public GameObject Monster;
    public bool encounterTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log(other);
        if(other.gameObject == Player){
            //encounterTriggered = True;
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject sphere = Instantiate(spawnRock, transform.position, Quaternion.identity);

            //create monster on other side of bridge
            //GameObject enemy = Instantiate(Monster, new Vector3(8, 6, -174), Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
