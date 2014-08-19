Shader "Window/Mask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha ("Mask", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		AlphaTest Greater 0.3
		Lighting off
		
		Pass{
			SetTexture[_MainTex] {
				constantColor [_Color]
		        combine constant * texture
		    }

		    SetTexture[_Alpha] {
		        Combine previous * texture
		    }
		}
	}
}	