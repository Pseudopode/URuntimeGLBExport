Shader "RoughnessToStandard"
{
	Properties{
	   _RoughnessMap("Texture Image", 2D) = "white" {}
	   _MetalnessMap("Texture Image", 2D) = "white" {}
	   //_MainTex1("Texture Image", 2D) = "white" {}
	   //_MainTex2("Texture Image", 2D) = "white" {}
	   //_MainTex3("Texture Image", 2D) = "white" {}
	
	}
		SubShader{
		   Pass {
			  CGPROGRAM

			  #pragma vertex vert  
			  #pragma fragment frag 

			  uniform sampler2D _RoughnessMap;
			  uniform sampler2D _MetalnessMap;
			/*	uniform sampler2D _MainTex1;
				uniform sampler2D _MainTex2;
				uniform sampler2D _MainTex3;*/

			 struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			 };
			 struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			 };

			 vertexOutput vert(vertexInput input)
			 {
				vertexOutput output;

				output.tex = input.texcoord;
				 output.pos = UnityObjectToClipPos(input.vertex);
				 return output;
			}
			float4 frag(vertexOutput input) : COLOR
			{
				float4 roughness = tex2D(_RoughnessMap, input.tex.xy);
				float4 metalness = tex2D(_MetalnessMap, input.tex.xy);
				float4 smoothness = 1 - roughness;
				metalness.a = smoothness.r;
			   return metalness;

			}

			ENDCG
			}
	}
		Fallback "Unlit/Texture"
}