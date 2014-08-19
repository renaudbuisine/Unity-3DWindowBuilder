Shader "Window/Display" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask ("Mask", 2D) = "white" {}
		_Border("Border",2D) = "white" {}
		_ColorContent("Color Content",Color) = (1,1,1,1)
		_Color("Color",Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" }
	    CGPROGRAM
	    #pragma surface surf Lambert alpha
	    struct Input {
	        float2 uv_MainTex;
            float2 uv_Mask;
            float2 uv_Border;
	    };
	    
	    uniform sampler2D _MainTex;
	    uniform sampler2D _Mask;
	    uniform sampler2D _Border;
	    uniform float4 _Color;
	    uniform float4 _ColorContent;
	    
	    void surf (Input IN, inout SurfaceOutput o) {
	        float3 mask = tex2D (_Mask, IN.uv_Mask).rgb;
	        float3 tex = tex2D (_MainTex, IN.uv_MainTex).rgb;
	        float4 border = tex2D (_Border, IN.uv_Border);
	        
	        tex *= mask * _ColorContent.rgb;
	        border.rgb *= _Color.rgb;
	        border.a *= _Color.a;
	     
	        if(mask.r == 1 && mask.g == 1 && mask.b == 1 && border.a < 1){
	        	o.Albedo = tex * _ColorContent.rgb;
	            o.Alpha = _ColorContent.a;
	        	if(border.a > 0){
	        		o.Albedo = o.Albedo * (1 - border.a) + border.rgb * border.a;
	        	}
	        }
	        else{
		        o.Albedo = border.rgb * _Color.rgb;
		        o.Alpha = border.a * _Color.a;
	        }
	    }
	    ENDCG
		
	}
}	