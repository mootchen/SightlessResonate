Shader "Unlit/ShowDepth"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _SonarBaseColor("Base Color",  Color) = (0.1, 0.1, 0.1, 0)
        _SonarWaveColor("Wave Color",  Color) = (1.0, 0.1, 0.1, 1)
        _SonarWaveParams("Wave Params", Vector) = (1, 20, 20, 10)
        _SonarWaveVector("Wave Vector", Vector) = (0, 0, 1, 0)
        _SonarAddColor("Add Color",   Color) = (0, 0, 0, 0)
        _WaveTransparency("Wave Transparency", Range(0.0,1)) = 0.25
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float depth : DEPTH;
                    float4 vertex : SV_POSITION;
                    float3 worldPos : SOMETHING;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float4 _SonarBaseColor;
                float4 _SonarWaveColor;
                float4 _SonarWaveParams; // Amp, Exp, Interval, Speed
                float3 _SonarWaveVector;
                float4 _SonarAddColor;
                float _WaveTransparency;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    //o.vertex = UnityObjectToClipPos(v.vertex);
                    //o.depth = -UnityObjectToClipPos(v.vertex).z * _ProjectionParams.w;
                    o.depth = -UnityObjectToViewPos(v.vertex).z * _ProjectionParams.w;
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    //float w = length(i.worldPos - _SonarWaveVector);
                    float w = length(float3(0,0,0) - _SonarWaveVector);

                    // Moving wave.
                    w -= _Time.y * _SonarWaveParams.w;

                    // Get modulo (w % params.z / params.z)
                    w /= _SonarWaveParams.z;
                    w = w - floor(w);

                    // Make the gradient steeper.
                    float p = _SonarWaveParams.y;
                    w = (pow(w, p) + pow(1 - w, p * 4)) * 0.5;

                    // Amplify.
                    w *= _SonarWaveParams.x;

                    // Apply to the surface.
                    fixed4 col2 = (_SonarWaveColor * _WaveTransparency) * w + _SonarAddColor;

                    // sample the texture
                    float invert = 1 - i.depth;
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                    return fixed4(invert, invert, invert, 1) * col + col2;
                }
                ENDCG
            }
        }
}
