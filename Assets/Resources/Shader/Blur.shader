Shader "UI/PanelBlur"
{
    Properties
    {
        _Size ("Blur Size", Float) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        GrabPass { }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            float _Size;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 grabUV : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = o.vertex.xy;
                o.grabUV = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float blur = _Size * 0.001;
                fixed4 col = tex2Dproj(_GrabTexture, i.grabUV);
                col += tex2Dproj(_GrabTexture, i.grabUV + float4( blur,  0, 0, 0));
                col += tex2Dproj(_GrabTexture, i.grabUV + float4(-blur,  0, 0, 0));
                col += tex2Dproj(_GrabTexture, i.grabUV + float4( 0,  blur, 0, 0));
                col += tex2Dproj(_GrabTexture, i.grabUV + float4( 0, -blur, 0, 0));
                return col / 5;
            }
            ENDCG
        }
    }
}
