using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReplacementShaderEffect : MonoBehaviour
{

    public Shader ReplacementShader;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (ReplacementShader != null)
            GetComponent<Camera>().SetReplacementShader(ReplacementShader, "RenderType");
    }

    // Update is called once per frame
    void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }
}
