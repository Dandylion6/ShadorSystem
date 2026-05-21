Shader "Custom/StarShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _EmissionMap("Emission Map", 2D) = "white" {}
        _EdgePower("Edge Power", Range(0.1, 5.0)) = 2.0
        _EmissionStrength("Emission Strength", float) = 3.0
        _Radius("Radius", float) = 6.0
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
                float3 normalOS : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
                float3 positionOS : TEXCOORD4;
            };

            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EmissionMap_ST;
                float _EdgePower;
                float _EmissionStrength;
                float _Radius;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _EmissionMap);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.normalOS = IN.normalOS;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionOS = IN.positionOS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Random sine waves to create distortion in uvs.

                float3 projectionPosition = (IN.positionOS / _Radius) - 0.5;
                half4 emissionX = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, projectionPosition.yz);
                half4 emissionY = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, projectionPosition.xz);
                half4 emissionZ = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, projectionPosition.xy);

                float3 weights = abs(IN.normalOS);
                weights = pow(weights, float3(1.0, 1.0, 1.0) * 10.0); // Makes the blur less.
                weights /= weights.x + weights.y + weights.z;

                half4 emission = emissionX * weights.x + emissionY * weights.y + emissionZ * weights.z;

                // Fresnel
                float3 wordSpaceNormal = normalize(IN.normalWS);
                float3 viewDirection = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float normalToViewProjection = dot(wordSpaceNormal, viewDirection);
                float fresnel = pow(1.0 - saturate(normalToViewProjection), _EdgePower);

                half4 color = _BaseColor;
                color.rgb += emission * _EmissionStrength * 0.6;
                color.rgb += color.rgb * fresnel * _EmissionStrength;

                color.a = 1.0;
                return color;
            }
            ENDHLSL
        }
    }
}
