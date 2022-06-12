// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "YOS/TORNADO/CORE"
{
	Properties
	{
		_POWERV("POWER V", Float) = 1
		_INTENSITY("INTENSITY ", Range( 0 , 1)) = 1
		_WAVES("WAVES", Float) = 17.39
		_triangular("triangular", Float) = 1
		_triangular2("triangular2", Float) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_SPEED("SPEED", Vector) = (0,0,0,0)
		_GRADIENTE("GRADIENTE", 2D) = "white" {}
		_POWERGRAD("POWER GRAD", Float) = 1
		_REMAPSAMPLE("REMAPSAMPLE", Float) = 0.2
		_DESTRUCTOR("DESTRUCTOR", Float) = 5.66
		_SPEEDROTATEAROUNDAXIS("SPEED ROTATE AROUND AXIS", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _SPEEDROTATEAROUNDAXIS;
		uniform float _POWERV;
		uniform float _INTENSITY;
		uniform float _WAVES;
		uniform float _triangular;
		uniform float _triangular2;
		uniform sampler2D _GRADIENTE;
		uniform float _DESTRUCTOR;
		uniform sampler2D _TextureSample0;
		uniform float2 _SPEED;
		uniform float4 _TextureSample0_ST;
		uniform float _POWERGRAD;
		uniform float _REMAPSAMPLE;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float mulTime33 = _Time.y * _SPEEDROTATEAROUNDAXIS;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_34_0 = ( mulTime33 + (0.0 + (ase_vertex3Pos.z - 0.0) * (5.0 - 0.0) / (19.6 - 0.0)) );
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
			float2 uv0_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float2 panner46 = ( 1.0 * _Time.y * _SPEED + uv0_TextureSample0);
			float2 temp_cast_0 = (( floor( ( _DESTRUCTOR * (_REMAPSAMPLE + (pow( tex2D( _TextureSample0, panner46 ).r , _POWERGRAD ) - 0.0) * (1.0 - _REMAPSAMPLE) / (1.0 - 0.0)) ) ) / _DESTRUCTOR )).xx;
			o.Emission = tex2D( _GRADIENTE, temp_cast_0 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
600;314;999;603;-1711.196;1260.046;2.561055;True;False
Node;AmplifyShaderEditor.CommentaryNode;44;-2006.886,-73.24353;Inherit;False;2069.76;966.2808;MOV ONDAS ;18;28;30;27;29;10;26;21;19;20;23;22;5;6;24;9;8;7;25;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;48;324.0366,-482.3337;Inherit;False;Property;_SPEED;SPEED;6;0;Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;282.6226,-710.1085;Inherit;False;0;45;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;43;-1554.739,-1077.891;Inherit;False;1527.703;719.7592;MOV ESPIRAL TORNADO;10;31;32;33;34;40;37;35;39;36;41;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1921.318,201.7681;Inherit;False;Property;_WAVES;WAVES;2;0;Create;True;0;0;False;0;False;17.39;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;19;-1956.886,16.24268;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;46;565.6152,-568.6118;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;23;-1609.407,-23.24353;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-1362.09,-1160.113;Inherit;False;Property;_SPEEDROTATEAROUNDAXIS;SPEED ROTATE AROUND AXIS;11;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;31;-1504.739,-980.658;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;20;-1675.217,68.89094;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;19.6;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;854.0281,-657.7706;Inherit;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;1034.969,-380.5253;Inherit;False;Property;_POWERGRAD;POWER GRAD;8;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;1311.469,-353.439;Inherit;False;Property;_REMAPSAMPLE;REMAPSAMPLE;9;0;Create;True;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;50;1236.86,-518.5706;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;33;-927.7283,-1027.891;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;32;-1222.65,-971.1801;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;19.6;False;3;FLOAT;0;False;4;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-1376.439,51.7802;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-711.6326,535.7524;Inherit;False;Property;_triangular;triangular;3;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;52;1504.025,-542.1039;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;1677.19,-909.1583;Inherit;False;Property;_DESTRUCTOR;DESTRUCTOR;10;0;Create;True;0;0;False;0;False;5.66;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1409.575,677.1333;Inherit;False;Property;_POWERV;POWER V;0;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-694.7595,-952.8674;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1370.716,227.2022;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;24;-1162.098,-3.300111;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;1805.701,-723.565;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;27;-517.1611,522.3375;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1258.35,777.0372;Inherit;False;Property;_INTENSITY;INTENSITY ;1;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-660.5699,709.1367;Inherit;False;Property;_triangular2;triangular2;4;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;25;-895.0247,150.241;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-841.1238,-715.3636;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosOpNode;37;-458.4626,-712.8192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;8;-1226.35,627.0372;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;9;-1121.287,475.1982;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;35;-417.8792,-888.1102;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-549.3854,202.1794;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;39;-539.7324,-612.1318;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0.57;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;56;2028.926,-824.5147;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-316.4521,581.7646;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;36;-223.7906,-812.9971;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-89.12534,-22.80541;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-189.0351,-527.7816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;2036.926,-685.5147;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;808.501,142.3714;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;49;2341.473,-779.5527;Inherit;True;Property;_GRADIENTE;GRADIENTE;7;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;59;3133.53,-505.289;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;YOS/TORNADO/CORE;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;46;0;47;0
WireConnection;46;2;48;0
WireConnection;20;0;19;3
WireConnection;20;4;21;0
WireConnection;45;1;46;0
WireConnection;50;0;45;1
WireConnection;50;1;51;0
WireConnection;33;0;58;0
WireConnection;32;0;31;3
WireConnection;22;0;23;0
WireConnection;22;1;20;0
WireConnection;52;0;50;0
WireConnection;52;3;53;0
WireConnection;34;0;33;0
WireConnection;34;1;32;0
WireConnection;24;0;22;0
WireConnection;55;0;54;0
WireConnection;55;1;52;0
WireConnection;27;0;5;2
WireConnection;27;1;28;0
WireConnection;25;0;24;0
WireConnection;37;0;34;0
WireConnection;9;0;5;2
WireConnection;9;1;6;0
WireConnection;35;0;34;0
WireConnection;10;0;9;0
WireConnection;10;1;8;0
WireConnection;10;2;7;0
WireConnection;10;3;25;0
WireConnection;39;0;40;2
WireConnection;56;0;55;0
WireConnection;29;0;27;0
WireConnection;29;1;8;0
WireConnection;29;2;30;0
WireConnection;36;0;35;0
WireConnection;36;1;37;0
WireConnection;26;0;10;0
WireConnection;26;1;29;0
WireConnection;41;0;39;0
WireConnection;41;1;36;0
WireConnection;57;0;56;0
WireConnection;57;1;54;0
WireConnection;38;0;41;0
WireConnection;38;1;26;0
WireConnection;49;1;57;0
WireConnection;59;2;49;0
WireConnection;59;11;38;0
ASEEND*/
//CHKSM=520B351A332C0338D6D38BD92CEF8BAF8E1717D3