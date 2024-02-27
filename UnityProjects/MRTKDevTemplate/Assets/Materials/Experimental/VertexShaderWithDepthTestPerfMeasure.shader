// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/VertexShaderWithDepthTestPerfMeasure"
{
    Properties
    {
        // we have removed support for texture tiling/offset,
        // so make them not be displayed in material inspector
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _SubshaderToRun ("Subshader to run", Integer) = 0
    }
    SubShader
    {
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            // use "vert" function as the vertex shader
            #pragma vertex vert
            // use "frag" function as the pixel (fragment) shader
            #pragma fragment frag
            #pragma target 3.0

            #pragma enable_d3d11_debug_symbols

            // vertex shader inputs
            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                float2 uv : TEXCOORD0; // texture coordinate
                float4 vertex : SV_POSITION; // clip space position
                float testData : TEXCOORD1;
            };

            // texture we will sample
            sampler2D _MainTex;

            int _SubshaderToRun;

            float4 someExpensiveComputation1(float4 someValue, int repetitions, float uv)
            {
                float4 result = someValue;

                for (int i = 0; i < repetitions; i++)
                {
                    for (int j = 0; j < repetitions; j++)
                    {
                        for (int k = 0; k < repetitions; k++)
                        {
                            result = pow(i + j + k, 100);
                        }
                    }
                }

                return result;
            }

            float4 someExpensiveComputation2(float4 someValue, int repetitions, float uv)
            {
                float4 result = someValue;

                for (int i = 0; i < 1023; i++)
                {
                    result.xyz = tex2D (_MainTex, uv).rgb;
                }

                return result;
            }

            float4 someExpensiveComputation3(float4 someValue, int repetitions, float uv)
            {
                float4 result = someValue;

                for (int i = 0; i < repetitions; i++)
                {
                    for (int j = 0; j < repetitions; j++)
                    {
                        for (int k = 0; k < repetitions; k++)
                        {
                            result = sqrt(i + j + k);
                        }
                    }
                }

                return result;
            }

            // float myRandom(float val)
            // {
            //     float2 uv = float2(val, _Time);
            //     float2 noise = (frac(sin(dot(uv ,float2(12.9898,78.233)*2.0)) * 43758.5453));
            //     return abs(noise.x + noise.y) * 0.5f;
            // }
 
            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                // transform position to clip space
                // (multiply with model*view*projection matrix)
                o.vertex = UnityObjectToClipPos(v.vertex);
                //float4 temp = mul(unity_WorldToObject, v.vertex); //??
                float4 temp = v.vertex;
                o.testData = temp.z;

                return o;
            }

            // pixel shader; returns low precision ("fixed4" type)
            fixed4 frag (v2f i) : SV_Target
            {
                int repeat = 1000000000;
                //int repeat = 1023;
                fixed4 col = fixed4(0.1f, 0.1f, 0.1f, 1.0f);
                float4 someResult = float4(0.0f, 0.0f, 0.0f, 0.0f);

                //if (i.testData > 0.0f)
                {
                    someResult = someExpensiveComputation1(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                    //someResult = someExpensiveComputation2(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                    //someResult = someExpensiveComputation3(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                    someResult.x += 1;
                    float cycleDuration = 4.0f;
                    float timeStep = _Time.y % cycleDuration;
                    col = fixed4(timeStep / (cycleDuration - 1.0f), 0.0f, 0.0f, 0.5f);
                    //Call subshader for backplate here
                }
                // else if (i.testData < 0.0f)
                // {
                //     someResult = someExpensiveComputation1(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                //     //someResult = someExpensiveComputation2(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                //     //someResult = someExpensiveComputation3(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                //     someResult.x += 1;
                //     col = fixed4(0.0f, 0.5f, 0.0f, 0.5f);
                //     //Call subshader for backglow here
                // }
                // else
                // {
                //     someResult = someExpensiveComputation1(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                //     //someResult = someExpensiveComputation2(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                //     //someResult = someExpensiveComputation3(float4(1.0f, 1.0f, 1.0f, 1.0f), repeat, i.uv);
                //     someResult.x += 1;
                //     col = fixed4(0.0f, 0.0f, 0.5f, 0.5f);
                //     //Call subshader for frontplate here
                // }

                return col;
            }
            ENDCG
        }
    }
}
