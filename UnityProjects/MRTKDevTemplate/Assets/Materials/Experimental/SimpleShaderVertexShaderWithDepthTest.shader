// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SimpleShaderVertexShaderWithDepthTest"
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
            
            // texture we will sample
            sampler2D _MainTex;

            int _SubshaderToRun;

            // pixel shader; returns low precision ("fixed4" type)
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0.1f, 0.1f, 0.1f, 1.0f);

                if (i.testData > 0.0f)
                {
                    col = fixed4(0.5f, 0.5f, 0.0f, 0.5f);
                    //Call subshader for backplate here
                }
                else if (i.testData < 0.0f)
                {
                    col = fixed4(0.0f, 0.5f, 0.5f, 0.5f);
                    //Call subshader for backglow here
                }
                else
                {
                    col = fixed4(0.5f, 0.0f, 0.0f, 0.5f);
                    //Call subshader for frontplate here
                }

                return col;
            }
            ENDCG
        }
    }
}
