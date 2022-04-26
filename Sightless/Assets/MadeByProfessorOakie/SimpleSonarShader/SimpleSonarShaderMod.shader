// SimpleSonarShader scripts and shaders were written by Drew Okenfuss.
// For this shader to work, the object must have values passed in to it from the SimpleSonarShader_Parent.cs script.
// By default, this happens by having the object be a child of SimpleSonarShader_Parent.
Shader "MadeByProfessorOakie/SimpleSonarShaderMod" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_Darkness("Darkness", float) = 1
		_MainTex("Albedo", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_RingColor("Ring Color", Color) = (1,1,1,1)
		_RingColorIntensity("Ring Color Intensity", float) = 5
		_RingSpeed("Ring Speed", float) = 1
		_RingWidth("Ring Width", float) = 0.1
		_RingGradiant("Ring Gradiant", float) = 1
		_RingIntensityScale("Ring Range", float) = 1
	}
	SubShader {
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		// The size of these arrays is the number of rings that can be rendered at once.
		// If you want to change this, you must also change QueueSize in SimpleSonarShader_Parent.cs
		half4 _hitPts[20];
		half _StartTime;
		half _Intensity[20];

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _RingColor;
		half _RingColorIntensity;
		half _RingSpeed;
		half _RingWidth;
		half _RingIntensityScale;
		float _RingGradiant;
		float _Darkness;


		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = (_Color * tex2D(_MainTex, IN.uv_MainTex)) * 0;
			o.Albedo = c.rgb;

			half DiffFromRingCol = abs(o.Albedo.r - _RingColor.r) + abs(o.Albedo.b - _RingColor.b) + abs(o.Albedo.g - _RingColor.g);

			// Check every point in the array
			// The goal is to set RGB to highest possible values based on current sonar rings
			for (int i = 0; i < 20; i++) {

				half d = distance(_hitPts[i], IN.worldPos);
				half intensity = _Intensity[i] * _RingIntensityScale;
				half val = (1 - (d / intensity)); // Distance of wave outward.

				if (d < (_Time.y - _hitPts[i].w) * _RingSpeed && d >(_Time.y - _hitPts[i].w) * _RingSpeed - _RingWidth && val > 0) {
					half posInRing = (d - ((_Time.y - _hitPts[i].w) * _RingSpeed - _RingWidth)) / _RingWidth;

					float f = length(posInRing);
					val *= f * (pow(f, _RingGradiant) + pow(1 - f, _RingGradiant * 4)) * 0.5;
					half3 tmp = _RingColor * val * _RingColorIntensity;
					tmp += c;

					// Determine if predicted values will be closer to the Ring color
					half tempDiffFromRingCol = abs(tmp.r - _RingColor.r) + abs(tmp.b - _RingColor.b) + abs(tmp.g - _RingColor.g);
					//half tempDiffFromRingCol = abs(tmp.rgb - _RingColor.rgb);
					if (tempDiffFromRingCol < DiffFromRingCol)
					{
						// Update values using our predicted ones.
						DiffFromRingCol = tempDiffFromRingCol;
						o.Albedo.r = tmp.r;
						o.Albedo.g = tmp.g;
						o.Albedo.b = tmp.b;
						o.Emission.r *= tmp.r;
						o.Emission.g *= tmp.g;
						o.Emission.b *= tmp.b;
						o.Albedo.rgb *= _RingColorIntensity;
					}
				}
			}

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
