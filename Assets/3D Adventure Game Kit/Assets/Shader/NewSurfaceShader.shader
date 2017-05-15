// simple "dissolving" shader by genericuser (radware.wordpress.com)
// clips materials, using an image as guidance.
// use clouds or random noise as the slice guide for best results.
Shader "Custom Shaders/Dissolving" {
	Properties{
		_MainTex("Texture (RGB)", 2D) = "white" {}
		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5
		_Color("Tint", Color) = (1.0, 0.6, 0.6, 1.0)

	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Lighting Off
		Cull Off
		CGPROGRAM
		//if you're not planning on using shadows, remove "addshadow" for better performance
#pragma surface surf Lambert addshadow
	struct Input {
		float2 uv_MainTex;
		float2 uv_SliceGuide;
		float _SliceAmount;
		float4 _Color;
	};
	sampler2D _MainTex;
	sampler2D _SliceGuide;
	float _SliceAmount;
	float4 _Color;
	void surf(Input IN, inout SurfaceOutput o) {
		clip(tex2D(_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount);
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;

	}
	ENDCG
	}
		Fallback "Diffuse"
}