using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSonarEffector : MonoBehaviour
{
    public SimpleSonarReplacementMain script;

    public Color ringColor = new Color(1.0f, 0.2f, 0.2f, 0);

    public float viewRadius = 0;

    public Material replacementMaterial;

    public float ringSpeed = 4f;

    Vector3 drawPosition;

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (drawPosition, viewRadius);
    }

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
        if (script) {
            drawPosition = collision.contacts[0].point;
            float mag = collision.impulse.magnitude / 8.0f;
            script.StartSonarRing(drawPosition, mag, ringColor);
            StartCoroutine(HintTool(drawPosition, mag));
        }
    }

    private Dictionary<Collider, Material> colliderDict = new Dictionary<Collider, Material>();

    IEnumerator HintTool(Vector3 position, float magnitude)
    {
        float hitTime = Time.timeSinceLevelLoad;
        while(viewRadius <= magnitude)
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
}
