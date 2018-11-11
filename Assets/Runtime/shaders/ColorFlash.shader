Shader "Custom/ColorFlash" {
  Properties{
    [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
    _Color("Tint", Color) = (1,1,1,1)
    _FlashColor("Flash Color", Color) = (1,1,1,1)
    _FlashAmount("Flash Amount",Range(0.0,1.0)) = 0.0
  }
  
  SubShader {
    Tags {
      "Queue" = "Transparent"
      "IgnoreProjector" = "True"
      "RenderType" = "Transparent"
      "PreviewType" = "Plane"
      "CanUseSpriteAtlas" = "True"
    }

    Cull Off
    Lighting Off
    ZWrite Off
    Blend One OneMinusSrcAlpha

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float2 uv : TEXCOORD0;
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        float4 color : COLOR;
        float2 uv : TEXCOORD0;
      };

      fixed4 _Color;

      v2f vert(appdata v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        o.color = v.color * _Color;
        return o;
      }

      sampler2D _MainTex;
      fixed4 _FlashColor;
      float  _FlashAmount;

      fixed4 frag(v2f i) : COLOR{
        fixed4 c = tex2D(_MainTex, i.uv) * i.color;
        c.rgb = lerp(c.rgb, _FlashColor.rgb, _FlashAmount);
        c.rgb *= c.a;
        return c;
      }

      ENDCG
    }
  }
}