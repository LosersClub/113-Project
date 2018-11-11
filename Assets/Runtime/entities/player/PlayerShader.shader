// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PlayerShader" {
  Properties {
    [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
    _Color("Tint", Color) = (1,1,1,1)
    _TargetColor("Original Eye Color", Color) = (1, 1, 0)
    _OutputColor("New Eye Color", Color) = (1, 1, 1)
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

      fixed3 bump3(fixed3 b, fixed3 p) {
        fixed3 diff = abs(b - p);
        return max(0.0, 1.0 - 255.0*(diff.x + diff.y + diff.z));
      }

      sampler2D _MainTex;
      fixed4 _TargetColor;
      fixed4 _OutputColor;

      fixed4 frag(v2f i) : SV_Target {
        fixed4 x = tex2D(_MainTex, i.uv);
        x.rgb = x.rgb + bump3(_TargetColor, x.rgb) * (_OutputColor - _TargetColor);
        x.rgb *= i.color * x.a;
        return x;
      }
      ENDCG
    }
  }
}