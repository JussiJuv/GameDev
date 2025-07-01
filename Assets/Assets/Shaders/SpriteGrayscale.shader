Shader "Custom/SpriteGrayscale"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _GrayAmount("Gray Amount", Range(0,1)) = 1
        _Color("Tint", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            LOD 100

            Pass
            {
                Cull Off
                Lighting Off
                ZWrite Off
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t { float4 vertex : POSITION; float4 color : COLOR; float2 texcoord : TEXCOORD0; };
                struct v2f { float4 vertex : SV_POSITION; fixed4 color : COLOR; float2 texcoord : TEXCOORD0; };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _Color;
                float _GrayAmount;

                v2f vert(appdata_t IN)
                {
                    v2f OUT;
                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                    OUT.color = IN.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                // luminance
                fixed gray = dot(c.rgb, fixed3(0.299, 0.587, 0.114));
                // lerp between gray and original
                c.rgb = lerp(gray.xxx, c.rgb, 1 - _GrayAmount);
                return c;
            }
            ENDCG
        }
        }
}
