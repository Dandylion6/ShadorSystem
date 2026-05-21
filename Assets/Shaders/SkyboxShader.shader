Shader "Custom/SkyboxShader"
{
    Properties
    {
        [MainColor] _SpaceColor("Space Color", Color) = (1, 1, 1, 1)
        _StarSize("Star Size", Range(0.001, 1.0)) = 0.1
        _StarDensity("Star Density", Range(0.001, 1.0)) = 0.1
        _StarSharpness("Star Sharpness", Range(1.0, 10.0)) = 2.0
        _StarTwinkleSpeed("Star Twinkle Speed", Range(0.1, 10.0)) = 1.0
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "PreviewType" = "Skybox"
        }

        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _SpaceColor;
                float _StarSize;
                float _StarDensity;
                float _StarSharpness;
                float _StarTwinkleSpeed;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            float2 hash2(float3 seed)
            {
                seed = float3(dot(seed, float3(127.1, 311.7, 74.7)),
                           dot(seed, float3(269.5, 183.3, 246.1)),
                           dot(seed, float3(113.5,  271.9, 124.6)));
                return frac(sin(seed.xy + seed.z) * 43758.5453);
            }

            float3 star(float3 direction)
            {
                float cellSize = 1.0 / _StarSize;
                float3 cell = round(direction * cellSize);
                float3 localPosition = (direction * cellSize) - cell;

                float2 random = hash2(cell); // Both value x and y are used as separate 'random' values.

                float spawnChance = step(1.0 - _StarDensity, random.x);
                if (spawnChance < 0.001) return 0.0; // In case of floating point error.

                float3 starColor = lerp(float3(1, 0.404, 0.31), float3(0.42, 0.749, 1), random.y);
                float starSize = lerp(0.6, 1.6, random.y);
                float brightness = lerp(0.9, 2.2, random.y);

                float distance = length(localPosition) / (0.5 * starSize);
                float shape = pow(saturate(1.0 - distance), _StarSharpness); // Create a star shape by fading out from the center.

                float twinkePhase = random.y * 6.2831;
                starColor *= 0.7 + 0.3 * sin((_Time.y - twinkePhase) * _StarTwinkleSpeed); // Twinkle effect.

                return spawnChance * shape * starColor * brightness;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Base color to fill in the sky.
                half4 color = _SpaceColor;
                
                float3 direction = normalize(IN.positionWS);                
                color.rgb += star(direction); // Add stars to the sky.

                return color;
            }
            ENDHLSL
        }
    }
}
