﻿// Modified unity standar shader supporting curved vertex shader with an alpha channel blend map
Shader "Curved/Standard 4 Channel Colored"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        _ChannelRedColor("Channel 1 Color (Red)", Color) = (1,1,1,1)
        _ChannelGreenColor("Channel 2 Color (Green)", Color) = (1,1,1,1)
        _ChannelBlueColor("Channel 3 Color (Blue)", Color) = (1,1,1,1)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}

        _Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
        _ParallaxMap ("Height Map", 2D) = "black" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        [Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        [HideInInspector] _ZTest ("__zt", Float) = 2.0
    }

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG

    SubShader 
    {
        Name "Color"
        Tags { "LightMode" = "Opaque" }

        CGPROGRAM
        #pragma surface surf Standard

        sampler2D _MainTex;
        float4 _Color;
        float4 _ChannelRedColor;
        float4 _ChannelGreenColor;
        float4 _ChannelBlueColor;

        struct Input {
            float2 uv_MainTex;
        };

        float3 BlendAlbedo(float3 DetailColor, float DetailAlpha)
        {
            float3 finalColor;
            finalColor = _ChannelRedColor.rgb * DetailColor.r;
            finalColor += _ChannelGreenColor.rgb * DetailColor.g;
            finalColor += _ChannelBlueColor.rgb * DetailColor.b;
            return lerp(finalColor, DetailColor * _Color.rgb, DetailAlpha);
        }

        half3 ComputeAlbedo(float2 texcoords)
        {
            half4 detail = tex2D(_MainTex, texcoords.xy).rgba;
            return BlendAlbedo(detail.rgb, detail.a);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = ComputeAlbedo(IN.uv_MainTex);
        }
        ENDCG
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300

        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON // only need forward variations for transparent
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            //#pragma shader_feature_local ___ _DETAIL_MULX2
            #pragma shader_feature_local _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup
            //#pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile __ FIRSTPERSONVIEW LOD_FADE_CROSSFADE

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #pragma vertex vert
            #pragma fragment fragBase

            #include "CurvedHelper.cginc"
            #include "UnityStandardCoreForward.cginc"

#if UNITY_STANDARD_SIMPLE
            VertexOutputBaseSimple vert(VertexInput v)
#else
            VertexOutputForwardBase vert(VertexInput v)
#endif
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                return vertBase(v);
            }
            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest [_ZTest]

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------


            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON // only need forward variations for transparent
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            //#pragma shader_feature_local ___ _DETAIL_MULX2
            //#pragma shader_feature_local _PARALLAXMAP
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup

            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma multi_compile __ FIRSTPERSONVIEW LOD_FADE_CROSSFADE

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #pragma vertex vert
            #pragma fragment fragAdd

            #include "UnityStandardCoreForward.cginc"
            #include "CurvedHelper.cginc"

#if UNITY_STANDARD_SIMPLE
            VertexOutputForwardAddSimple vert(VertexInput v)
#else
            VertexOutputForwardAdd vert(VertexInput v)
#endif
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                return vertAdd(v);
            }
            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------


            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup
            
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            #pragma multi_compile _ LOD_FADE_CROSSFADE

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #pragma vertex vert
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"
            #include "CurvedHelper.cginc"

#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
            void vert(VertexInput v, out float4 opos : SV_POSITION, out VertexOutputShadowCaster o)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                opos = vertShadowCaster(v, o).pos;
            }
#elif UNITY_STANDARD_USE_STEREO_SHADOW_OUTPUT_STRUCT
            void vert(VertexInput v, out float4 opos : SV_POSITION, out VertexOutputStereoShadowCaster os)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                opos = vertShadowCaster(v, os).pos;
            }
#else
            void vert(VertexInput v, out float4 opos : SV_POSITION)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                opos = vertShadowCaster(v).pos;
            }
#endif
            ENDCG
        }
        // ------------------------------------------------------------------
        //  Deferred pass
        Pass
        {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            #pragma target 3.0
            #pragma exclude_renderers nomrt


            // -------------------------------------

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local ___ _DETAIL_MULX2
            #pragma shader_feature_local _PARALLAXMAP
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup

            #pragma multi_compile_prepassfinal
            #pragma multi_compile_instancing
            #pragma multi_compile __ FIRSTPERSONVIEW LOD_FADE_CROSSFADE

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #pragma vertex vert
            #pragma fragment fragDeferred

            #include "UnityStandardCore.cginc"
            #include "CurvedHelper.cginc"

            VertexOutputDeferred vert(VertexInput v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                return vertDeferred(v);
            }
            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local ___ _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup

            #pragma multi_compile __ FIRSTPERSONVIEW

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #pragma vertex vert
            #pragma fragment frag_meta

            #include "UnityStandardMeta.cginc"
            #include "CurvedHelper.cginc"

            v2f_meta vert(VertexInput v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                return vert_meta(v);
            }
            ENDCG
        }
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 150

        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON // only need forward variations for transparent
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            // SM2.0: NOT SUPPORTED shader_feature_local ___ _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup

            #pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile __ FIRSTPERSONVIEW

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #pragma vertex vert
            #pragma fragment fragBase

            #include "UnityStandardCoreForward.cginc"
            #include "CurvedHelper.cginc"

            VertexOutputForwardBase vert(VertexInput v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                return vertBase(v);
            }
            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON // only need forward variations for transparent
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            //#pragma shader_feature_local ___ _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP
            #pragma skip_variants SHADOWS_SOFT
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup

            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma multi_compile __ FIRSTPERSONVIEW

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #pragma vertex vert
            #pragma fragment fragAdd

            #include "UnityStandardCoreForward.cginc"
            #include "CurvedHelper.cginc"

            VertexOutputForwardAdd vert(VertexInput v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                return vertAdd(v);
            }
            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup
            
            #pragma skip_variants SHADOWS_SOFT
            
            #pragma multi_compile_shadowcaster
            #pragma multi_compile __ FIRSTPERSONVIEW

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #pragma vertex vert
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"
            #include "CurvedHelper.cginc"

#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
            void vert(VertexInput v, out float4 opos : SV_POSITION, out VertexOutputShadowCaster o)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                opos = vertShadowCaster(v, o).pos;
            }
#elif UNITY_STANDARD_USE_STEREO_SHADOW_OUTPUT_STRUCT
            void vert(VertexInput v, out float4 opos : SV_POSITION, out VertexOutputStereoShadowCaster os)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                opos = vertShadowCaster(v, os).pos;
            }
#else
            void vert(VertexInput v, out float4 opos : SV_POSITION)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                opos = vertShadowCaster(v).pos;
            }
#endif
            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local ___ _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION
            #pragma instancing_options assumeuniformscaling lodfade procedural:IndirectRenderingSetup

            // Eco keywords
            #pragma shader_feature NO_CURVE

            #include "UnityStandardMeta.cginc"
            #include "CurvedHelper.cginc"

            v2f_meta vert(VertexInput v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                return vert_meta(v);
            }
            ENDCG
        }
    }

    FallBack "VertexLit"
    //CustomEditor "StandardShaderGUI"
}
