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

    public float viewRadius = 0;

    public float ringSpeed = 4f;

    public LayerMask colliderMask;

    public Material replacementMaterial;

    private Vector3 drawPosition;

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        //Use the same vars you use to draw your Overlap Sphere to draw your Wire Sphere.
        Gizmos.DrawWireSphere (drawPosition, viewRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            if (script) {
                drawPosition = player.transform.position;
                script.StartSonarRing(drawPosition, 15f, ringColor);
                StartCoroutine(HintTool(drawPosition));
            }
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

    private Dictionary<Collider, Material> colliderDict = new Dictionary<Collider, Material>();

    IEnumerator HintTool(Vector3 position)
    {
        float hitTime = Time.timeSinceLevelLoad;
        while(viewRadius <= 15f)
        {
            viewRadius = Mathf.Max((Time.timeSinceLevelLoad - hitTime) * (ringSpeed+1), 0);
            Collider[] sphereRange = Physics.OverlapSphere(position, viewRadius);
            foreach (Collider collider in sphereRange)
            {
                bool active = false;
                Color col = Color.white;
                switch (collider.tag)
                {
                    case "Actable":
                        col = Color.cyan;
                        active = true;
                        break;
                    case "Viewable":
                        col = Color.white;
                        active = true;
                        break;
                    case "Danger":
                        col = Color.red;
                        active = true;
                        NewBehaviourScript script = collider.GetComponent<NewBehaviourScript>();
                        if(script != null) {
                            script.LookingPlayer(position);
                        }
                        break;
                    default:
                        break;
                }
                if(active) {
                    if (!colliderDict.ContainsKey(collider)) {
                        colliderDict.Add(collider, collider.GetComponent<Renderer>().material);
                        Material oldMat = collider.GetComponent<Renderer>().material;
                        collider.GetComponent<Renderer>().material = replacementMaterial;
                        collider.GetComponent<Renderer>().material.SetColor("_FresnelColor", col);
                        collider.gameObject.layer = 9;
                        StartCoroutine(FadeMat(collider.GetComponent<Renderer>().material));
                    }
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
        viewRadius = 0;
        yield return new WaitForSeconds(2f);
        foreach(KeyValuePair<Collider, Material> entry in colliderDict)
        {
            entry.Key.GetComponent<Renderer>().material = entry.Value;
            entry.Key.gameObject.layer = 0;
        }
        colliderDict.Clear();
    }

    IEnumerator FadeMat(Material mat) {
        Color fresnelColor = mat.GetColor("_FresnelColor");
        yield return new WaitForSeconds(2f);
        while(Mathf.Abs(Color.black.r-fresnelColor.r)+Mathf.Abs(Color.black.g-fresnelColor.g)+Mathf.Abs(Color.black.b-fresnelColor.b) > 0.1f)
        {
            fresnelColor = Color.Lerp(fresnelColor, Color.black, 0.1f);
            mat.SetColor("_FresnelColor", fresnelColor);
            yield return new WaitForSeconds(0.1f);
        }
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
