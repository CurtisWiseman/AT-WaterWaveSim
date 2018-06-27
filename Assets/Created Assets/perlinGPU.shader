Shader "Custom/PerlinGPU"
{
	Properties
	{
		_Color ("Color", Color) = (1,0,0,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 1.0
		_Length ("Wave Length", Float) = 0.5
		_Height ("Wave Height", Float) = 2.5
		_Speed ("Wave Speed", Float) = 0.25
	}

    CGINCLUDE

    #include "UnityCG.cginc"
	#include "ClassicNoise3D.hlsl"

    ENDCG

    SubShader
    {
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

			float _Length;
			float _Speed;
			float _Height;
			float _scriptTime;

			v2f_img vert(appdata_base v)
			{
				v2f_img i;
				i.pos = UnityObjectToClipPos(v.vertex);
				i.uv = v.texcoord.xy;
        
				float2 uv = i.uv * 4.0 + float2(0.2, 1) * _scriptTime;
				
				float y_val = 0.5;
		
				float3 coord = float3(uv * _Length, _scriptTime * _Speed);
				y_val += cnoise(coord) * _Height;

				i.pos.y -= y_val;

				return i;
			}

			float4 frag(v2f_img i) : SV_Target
			{
				return float4(0,1,1,1);
			}

            ENDCG
        }
    }
}