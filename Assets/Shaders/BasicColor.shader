Shader "Custom/BasicColor"
{
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
    }

    SubShader{
        Pass {
            Fog {Mode Off}
            Lighting Off
            SetTexture[_MainTex] {
                constantColor[_Color]
                Combine previous * constant
            }
        }
    }
}
