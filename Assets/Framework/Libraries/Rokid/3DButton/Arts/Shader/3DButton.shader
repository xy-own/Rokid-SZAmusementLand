Shader "Unlit/3DButton"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _EdgeLight("Edge Light Text",2D) = "black"{}
        _EdgeStrength("Edge Strength", Range(0, 1)) = 0
        _Alpha("Alpha",Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _EdgeLight;
            float _EdgeStrength;
            float _Alpha;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 base = tex2D(_MainTex, i.uv);
                fixed4 light = tex2D(_EdgeLight, i.uv);
                light.a = light.a * _EdgeStrength;
                fixed4 col;
                col.rgb = light.rgb * light.a + (1-light.a) * base.rgb; 
                col.a = (base.a + light.a) * _Alpha ;
                return col;
            }
            ENDCG
        }
    }
}
