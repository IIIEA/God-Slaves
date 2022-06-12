// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "YOS/TORNADO/EXTERN"
{
	Properties
	{
		_POWERV("POWER V", Float) = 1.02
		_INTENSITY("INTENSITY", Range( 0 , 1)) = 1
		_WAVES("WAVES", Float) = 17.39
		_triangular("triangular", Float) = 1
		_triangular2("triangular2", Float) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_SPEED("SPEED", Vector) = (0,0,0,0)
		_POWERGRAD("POWER GRAD", Float) = 1
		_REMAPSAMPLE("REMAPSAMPLE", Float) = 0.2
		_DESTRUCTOR("DESTRUCTOR", Float) = 5.66
		[HDR]_Color0("Color 0", Color) = (0,0,0,0)
		_OPACITY("OPACITY", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _POWERV;
		uniform float _INTENSITY;
		uniform float _WAVES;
		uniform float _triangular;
		uniform float _triangular2;
		uniform float4 _Color0;
		uniform float _DESTRUCTOR;
		uniform sampler2D _TextureSample0;
		uniform float2 _SPEED;
		uniform float4 _TextureSample0_ST;
		uniform float _POWERGRAD;
		uniform float _REMAPSAMPLE;
		uniform float _OPACITY;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_34_0 = ( _Time.y + (0.0 + (ase_vertex3Pos.z - 0.0) * (5.0 - 0.0) / (19.6 - 0.0)) );
			float4 appendResult36 = (float4(sin( temp_output_34_0 ) , cos( temp_output_34_0 ) , 0.0 , 0.0));
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( pow( v.texcoord.xy.y , 0.57 ) * appendResult36 ) + float4( ( ( pow( v.texcoord.xy.y , _POWERV ) * ase_vertexNormal * _INTENSITY * (0.0 + (sin( ( _Time.y + (0.0 + (ase_vertex3Pos.z - 0.0) * (_WAVES - 0.0) / (19.6 - 0.0)) ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) ) + ( pow( v.texcoord.xy.y , _triangular ) * ase_vertexNormal * _triangular2 ) ) , 0.0 ) ).xyz;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color0.rgb;
			float2 uv0_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float2 panner46 = ( 1.0 * _Time.y * _SPEED + uv0_TextureSample0);
			float clampResult63 = clamp( ( ( floor( ( _DESTRUCTOR * (_REMAPSAMPLE + (pow( tex2D( _TextureSample0, panner46 ).r , _POWERGRAD ) - 0.0) * (1.0 - _REMAPSAMPLE) / (1.0 - 0.0)) ) ) / _DESTRUCTOR ) * _OPACITY ) , 0.0 , 1.0 );
			o.Alpha = clampResult63;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
414;422;999;597;-2034.351;365.5017;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;282.6226,-710.1085;Inherit;False;0;45;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;48;324.0366,-482.3337;Inherit;False;Property;_SPEED;SPEED;6;0;Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;44;-2006.886,-73.24353;Inherit;False;2069.76;966.2808;MOV ONDAS ;18;28;30;27;29;10;26;21;19;20;23;22;5;6;24;9;8;7;25;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;46;565.6152,-568.6118;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;45;854.0281,-657.7706;Inherit;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;1034.969,-380.5253;Inherit;True;Property;_POWERGRAD;POWER GRAD;7;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1921.318,201.7681;Inherit;False;Property;_WAVES;WAVES;2;0;Create;True;0;0;False;0;False;17.39;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;43;-1554.739,-1077.891;Inherit;False;1527.703;719.7592;MOV ESPIRAL TORNADO;10;31;32;33;34;40;37;35;39;36;41;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;19;-1956.886,16.24268;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;23;-1609.407,-23.24353;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;20;-1675.217,68.89094;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;19.6;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;50;1236.86,-518.5706;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;1311.469,-353.439;Inherit;False;Property;_REMAPSAMPLE;REMAPSAMPLE;8;0;Create;True;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;31;-1504.739,-980.658;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-1376.439,51.7802;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;52;1504.025,-542.1039;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;1677.19,-909.1583;Inherit;False;Property;_DESTRUCTOR;DESTRUCTOR;9;0;Create;True;0;0;False;0;False;5.66;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;33;-927.7283,-1027.891;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;32;-1222.65,-971.1801;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;19.6;False;3;FLOAT;0;False;4;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-694.7595,-952.8674;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;1805.701,-723.565;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;24;-1162.098,-3.300111;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-711.6326,535.7524;Inherit;False;Property;_triangular;triangular;3;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1470.716,392.2022;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-1409.575,677.1333;Inherit;False;Property;_POWERV;POWER V;0;0;Create;True;0;0;False;0;False;1.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;25;-895.0247,150.241;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;56;2028.926,-824.5147;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;37;-458.4626,-712.8192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-841.1238,-715.3636;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;30;-671.5699,628.1367;Inherit;False;Property;_triangular2;triangular2;4;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;8;-1226.35,627.0372;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;9;-1121.287,475.1982;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;27;-517.1611,522.3375;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;35;-417.8792,-888.1102;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1258.35,777.0372;Inherit;False;Property;_INTENSITY;INTENSITY;1;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-316.4521,581.7646;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;2036.926,-685.5147;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;2151.28,-405.0127;Inherit;False;Property;_OPACITY;OPACITY;11;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;39;-539.7324,-612.1318;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0.57;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;36;-223.7906,-812.9971;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-549.3854,202.1794;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-189.0351,-527.7816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;2294.083,-525.5527;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-89.12534,-22.80541;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;59;-48.60522,42.80145;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;63;2466.097,-490.5607;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;184.3635,117.8251;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;58;2645.649,-741.1386;Inherit;False;Property;_Color0;Color 0;10;1;[HDR];Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;38;2419.289,21.56231;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;64;2883.775,-496.556;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;YOS/TORNADO/EXTERN;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;46;0;47;0
WireConnection;46;2;48;0
WireConnection;45;1;46;0
WireConnection;20;0;19;3
WireConnection;20;4;21;0
WireConnection;50;0;45;1
WireConnection;50;1;51;0
WireConnection;22;0;23;0
WireConnection;22;1;20;0
WireConnection;52;0;50;0
WireConnection;52;3;53;0
WireConnection;32;0;31;3
WireConnection;34;0;33;0
WireConnection;34;1;32;0
WireConnection;55;0;54;0
WireConnection;55;1;52;0
WireConnection;24;0;22;0
WireConnection;25;0;24;0
WireConnection;56;0;55;0
WireConnection;37;0;34;0
WireConnection;9;0;5;2
WireConnection;9;1;6;0
WireConnection;27;0;5;2
WireConnection;27;1;28;0
WireConnection;35;0;34;0
WireConnection;29;0;27;0
WireConnection;29;1;8;0
WireConnection;29;2;30;0
WireConnection;57;0;56;0
WireConnection;57;1;54;0
WireConnection;39;0;40;2
WireConnection;36;0;35;0
WireConnection;36;1;37;0
WireConnection;10;0;9;0
WireConnection;10;1;8;0
WireConnection;10;2;7;0
WireConnection;10;3;25;0
WireConnection;41;0;39;0
WireConnection;41;1;36;0
WireConnection;61;0;57;0
WireConnection;61;1;62;0
WireConnection;26;0;10;0
WireConnection;26;1;29;0
WireConnection;63;0;61;0
WireConnection;60;0;59;0
WireConnection;38;0;41;0
WireConnection;38;1;26;0
WireConnection;64;2;58;0
WireConnection;64;9;63;0
WireConnection;64;11;38;0
ASEEND*/
//CHKSM=FB45D911122A893E5CB08B94F0DF4F4DEE9A5851