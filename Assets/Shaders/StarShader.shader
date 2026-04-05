Shader "Custom/StarShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _EmissionMap("Emission Map", 2D) = "white" {}
        _EdgePower("Edge Power", Range(0.1, 5.0)) = 2.0
        _EmissionStrength("Emission Strength", float) = 3.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EmissionMap_ST;
                float _EdgePower;
                float _EmissionStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _EmissionMap);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Random sine waves to create distortion in uvs.
                float2 uv = IN.uv;
                uv.x += sin((_Time.x - IN.uv.y) * 0.7) * 0.2;
                uv.x += -sin((_Time.x - IN.uv.y - 0.6) * 1.9) * 0.09;
                uv.y += sin((_Time.y - IN.uv.x) * 0.8) * 0.04;
                uv.y += -sin((_Time.y - IN.uv.x + 0.2) * 1.7) * 0.01;

                half4 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv);

                // Fresnel
                float3 wordSpaceNormal = normalize(IN.normalWS);
                float3 viewDirection = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float normalToViewProjection = dot(wordSpaceNormal, viewDirection);
                float fresnel = pow(1.0 - saturate(normalToViewProjection), _EdgePower);

                half4 color = _BaseColor;
                color.rgb += emission.rgb * _EmissionStrength * 0.3;
                color.rgb += fresnel * _BaseColor.rgb * _EmissionStrength;

                color.a = 1.0;
                return color;
            }
            ENDHLSL
        }
    }
}
