Shader "Unlit/FieldOfVision"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,0.5,0,0.5)
        _PulseColor ("Pulse Color", Color) = (1,0,0,0.5)
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _EdgeGlow ("Edge Glow", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainColor;
                float4 _PulseColor;
                float _PulseSpeed;
                float _EdgeGlow;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(IN.positionOS.xyz));
                return OUT;
            }
            
            float4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 2.0) * _EdgeGlow;
                
                float pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);
                float4 finalColor = lerp(_MainColor, _PulseColor, pulse);
                finalColor.rgb += fresnel;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}
