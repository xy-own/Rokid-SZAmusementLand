Shader "Shader Forge/URP_ParticleBase" {
    Properties{
        _texMain("texMain", 2D) = "white" {}
        [HDR]_MainColor("MainColor", Color) = (0.5019608,0.5019608,0.5019608,1)
        _texFlow("texFlow", 2D) = "white" {}
        [HDR]_FlowColor("FlowColor", Color) = (0,0,0,1)
        [MaterialToggle] _Flow_Mask("Flow_Mask", Float) = 0
        _texMask("texMask", 2D) = "white" {}
        _texNoise("texNoise", 2D) = "white" {}
        _UVmain_dist_ange("UVmain_dist_ange", Vector) = (0,0,0,0)
        _UVflow_dist_alADD("UVflow_dist_alADD", Vector) = (0,0,0,0)
        _MinMax_ange_Scale("MinMax_ange_Scale", Vector) = (0,0,0,1)
        _mkR_dist_Min_Max("mkR_dist_Min_Max", Vector) = (0,0,-0.5,0.5)
        _UVnoise_rSpd_Scale("UVnoise_rSpd_Scale", Vector) = (0,0,0,1)
        [HDR]_dissColor("dissColor", Color) = (1,0.4316475,0,0)
        [Toggle(_USESOFTDIS_ON)] _useSoftDis("useSoftDis",Float) = 0
        _softDiss("softDiss",Range(0,1)) = 1
        _diss("diss", Range(0, 2)) = 0
        _dissOutLine("dissOutLine", Range(0, 1)) = 0
        [HDR]_FresnelColor("FresnelColor", Color) = (0,0,0,1)
        _FresnelExp("FresnelExp", Float) = 1
        [MaterialToggle] _Ns_Mk("Ns_Mk", Float) = 0
        [MaterialToggle] _polar("polar", Float) = 0
        _pointX("pointX", Float) = 0
        _pointY("pointY", Float) = 0
        [MaterialToggle] _par_diss("par_diss", Float) = 0
        [MaterialToggle] _UV1("UV1", Float) = 0
        [MaterialToggle] _Main_uv0("Main_uv0", Float) = 0
        [MaterialToggle] _Mask_uv0("Mask_uv0", Float) = 0
        [MaterialToggle] _Noise_polar("Noise_polar", Float) = 0
        [Toggle(_USEWAVE_ON)] _useWave("useWave", Float) = 0
        _WaveLength("WaveLength", Float) = 10
        _wave("wave", Float) = 0
        _WaveSpeed("WaveSpeed", Float) = 0
        [HideInInspector]_Cutoff("Alpha cutoff", Range(0,1)) = 0.5

        [Toggle(_USECUSTOMDATA_ON)] _useCustomData("useCustomData",Float) = 0

        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull",Float) = 2
        [Enum(On,1,Off,0)]_ZWrite("Zwrite", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend",Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend",Float) = 10
    }
        

            SubShader{
         Tags {
             "RenderPipeline" = "UniversalPipeline"
             "Queue" = "Transparent"
             "RenderType" = "Transparent"
             "IgnoreProjector" = "True"
         }

         Pass {
             Name "ForwardLit"
             Tags { "LightMode" = "UniversalForward" }

             Blend[_SrcBlend][_DstBlend]
             ZWrite[_ZWrite]
             Cull[_Cull]

             HLSLPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_instancing
             #pragma multi_compile __ _USEWAVE_ON
             #pragma multi_compile __ _USESOFTDIS_ON
             #pragma multi_compile __ _USECUSTOMDATA_ON

             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

             struct VertexInput {
                 float4 vertex : POSITION;
                 float3 normal : NORMAL;
                 float4 texcoord0 : TEXCOORD0;
                 float4 texcoord1 : TEXCOORD1;
                 float4 color : COLOR;
                 UNITY_VERTEX_INPUT_INSTANCE_ID
             };

             struct VertexOutput {
                 float4 pos : SV_POSITION;
                 float4 uv0 : TEXCOORD0;
                 float4 uv1 : TEXCOORD1;
                 float3 normalDir : TEXCOORD2;
                 float4 vertexColor : COLOR;
                 float3 posWorld : TEXCOORD3;
                 UNITY_VERTEX_INPUT_INSTANCE_ID
             };

             TEXTURE2D(_texMain); SAMPLER(sampler_texMain);
             TEXTURE2D(_texFlow); SAMPLER(sampler_texFlow);
             TEXTURE2D(_texNoise); SAMPLER(sampler_texNoise);
             TEXTURE2D(_texMask); SAMPLER(sampler_texMask);

             CBUFFER_START(UnityPerMaterial)
                 float4 _texMain_ST;
                 float4 _texFlow_ST;
                 float4 _texNoise_ST;
                 float4 _texMask_ST;
                 half4 _MainColor;
                 half4 _FlowColor;
                 half _diss;
                 half _dissOutLine;
                 half4 _dissColor;
                 half4 _UVflow_dist_alADD;
                 half4 _UVmain_dist_ange;
                 half4 _FresnelColor;
                 half _Ns_Mk;
                 half _FresnelExp;
                 half _polar;
                 half _par_diss;
                 half _UV1;
                 half _Main_uv0;
                 half _Mask_uv0;
                 half _Noise_polar;
                 half4 _UVnoise_rSpd_Scale;
                 half4 _MinMax_ange_Scale;
                 half4 _mkR_dist_Min_Max;
                 half _Flow_Mask;
                 half _pointX;
                 half _pointY;
                 half _WaveLength;
                 half _wave;
                 half _WaveSpeed;
                 half _softDiss;
             CBUFFER_END

             VertexOutput vert(VertexInput v) {
                 VertexOutput o = (VertexOutput)0;
                 UNITY_SETUP_INSTANCE_ID(v);
                 UNITY_TRANSFER_INSTANCE_ID(v, o);
                 o.uv0 = v.texcoord0;
                 o.uv1 = v.texcoord1;
                 o.vertexColor = v.color;
                 o.normalDir = TransformObjectToWorldNormal(v.normal);
                 o.posWorld = TransformObjectToWorld(v.vertex.xyz);
                 o.pos = TransformObjectToHClip(v.vertex.xyz);
                 return o;
             }

             half4 frag(VertexOutput i, half facing : VFACE) : SV_Target{
    UNITY_SETUP_INSTANCE_ID(i);
    half faceSign = (facing >= 0 ? 1 : -1);
    i.normalDir = normalize(i.normalDir) * faceSign;

    // 以下是核心逻辑修改点 
    float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
    float3 normalDirection = i.normalDir;

    // --- 溶解计算部分 ---
    half _diss_var = _diss;
    #ifdef _USECUSTOMDATA_ON 
        half _par_diss_var = lerp(_diss_var * i.uv0.z,  ((1.0 - i.vertexColor.a) * _diss_var) * i.uv0.z,  _par_diss);
    #else 
        half _par_diss_var = lerp(_diss_var, ((1.0 - i.vertexColor.a) * _diss_var), _par_diss);
    #endif 
    half fDissVar = (_par_diss_var - _dissOutLine);

    // --- UV计算部分 ---
    float2 _UV1_var = lerp(i.uv0.xy,  i.uv1.xy,  _UV1);
    float2 point_uv1 = (float2(_pointX,_pointY) + _UV1_var) * 2.0 - 1.0;
    float2 _polar_var = lerp(_UV1_var, float2(length(point_uv1), ((atan2(point_uv1.r,point_uv1.g) / 6.28) + 0.5)), _polar);

    // --- 噪声纹理采样 ---
    float b_ns = _UVnoise_rSpd_Scale.b * _TimeParameters.y;
    float2x2 rotMatrix = float2x2(cos(b_ns), -sin(b_ns), sin(b_ns), cos(b_ns));
    float2 mul_noisetex_uv = mul(rotMatrix, (_UVnoise_rSpd_Scale.rg * _TimeParameters.y + _polar_var - 0.5)) + 0.5;
    float2 a_ns_rcp = 1.0 / _UVnoise_rSpd_Scale.aa;
    float2 noiseUV = mul_noisetex_uv * a_ns_rcp - (0.5 / _UVnoise_rSpd_Scale.a - 0.5);
    float4 _texNoise_var = SAMPLE_TEXTURE2D(_texNoise, sampler_texNoise, TRANSFORM_TEX(noiseUV, _texNoise));

    // --- 遮罩纹理采样 ---
    float r_mk_dist = _mkR_dist_Min_Max.r * 0.0174532925;
    float2x2 mkRotMatrix = float2x2(cos(r_mk_dist), -sin(r_mk_dist), sin(r_mk_dist), cos(r_mk_dist));
    float2 _Mask_uv0_var = lerp(_polar_var, i.uv0.xy,  _Mask_uv0);
    float2 mul_mask_uv = mul(mkRotMatrix, _Mask_uv0_var - 0.5) + 0.5;
    float4 _texMask_var = SAMPLE_TEXTURE2D(_texMask, sampler_texMask, TRANSFORM_TEX(_texNoise_var.rg * _mkR_dist_Min_Max.g + mul_mask_uv, _texMask));

    // --- 主纹理采样 ---
    #ifdef _USEWAVE_ON 
        float2 main_uv = sin(_WaveSpeed * _TimeParameters.y + _WaveLength * i.uv0.x) * _wave + i.uv0.xy * (_texNoise_var.rg * _UVmain_dist_ange.b + 1);
    #else 
        float a_m_dist = _UVmain_dist_ange.a * 0.0174532925;
        float2x2 mainRotMatrix = float2x2(cos(a_m_dist), -sin(a_m_dist), sin(a_m_dist), cos(a_m_dist));
        float2 _Main_uv0_var = lerp(_polar_var, i.uv0.xy,  _Main_uv0);
        float2 main_uv = mul(mainRotMatrix, ((i.uv0.xy - 0.5) * (_texNoise_var.rg * _UVmain_dist_ange.b) + _UVmain_dist_ange.rg * _TimeParameters.y + _Main_uv0_var - 0.5)) + 0.5;
    #endif 
    float4 _texMain_var = SAMPLE_TEXTURE2D(_texMain, sampler_texMain, TRANSFORM_TEX(main_uv, _texMain));

    // --- 最终颜色合成 ---
    float3 finalColor = _texMain_var.rgb * _MainColor.rgb * i.vertexColor.rgb;
    float alpha = _texMain_var.a * _MainColor.a * i.vertexColor.a;

    // 添加溶解效果 
    #ifdef _USESOFTDIS_ON 
        float softFactor = saturate(_texMask_var.r - fDissVar + _softDiss);
        alpha *= softFactor;
    #else 
        clip(_texMask_var.r - fDissVar - 0.5);
    #endif 

    return half4(finalColor, alpha);
             }
             
             ENDHLSL
         }
                     }
                     FallBack "Hidden/Shader Graph/FallbackError"
        }