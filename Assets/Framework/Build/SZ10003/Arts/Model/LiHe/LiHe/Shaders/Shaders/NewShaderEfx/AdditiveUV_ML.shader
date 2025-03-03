// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "<effect>_ML/AdditiveUV_ML" {
    Properties {
        _diffuse ("diffuse", 2D) = "white" {}
        _Intensity ("Intensity", Float ) = 1
        _mask ("mask", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Uspeed ("Uspeed", Float ) = 0
        _Vspeed ("Vspeed", Float ) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
			Blend SrcAlpha One
            Cull Off
            ZWrite Off
			Fog { Mode off }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _TimeEditor;
            uniform sampler2D _diffuse; 
			uniform float4 _diffuse_ST;
            uniform float _Intensity;
            uniform sampler2D _mask; 
			uniform float4 _mask_ST;
            uniform float4 _Color;
            uniform float _Uspeed;
            uniform float _Vspeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                fixed4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv : TEXCOORD1;
                fixed4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv = v.texcoord0 + float2(_Uspeed, _Vspeed) * (_Time + _TimeEditor).g * 0.5;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR 
            {
                float4 _diffuse_var = tex2D(_diffuse,TRANSFORM_TEX(i.uv, _diffuse));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
				_Color.a = saturate(_Color.a);//UNITY_2017
				return _Color * i.vertexColor * _diffuse_var * _mask_var.r * _Intensity;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
