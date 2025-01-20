Shader "Unlit/Graph Vertex Shader"
{
    Properties
    {
        _Color ("Base Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : NORMAL;
                float4 instanceColor : COLOR; // Pass the calculated color to the fragment shader
            };
            
            float4 _BaseColor;
            float _Scale;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _InstancePosition) // Per-instance position
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                
                // Access per-instance position
                float4 instancePosition = UNITY_ACCESS_INSTANCED_PROP(Props, _InstancePosition);
                
                // Apply scale to vertex position
                float4 scaledVertex = v.vertex * _Scale;
                float4 worldPos = scaledVertex + instancePosition;

                o.pos = UnityObjectToClipPos(worldPos);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                // Calculate color based on position (normalized)
                float3 normalizedPosition = abs(instancePosition.xyz) / 10.0; // Scale for visibility
                o.instanceColor = float4(normalizedPosition, 1.0); // RGB from position, Alpha = 1
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                return i.instanceColor * 1; // Multiply the base color by the calculated instance color
            }
            ENDCG
        }
    }
}
