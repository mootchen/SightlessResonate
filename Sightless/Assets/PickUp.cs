using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    float throwForce = 6000;
    Vector3 objectPos;
    float distance;

    public bool canHold = true;
    public GameObject item;
    public GameObject tempParent;
    public bool isHolding = false;

    public SimpleSonarReplacementMain script;
    public GameObject player;

    public Color ringColor = new Color(1.0f, 0.2f, 0.2f, 0);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            if (script)
                script.StartSonarRing(player.transform.position, 15f, ringColor);
            StopCoroutine(DynamicFog());
            StartCoroutine(DynamicFog());
        }

        if(isHolding == true) {
            item.GetComponent<Rigidbody>().velocity = Vector3.zero;
            item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            item.transform.SetParent(tempParent.transform);

            if(Input.GetMouseButtonDown(1)) {
                //throw
                item.GetComponent<Rigidbody>().AddForce(tempParent.transform.forward * throwForce);
                isHolding = false;
            }
        }
        else {
            objectPos = item.transform.position;
            item.transform.SetParent(null);
            item.GetComponent<Rigidbody>().useGravity = true;
            item.transform.position = objectPos;
        }
    }

    IEnumerator DynamicFog()
    {
        float currentFogEndDistance = RenderSettings.fogEndDistance;
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        for (int i = (int)currentFogEndDistance; i <= 30; i++)
        {
            RenderSettings.fogEndDistance = RenderSettings.fogStartDistance + i;
            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 30; i > (int)currentFogEndDistance; i--)
        {
            RenderSettings.fogEndDistance = i;
            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(0.1f);
        }

        RenderSettings.fogEndDistance = currentFogEndDistance;


        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    void OnMouseDown() {
        isHolding = true;
        item.GetComponent<Rigidbody>().useGravity = false;
        item.GetComponent<Rigidbody>().detectCollisions = true;
    }

    void OnMouseUp() {
        isHolding = false;
    }
}
