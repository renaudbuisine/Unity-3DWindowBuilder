Shader "Window/ScrollBar" {
	Properties {
		_MainTex ("Base (RGB) Alpha(A)", 2D) = "white" {}
		_AlphaTest ("AlphaTest",Range(0,1)) = 0.1
	}
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		AlphaTest Greater [_AlphaTest]

		Pass{
			SetTexture [_MainTex] { combine texture }
		}
	}
}
