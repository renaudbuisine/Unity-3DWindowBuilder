Shader "Window/Interface" {
	Properties {
		_MainTex ("Base (RGB) Alpha(A)", 2D) = "white" {}
		_AlphaTest ("AlphaTest",Range(0,1)) = 0.1
	}
	SubShader {
		Tags { "Queue"="AlphaTest" "Queue"="Transparent" "RenderType"="Transparent" }
		AlphaTest Greater [_AlphaTest]
		Lighting off
		// Render both front and back facing polygons.
		//Cull Off	

		Pass{
			SetTexture [_MainTex] { combine texture }
		}
	}
}
