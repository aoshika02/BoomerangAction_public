Shader "Unlit/IrisWipe"
{
   Properties
    {
        s_MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _CircleRadius ("CircleRadius", Float) = 0.5
        _CenterX ("CenterX", Float) = 0.5
        _CenterY ("CenterY", Float) = 0.5
        _SmoothThickness ("SmoothThickness", Float) = 0.5

    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            float _CircleRadius;
            float _SmoothThickness;
            float _CenterX;
            float _CenterY;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 dir = i.uv - float2(_CenterX,_CenterY);
                dir.y *= _ScreenParams.y / _ScreenParams.x;
                return smoothstep(_CircleRadius, _CircleRadius+_SmoothThickness, length(dir))*_Color;
            }
            ENDCG
        }
    }
}