Shader "Custom/cOcean"
{
	Properties
	{
		_Color ("Color", Color) = (1,0,0,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 1.0
		_WaveLength ("WaveLength", Float) = 0.5
		_WaveHeight ("Wave Height", Float) = 0.5
		_WaveSpeed ("Wave Speed", Float) = 1.0
	}

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
			#pragma target 4.0
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			//These affect the size and speed of waves
			float _WaveLength;
			float _WaveHeight;
			float _WaveSpeed;

			//These are for lighting
			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

			//Struct for a textured/normal lit vert
			struct v2g 
			{
	    		float4  pos : SV_POSITION;
				float3	norm : NORMAL;
	     		float2  uv : TEXCOORD0;
			};

			//Struct for a diffuse/spec lit vert
			struct g2f 
			{
	    		float4  pos : SV_POSITION;
	  			float3  norm : NORMAL;
	   			float2  uv : TEXCOORD0;            
				float3 diffuseColor : TEXCOORD1;
				float3 specularColor : TEXCOORD2;
			};

			//v2g applies height deformation to the vertices
			v2g vert(appdata_full v)
			{
				//Extract the vertex world co-ordinates to a float3
				float3 v0 = mul(unity_ObjectToWorld, v.vertex).xyz;

				//phase0 multiplies the wave height by the sine of the
				//current time and the vertex's position, scaled by wave speed and length
				//This gives the effect of a diagonal wave as both
				//x and z are added into the same sine function
				float phase0 = (_WaveHeight) * 
								sin(
								(_Time[1] * _WaveSpeed) + 
								(v0.x * _WaveLength) + 
								(v0.z * _WaveLength)
								);

				//This value is then added to the vertex's y-pos
				v0.y += phase0;

				//Insert the new co-ord back into object space and apply to the vertex
				v.vertex.xyz = mul((float3x3)unity_WorldToObject, v0);

				//Return a copy of the vertex data
	    		v2g OUT;
				OUT.pos = v.vertex;
				OUT.norm = v.normal;
	    		OUT.uv = v.texcoord;
	    		return OUT;
			}

			//This section calculates lighting and colouration, traditional shader functionality
			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
			{
				//Gets the verts of each tri
				float3 v0 = IN[0].pos.xyz;
				float3 v1 = IN[1].pos.xyz;
				float3 v2 = IN[2].pos.xyz;

				//Finds the centre of tri
				float3 centerPos = (v0 + v1 + v2) / 3.0;

				//Finds the normal by way of cross product
				float3 vn = normalize(cross(v1 - v0, v2 - v0));

				//Get the obj-to-world and world-to-obj matrices
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(
					mul(float4(vn, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos
					- mul(modelMatrix, float4(centerPos, 0.0)).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float attenuation = 1.0;

				float3 ambientLighting =
					UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				float3 diffuseReflection =
					attenuation * _LightColor0.rgb * _Color.rgb
					* max(0.0, dot(normalDirection, lightDirection));

				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0)
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else
				{
					specularReflection = attenuation * _LightColor0.rgb
						* _SpecColor.rgb * pow(max(0.0, dot(
							reflect(-lightDirection, normalDirection),
							viewDirection)), _Shininess);
				}

				g2f OUT;
				OUT.pos = UnityObjectToClipPos(IN[0].pos);
				OUT.norm = vn;
				OUT.uv = IN[0].uv;
				OUT.diffuseColor = ambientLighting + diffuseReflection;
				OUT.specularColor = specularReflection;
				triStream.Append(OUT);

				OUT.pos = UnityObjectToClipPos(IN[1].pos);
				OUT.norm = vn;
				OUT.uv = IN[1].uv;
				OUT.diffuseColor = ambientLighting + diffuseReflection;
				OUT.specularColor = specularReflection;
				triStream.Append(OUT);

				OUT.pos = UnityObjectToClipPos(IN[2].pos);
				OUT.norm = vn;
				OUT.uv = IN[2].uv;
				OUT.diffuseColor = ambientLighting + diffuseReflection;
				OUT.specularColor = specularReflection;
				triStream.Append(OUT);
				
			}
			
			half4 frag(g2f IN) : COLOR
			{
				return float4(IN.specularColor +
				IN.diffuseColor, 1.0);
			}
			
			ENDCG
		}
	}
}