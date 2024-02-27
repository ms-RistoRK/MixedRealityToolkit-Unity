// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SimpleUnlitTexturedShader"
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
        Pass
        {
            CGPROGRAM
            // use "vert" function as the vertex shader
            #pragma vertex vert
            // use "frag" function as the pixel (fragment) shader
            #pragma fragment frag

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
            };

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                // transform position to clip space
                // (multiply with model*view*projection matrix)
                o.vertex = UnityObjectToClipPos(v.vertex);
                // just pass the texture coordinate
                o.uv = v.uv;
                return o;
            }
            
            // texture we will sample
            sampler2D _MainTex;

            int _SubshaderToRun;

            // pixel shader; returns low precision ("fixed4" type)
            // color ("SV_Target" semantic)
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0.0f, 0.0f, 0.0f, 1.0f);
                if (_SubshaderToRun == 0)
                {
                    col = fixed4(1.0f, 0.0f, 0.0f, 1.0f);
                    //Call subshader for backplate here
                }
                else if (_SubshaderToRun == 1)
                {
                    col = fixed4(0.0f, 1.0f, 0.0f, 1.0f);
                    //Call subshader for backglow here
                }
                else if (_SubshaderToRun == 2)
                {
                    col = fixed4(0.0f, 0.0f, 1.0f, 1.0f);
                    //Call subshader for frontplate here
                }
                return col;
            }
            ENDCG
        }
    }
}
