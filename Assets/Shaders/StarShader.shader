Shader "Custom/StarShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _EmissionMap("Emission Map", 2D) = "white" {}
        _EdgePower("Edge Power", Range(0.1, 5.0)) = 2.0
        _EmissionStrength("Emission Strength", float) = 3.0
        _Radius("Radius", float) = 6.0
        _WarpStrength("Warp Strength", Range(0.001, 1.0)) = 0.5
        _WarpSpeed("Warp Speed", Range(0.1, 10.0)) = 0.4
        _WarpScale("Warp Scale", Range(0.001, 2.0)) = 0.2
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
                float _WarpStrength;
                float _WarpSpeed;
                float _WarpScale;
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

            float2 warpUV(float2 uv, float scale, float speed, float strength)
            {
                uv.x += sin((uv.y * scale + _Time.y) * speed * 0.5) * strength;
                uv.x += sin((uv.y * scale - 3.14 + _Time.y) * speed) * strength * 0.5;
                uv.x += sin((uv.y * scale - 1.6 + _Time.y) * speed * 2.0) * strength * 0.25;

                uv.y += cos((uv.x * scale + _Time.y) * speed * 0.5) * strength;
                uv.y += cos((uv.x * scale - 3.14 + _Time.y) * speed) * strength * 0.5;
                uv.y += cos((uv.x * scale - 1.6 + _Time.y) * speed * 2.0) * strength * 0.25;
                return uv;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 projectionPosition = (IN.positionOS / _Radius) - 0.5;

                float warpStrength = _WarpStrength / _Radius;
                float2 uvYZ = warpUV(projectionPosition.yz, _Radius * _WarpScale, _WarpSpeed, warpStrength);
                float2 uvXZ = warpUV(projectionPosition.xz, _Radius * _WarpScale, _WarpSpeed * 0.8, warpStrength);
                float2 uvXY = warpUV(projectionPosition.xy, _Radius * _WarpScale, _WarpSpeed * 1.3, warpStrength);

                half4 emissionX = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uvYZ);
                half4 emissionY = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uvXZ);
                half4 emissionZ = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uvXY);

                float3 weights = abs(IN.normalOS);
                weights = pow(weights, float3(1.0, 1.0, 1.0) * 10.0); // Makes the blur less.
                weights /= weights.x + weights.y + weights.z;

                half4 emission = saturate(emissionX * weights.x + emissionY * weights.y + emissionZ * weights.z);

                // Fresnel
                float3 wordSpaceNormal = normalize(IN.normalWS);
                float3 viewDirection = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float normalToViewProjection = dot(wordSpaceNormal, viewDirection);
                float fresnel = pow(1.0 - saturate(normalToViewProjection), _EdgePower);

                half4 color = _BaseColor;
                float brightness = lerp(min(1.0, _EmissionStrength), _EmissionStrength, fresnel);
                brightness += lerp(-0.2, 0.9, emission) * _EmissionStrength;



                color *= brightness;
                color.a = 1.0;
                return color;
            }
            ENDHLSL
        }
    }
}
