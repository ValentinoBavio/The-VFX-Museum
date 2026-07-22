Shader "Custom/Shd_Fog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 2
        _Softness ("Softness", Float) = 0.5
        _ScrollSpeed ("Scroll Speed", Vector) = (0,0.2,0,0)
        _Alpha ("Alpha", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _PlayerPos;
                float _Radius;
                float _Softness;
                float4 _ScrollSpeed;
                float _Alpha;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionHCS = pos.positionCS;
                OUT.worldPos = pos.positionWS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv + _ScrollSpeed.xy * _Time.y;

                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                float dist = distance(IN.worldPos, _PlayerPos.xyz);

                float mask = smoothstep(_Radius, _Radius + _Softness, dist);

                color.a *= mask;
                color.a *= _Alpha;

                return color;
            }

            ENDHLSL
        }
    }
}