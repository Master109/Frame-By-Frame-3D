// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/One Sided And Lighting"
{
	Properties
	{
		_Color("Color Tint", Color) = (1.0,1.0,1.0,1.0)
		_MainTex("Diffuse Texture", 2D) = "white" {}
		_SpecColor("Specular Color", Color) = (1.0,1.0,1.0,1.0)
		_Shininess("Shininess", Float) = 10
		[Toggle] _EnableTransparency("Enable Transparency", Int) = 1
		[Enum(Yes,0,No,2)] _Cull("Double Sided", Int) = 2
		[Toggle] _EnableFog("Enable Fog", Int) = 0
	}
	SubShader
	{
		Pass
		{
			Tags {"LightMode" = "ForwardBase"}
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			//user defined variables
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _LightColor0;
			//base input structs
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};
			
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
#ifdef _ENABLEFOG_ON
				UNITY_FOG_COORDS(1)
#endif
#ifdef _ENABLETRANSPARENCY_ON
				float4 screenPos : TEXCOORD2;
#endif
				float4 color : COLOR;
			};

			//vertex Function
			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalDir = normalize( mul( float4( v.normal, 0.0 ), unity_WorldToObject ).xyz );
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = v.texcoord;
				// return o;
				o.color = v.color;
#ifdef _ENABLEFOG_ON
				UNITY_TRANSFER_FOG(o, o.vertex);
#endif
#ifdef _ENABLETRANSPARENCY_ON
				o.screenPos = ComputeScreenPos(o.vertex);
#endif
				return o;
			}

			//fragment function
			float4 frag(vertexOutput i) : COLOR
			{
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz );
				float3 lightDirection;
				float atten;
				if (_WorldSpaceLightPos0.w == 0.0) //directional light
				{
					atten = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(fragmentToLightSource);
					atten = 1.0/distance;
					lightDirection = normalize(fragmentToLightSource);
				}
				//Lighting
				float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
				float3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)) , _Shininess);
				float3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection + specularReflection;// + rimLighting;
				//Texture Maps
				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				float4 col = float4(tex.xyz * lightFinal * _Color.xyz, 1.0);
#ifdef _ENABLEFOG_ON
                UNITY_APPLY_FOG(i.fogCoord, col);
#endif
#ifdef _ENABLETRANSPARENCY_ON
                float2 pos = _ScreenParams.xy * i.screenPos.xy / i.screenPos.w;
                const int MSAASampleCount = 8;
                float ran = frac(52.9829189*frac(dot(pos, float2(0.06711056,0.00583715))));
                col.a = clamp(col.a + 0.99*(ran-0.5)/float(MSAASampleCount), 0.0, 1.0);
#endif
                return col;
			}
ENDCG
		}
	}
}