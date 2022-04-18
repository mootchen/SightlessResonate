using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSonarEffector : MonoBehaviour
{
    public SimpleSonarReplacementMain script;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Start sonar ring from the contact point
        if (script) script.StartSonarRing(collision.contacts[0].point, collision.impulse.magnitude / 10.0f);
    }
}