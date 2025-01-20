Shader "Custom/GPUGraphShader"
{
    Properties
    {
        _Resolution("Resolution", Int) = 10
        _Step("Step", Float) = 0.1
        _Function("Function", Int) = 0
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
            #define PI 3.14159265359

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 color : COLOR;
            };

            float _Step;
            int _Resolution;
            int _Function;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            // Wave function
            float3 Wave(float u, float v, float t)
            {
                return float3(u, sin(3.141592 * (u + v + t)), v);
            }

            // MultiWave function
            float3 MultiWave(float u, float v, float t)
            {
                float3 p;
                p.x = u;
                p.y = sin(3.141592 * (u + 0.5 * t)) +
                      0.5 * sin(6.283185 * (v + t)) +
                      sin(3.141592 * (u + v + 0.25 * t)) * 0.4;
                p.z = v;
                return p;
            }

            float3 Ripple (float u, float v, float t) {
                float d = sqrt(u * u + v * v);
                float3 p;
                p.x = u;
                p.y = sin(PI * (4.0 * d - t));
                p.y /= 1.0 + 10.0 * d;
                p.z = v;
                return p;
            }
            
            float3 Sphere (float u, float v, float t) {
                float r = 0.9 + 0.1 * sin(PI * (12.0 * u + 8.0 * v + t));
                float s = r * cos(0.5 * PI * v);
                float3 p;
                p.x = s * sin(PI * u);
                p.y = r * sin(0.5 * PI * v);
                p.z = s * cos(PI * u);
                return p;
            }
            
            float3 Torus (float u, float v, float t) {
                float r1 = 0.7 + 0.1 * sin(PI * (8.0 * u + 0.5 * t));
                float r2 = 0.15 + 0.05 * sin(PI * (16.0 * u + 8.0 * v + 3.0 * t));
                float s = r2 * cos(PI * v) + r1;
                float3 p;
                p.x = s * sin(PI * u);
                p.y = r2 * sin(PI * v);
                p.z = s * cos(PI * u);
                return p;
            }

            float3 CalculatePosition(float u, float v, float t, int func)
            {
                if (func == 0) return Wave(u, v, t);
                if (func == 1) return MultiWave(u, v, t);
                if (func == 2) return Ripple(u, v, t);
                if (func == 3) return Sphere(u, v, t);
                if (func == 4) return Torus(u, v, t);
                return float3(u, 0, v); // Default
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);

                // Compute grid coordinates based on instance ID
                uint id = v.instanceID; // Get the instance ID
                float u = (id % _Resolution) * _Step - 1.0;  // x-coordinate
                float vCoord = (id / _Resolution) * _Step - 1.0; // y-coordinate

                // Compute position based on the function
                //float3 position = CalculatePosition(u, vCoord, _Time.y, _Function);
                float3 position;
                p.x = id;
                p.y = id;
                p.z = id;

                // Apply the position to the vertex
                o.vertex = UnityObjectToClipPos(float4(position, 1.0));
                o.color = float3(position * 0.5 + 0.5); // Encode position as color
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(i.color, 1.0);
            }
            ENDCG
        }
    }
}
