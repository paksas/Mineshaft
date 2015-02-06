Shader "Custom/ShawlShader" 
{
	Properties 
	{
		_Color ("Color", Color) = ( 1.0, 1.0, 1.0, 1.0 )
	}
	SubShader 
	{
		Lighting Off
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf SimpleUnlit
  
			half4 LightingSimpleUnlit (SurfaceOutput s, half3 lightDir, half atten) 
			{
				half4 c;
				c.rgb = s.Albedo;
				c.a = s.Alpha;
				return c;
			}

		struct Input 
		{
			float4 color : COLOR;
		};

		float4 _Color;

		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
