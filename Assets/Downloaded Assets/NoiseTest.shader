Shader "Custom/NoiseTest"
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
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

			float _Length;
			float _Speed;
			float _Height;

			v2f_img vert(appdata_base v)
			{
				v2f_img i;
				i.pos = UnityObjectToClipPos(v.vertex);
				i.uv = v.texcoord.xy;
        

				float2 uv = i.uv * 4.0 + float2(0.2, 1) * _Time.y;

				float y_val = 0.5;
		
				//Fractal?
				//for (int i = 0; i < 6; i++)
				{
					float3 coord = float3(uv * _Length, _Time.y * _Speed);
					y_val += cnoise(coord) * _Height;
				}

				i.pos.y -= y_val;

				return i;
			}

			float4 frag(v2f_img i) : SV_Target
			{
				/*
				float2 uv = i.uv * 4.0 + float2(0.2, 1) * _Time.y;
				//float2 uv = i.uv * 4.0 + float2(0.2, 1);

				float o = 0.5;
				float s = 1.0;
				float w = 0.5;
		
				//Fractal?
				//for (int i = 0; i < 6; i++)
				{
					float3 coord = float3(uv * s, _Time.y);
					o += cnoise(coord) * w;
					s *= 2.0;
					w *= 0.5;
				}

				return float4(o, o, o, 1);
				*/
				return float4(0,1,1,1);
			}

            ENDCG
        }
    }
}