Shader "Custom/TiledMeshShader"
{
	Properties 
	{
		_Color ("Color", Color) = ( 1.0, 1.0, 1.0, 1.0 )
	}
	SubShader 
	{
		LOD 100
		Tags { "RenderType"="Opaque" }

		Pass
		{	
			Tags { "LIGHTMODE"="Vertex" "RenderType"="Opaque" }

			Color ( 1.0, 1.0, 1.0, 1.0 ) Lighting Off
		}
	} 
}
