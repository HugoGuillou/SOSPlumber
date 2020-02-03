Shader "SOS/Character"
{
	Properties
	{
		Vector1_579EDB93("Wobble Speed", Float) = 6
		Vector2_DEE48742("Wooble Intensity", Vector) = (0, 1, 0, 0)
		[NoScaleOffset]Texture2D_9BE8AD51("DoodleTexture", 2D) = "white" {}
		Color_9182375C("Color", Color) = (1, 1, 1, 0)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15
	}
		SubShader
		{
			Tags
			{
				"RenderPipeline" = "UniversalPipeline"
				"RenderType" = "Transparent"
				"Queue" = "Transparent+0"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			ColorMask[_ColorMask]

			Pass
			{
				Name "Pass"
				Tags
				{
			// LightMode: <None>
		}

		// Render State
		Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
		Cull Back
		ZTest LEqual
		ZWrite Off
			// ColorMask: <None>


			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			// Pragmas
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			#pragma multi_compile_instancing

			// Keywords
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma shader_feature _ _SAMPLE_GI
			// GraphKeywords: <None>

			// Defines
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _AlphaClip 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD0
			#define VARYINGS_NEED_TEXCOORD0
			#define SHADERPASS_UNLIT

			// Includes
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float Vector1_579EDB93;
			float2 Vector2_DEE48742;
			float4 Color_9182375C;
			CBUFFER_END
			TEXTURE2D(Texture2D_9BE8AD51); SAMPLER(samplerTexture2D_9BE8AD51); float4 Texture2D_9BE8AD51_TexelSize;
			SAMPLER(_SampleTexture2D_7E068DE1_Sampler_3_Linear_Repeat);

			// Graph Functions

			void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out) {
				Out = UV * Tiling + Offset;
			}

			void Unity_Multiply_float(float A, float B, out float Out) {
				Out = A * B;
			}

			void Unity_Floor_float(float In, out float Out) {
				Out = floor(In);
			}

			void Unity_Flipbook_float(float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out) {
				Tile = fmod(Tile, Width*Height);
				float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
				float tileY = abs(Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
				float tileX = abs(Invert.x * Width - ((Tile - Width * floor(Tile * tileCount.x)) + Invert.x * 1));
				Out = (UV + float2(tileX, tileY)) * tileCount;
			}


			float2 Unity_GradientNoise_Dir_float(float2 p) {
				// Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
				p = p % 289;
				float x = (34 * p.x + 1) * p.x % 289 + p.y;
				x = (34 * x + 1) * x % 289;
				x = frac(x / 41) * 2 - 1;
				return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
			}

			void Unity_GradientNoise_float(float2 UV, float Scale, out float Out) {
				float2 p = UV * Scale;
				float2 ip = floor(p);
				float2 fp = frac(p);
				float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
				float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
				float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
				float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
				fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
				Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
			}

			void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out) {
				Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			}

			void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG) {
				RGBA = float4(R, G, B, A);
				RGB = float3(R, G, B);
				RG = float2(R, G);
			}

			void Unity_Add_float2(float2 A, float2 B, out float2 Out) {
				Out = A + B;
			}

			void Unity_Multiply_float(float4 A, float4 B, out float4 Out) {
				Out = A * B;
			}

			// Graph Vertex
			// GraphVertex: <None>

			// Graph Pixel
			struct SurfaceDescriptionInputs {
				float4 uv0;
				float3 TimeParameters;
			};

			struct SurfaceDescription {
				float3 Color;
				float Alpha;
				float AlphaClipThreshold;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN) {
				SurfaceDescription surface = (SurfaceDescription)0;
				float4 _Property_64E7CA4D_Out_0 = Color_9182375C;
				float2 _TilingAndOffset_34358C3_Out_3;
				Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (0, 0), _TilingAndOffset_34358C3_Out_3);
				float _Property_3EA8C021_Out_0 = Vector1_579EDB93;
				float _Multiply_F63E45E3_Out_2;
				Unity_Multiply_float(IN.TimeParameters.x, _Property_3EA8C021_Out_0, _Multiply_F63E45E3_Out_2);
				float _Floor_D6097B84_Out_1;
				Unity_Floor_float(_Multiply_F63E45E3_Out_2, _Floor_D6097B84_Out_1);
				float2 _Flipbook_CA53EA71_Out_4;
				float2 _Flipbook_CA53EA71_Invert = float2 (0, 1);
				Unity_Flipbook_float(_TilingAndOffset_34358C3_Out_3, 2, 2, _Floor_D6097B84_Out_1, _Flipbook_CA53EA71_Invert, _Flipbook_CA53EA71_Out_4);
				float _GradientNoise_F3598870_Out_2;
				Unity_GradientNoise_float(_Flipbook_CA53EA71_Out_4, 3.66, _GradientNoise_F3598870_Out_2);
				float2 _Property_84D032A0_Out_0 = Vector2_DEE48742;
				float _Remap_4240E64A_Out_3;
				Unity_Remap_float(_GradientNoise_F3598870_Out_2, float2 (0, 1), _Property_84D032A0_Out_0, _Remap_4240E64A_Out_3);
				float2 _TilingAndOffset_5A15518C_Out_3;
				Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1.4, 1.4), float2 (0.4, 0.3), _TilingAndOffset_5A15518C_Out_3);
				float2 _Flipbook_11044427_Out_4;
				float2 _Flipbook_11044427_Invert = float2 (0, 1);
				Unity_Flipbook_float(_TilingAndOffset_5A15518C_Out_3, 2, 2, _Floor_D6097B84_Out_1, _Flipbook_11044427_Invert, _Flipbook_11044427_Out_4);
				float _GradientNoise_98CC94B1_Out_2;
				Unity_GradientNoise_float(_Flipbook_11044427_Out_4, 3.66, _GradientNoise_98CC94B1_Out_2);
				float _Remap_E44CF283_Out_3;
				Unity_Remap_float(_GradientNoise_98CC94B1_Out_2, float2 (0, 1), _Property_84D032A0_Out_0, _Remap_E44CF283_Out_3);
				float4 _Combine_85CD1BCB_RGBA_4;
				float3 _Combine_85CD1BCB_RGB_5;
				float2 _Combine_85CD1BCB_RG_6;
				Unity_Combine_float(_Remap_4240E64A_Out_3, _Remap_E44CF283_Out_3, 0, 0, _Combine_85CD1BCB_RGBA_4, _Combine_85CD1BCB_RGB_5, _Combine_85CD1BCB_RG_6);
				float2 _TilingAndOffset_40692F97_Out_3;
				Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (0, 0), _TilingAndOffset_40692F97_Out_3);
				float2 _Add_F7D31AC2_Out_2;
				Unity_Add_float2(_Combine_85CD1BCB_RG_6, _TilingAndOffset_40692F97_Out_3, _Add_F7D31AC2_Out_2);
				float4 _SampleTexture2D_7E068DE1_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_9BE8AD51, samplerTexture2D_9BE8AD51, _Add_F7D31AC2_Out_2);
				float _SampleTexture2D_7E068DE1_R_4 = _SampleTexture2D_7E068DE1_RGBA_0.r;
				float _SampleTexture2D_7E068DE1_G_5 = _SampleTexture2D_7E068DE1_RGBA_0.g;
				float _SampleTexture2D_7E068DE1_B_6 = _SampleTexture2D_7E068DE1_RGBA_0.b;
				float _SampleTexture2D_7E068DE1_A_7 = _SampleTexture2D_7E068DE1_RGBA_0.a;
				float4 _Multiply_26645993_Out_2;
				Unity_Multiply_float(_Property_64E7CA4D_Out_0, _SampleTexture2D_7E068DE1_RGBA_0, _Multiply_26645993_Out_2);
				surface.Color = (_Multiply_26645993_Out_2.xyz);
				surface.Alpha = _SampleTexture2D_7E068DE1_A_7;
				surface.AlphaClipThreshold = 0.5;
				return surface;
			}

			// --------------------------------------------------
			// Structs and Packing

			// Generated Type: Attributes
			struct Attributes {
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};

			// Generated Type: Varyings
			struct Varyings {
				float4 positionCS : SV_Position;
				float4 texCoord0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
			};

			// Generated Type: PackedVaryings
			struct PackedVaryings {
				float4 positionCS : SV_Position;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				float4 interp00 : TEXCOORD0;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			// Packed Type: Varyings
			PackedVaryings PackVaryings(Varyings input) {
				PackedVaryings output;
				output.positionCS = input.positionCS;
				output.interp00.xyzw = input.texCoord0;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				return output;
			}

			// Unpacked Type: Varyings
			Varyings UnpackVaryings(PackedVaryings input) {
				Varyings output;
				output.positionCS = input.positionCS;
				output.texCoord0 = input.interp00.xyzw;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				return output;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input) {
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				output.uv0 = input.texCoord0;
				output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
			#else
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
			#endif
			#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

				return output;
			}


			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

				// Render State
				Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
				Cull Back
				ZTest LEqual
				ZWrite On
				// ColorMask: <None>


				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				// Debug
				// <None>

				// --------------------------------------------------
				// Pass

				// Pragmas
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0
				#pragma multi_compile_instancing

				// Keywords
				#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				// GraphKeywords: <None>

				// Defines
				#define _SURFACE_TYPE_TRANSPARENT 1
				#define _AlphaClip 1
				#define ATTRIBUTES_NEED_NORMAL
				#define ATTRIBUTES_NEED_TANGENT
				#define ATTRIBUTES_NEED_TEXCOORD0
				#define VARYINGS_NEED_TEXCOORD0
				#define SHADERPASS_SHADOWCASTER

				// Includes
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

				// --------------------------------------------------
				// Graph

				// Graph Properties
				CBUFFER_START(UnityPerMaterial)
				float Vector1_579EDB93;
				float2 Vector2_DEE48742;
				float4 Color_9182375C;
				CBUFFER_END
				TEXTURE2D(Texture2D_9BE8AD51); SAMPLER(samplerTexture2D_9BE8AD51); float4 Texture2D_9BE8AD51_TexelSize;
				SAMPLER(_SampleTexture2D_7E068DE1_Sampler_3_Linear_Repeat);

				// Graph Functions

				void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out) {
					Out = UV * Tiling + Offset;
				}

				void Unity_Multiply_float(float A, float B, out float Out) {
					Out = A * B;
				}

				void Unity_Floor_float(float In, out float Out) {
					Out = floor(In);
				}

				void Unity_Flipbook_float(float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out) {
					Tile = fmod(Tile, Width*Height);
					float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
					float tileY = abs(Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
					float tileX = abs(Invert.x * Width - ((Tile - Width * floor(Tile * tileCount.x)) + Invert.x * 1));
					Out = (UV + float2(tileX, tileY)) * tileCount;
				}


				float2 Unity_GradientNoise_Dir_float(float2 p) {
					// Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
					p = p % 289;
					float x = (34 * p.x + 1) * p.x % 289 + p.y;
					x = (34 * x + 1) * x % 289;
					x = frac(x / 41) * 2 - 1;
					return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
				}

				void Unity_GradientNoise_float(float2 UV, float Scale, out float Out) {
					float2 p = UV * Scale;
					float2 ip = floor(p);
					float2 fp = frac(p);
					float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
					float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
					float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
					float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
					fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
					Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
				}

				void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out) {
					Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
				}

				void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG) {
					RGBA = float4(R, G, B, A);
					RGB = float3(R, G, B);
					RG = float2(R, G);
				}

				void Unity_Add_float2(float2 A, float2 B, out float2 Out) {
					Out = A + B;
				}

				// Graph Vertex
				// GraphVertex: <None>

				// Graph Pixel
				struct SurfaceDescriptionInputs {
					float4 uv0;
					float3 TimeParameters;
				};

				struct SurfaceDescription {
					float Alpha;
					float AlphaClipThreshold;
				};

				SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN) {
					SurfaceDescription surface = (SurfaceDescription)0;
					float2 _TilingAndOffset_34358C3_Out_3;
					Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (0, 0), _TilingAndOffset_34358C3_Out_3);
					float _Property_3EA8C021_Out_0 = Vector1_579EDB93;
					float _Multiply_F63E45E3_Out_2;
					Unity_Multiply_float(IN.TimeParameters.x, _Property_3EA8C021_Out_0, _Multiply_F63E45E3_Out_2);
					float _Floor_D6097B84_Out_1;
					Unity_Floor_float(_Multiply_F63E45E3_Out_2, _Floor_D6097B84_Out_1);
					float2 _Flipbook_CA53EA71_Out_4;
					float2 _Flipbook_CA53EA71_Invert = float2 (0, 1);
					Unity_Flipbook_float(_TilingAndOffset_34358C3_Out_3, 2, 2, _Floor_D6097B84_Out_1, _Flipbook_CA53EA71_Invert, _Flipbook_CA53EA71_Out_4);
					float _GradientNoise_F3598870_Out_2;
					Unity_GradientNoise_float(_Flipbook_CA53EA71_Out_4, 3.66, _GradientNoise_F3598870_Out_2);
					float2 _Property_84D032A0_Out_0 = Vector2_DEE48742;
					float _Remap_4240E64A_Out_3;
					Unity_Remap_float(_GradientNoise_F3598870_Out_2, float2 (0, 1), _Property_84D032A0_Out_0, _Remap_4240E64A_Out_3);
					float2 _TilingAndOffset_5A15518C_Out_3;
					Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1.4, 1.4), float2 (0.4, 0.3), _TilingAndOffset_5A15518C_Out_3);
					float2 _Flipbook_11044427_Out_4;
					float2 _Flipbook_11044427_Invert = float2 (0, 1);
					Unity_Flipbook_float(_TilingAndOffset_5A15518C_Out_3, 2, 2, _Floor_D6097B84_Out_1, _Flipbook_11044427_Invert, _Flipbook_11044427_Out_4);
					float _GradientNoise_98CC94B1_Out_2;
					Unity_GradientNoise_float(_Flipbook_11044427_Out_4, 3.66, _GradientNoise_98CC94B1_Out_2);
					float _Remap_E44CF283_Out_3;
					Unity_Remap_float(_GradientNoise_98CC94B1_Out_2, float2 (0, 1), _Property_84D032A0_Out_0, _Remap_E44CF283_Out_3);
					float4 _Combine_85CD1BCB_RGBA_4;
					float3 _Combine_85CD1BCB_RGB_5;
					float2 _Combine_85CD1BCB_RG_6;
					Unity_Combine_float(_Remap_4240E64A_Out_3, _Remap_E44CF283_Out_3, 0, 0, _Combine_85CD1BCB_RGBA_4, _Combine_85CD1BCB_RGB_5, _Combine_85CD1BCB_RG_6);
					float2 _TilingAndOffset_40692F97_Out_3;
					Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (0, 0), _TilingAndOffset_40692F97_Out_3);
					float2 _Add_F7D31AC2_Out_2;
					Unity_Add_float2(_Combine_85CD1BCB_RG_6, _TilingAndOffset_40692F97_Out_3, _Add_F7D31AC2_Out_2);
					float4 _SampleTexture2D_7E068DE1_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_9BE8AD51, samplerTexture2D_9BE8AD51, _Add_F7D31AC2_Out_2);
					float _SampleTexture2D_7E068DE1_R_4 = _SampleTexture2D_7E068DE1_RGBA_0.r;
					float _SampleTexture2D_7E068DE1_G_5 = _SampleTexture2D_7E068DE1_RGBA_0.g;
					float _SampleTexture2D_7E068DE1_B_6 = _SampleTexture2D_7E068DE1_RGBA_0.b;
					float _SampleTexture2D_7E068DE1_A_7 = _SampleTexture2D_7E068DE1_RGBA_0.a;
					surface.Alpha = _SampleTexture2D_7E068DE1_A_7;
					surface.AlphaClipThreshold = 0.5;
					return surface;
				}

				// --------------------------------------------------
				// Structs and Packing

				// Generated Type: Attributes
				struct Attributes {
					float3 positionOS : POSITION;
					float3 normalOS : NORMAL;
					float4 tangentOS : TANGENT;
					float4 uv0 : TEXCOORD0;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				};

				// Generated Type: Varyings
				struct Varyings {
					float4 positionCS : SV_Position;
					float4 texCoord0;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				};

				// Generated Type: PackedVaryings
				struct PackedVaryings {
					float4 positionCS : SV_Position;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
					float4 interp00 : TEXCOORD0;
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				};

				// Packed Type: Varyings
				PackedVaryings PackVaryings(Varyings input) {
					PackedVaryings output;
					output.positionCS = input.positionCS;
					output.interp00.xyzw = input.texCoord0;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					return output;
				}

				// Unpacked Type: Varyings
				Varyings UnpackVaryings(PackedVaryings input) {
					Varyings output;
					output.positionCS = input.positionCS;
					output.texCoord0 = input.interp00.xyzw;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					return output;
				}

				// --------------------------------------------------
				// Build Graph Inputs

				SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input) {
					SurfaceDescriptionInputs output;
					ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

					output.uv0 = input.texCoord0;
					output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
				#else
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				#endif
				#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

					return output;
				}


				// --------------------------------------------------
				// Main

				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

				ENDHLSL
			}

			Pass
			{
				Name "DepthOnly"
				Tags
				{
					"LightMode" = "DepthOnly"
				}

					// Render State
					Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
					Cull Back
					ZTest LEqual
					ZWrite On
					ColorMask 0


					HLSLPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					// Debug
					// <None>

					// --------------------------------------------------
					// Pass

					// Pragmas
					#pragma prefer_hlslcc gles
					#pragma exclude_renderers d3d11_9x
					#pragma target 2.0
					#pragma multi_compile_instancing

					// Keywords
					// PassKeywords: <None>
					// GraphKeywords: <None>

					// Defines
					#define _SURFACE_TYPE_TRANSPARENT 1
					#define _AlphaClip 1
					#define ATTRIBUTES_NEED_NORMAL
					#define ATTRIBUTES_NEED_TANGENT
					#define ATTRIBUTES_NEED_TEXCOORD0
					#define VARYINGS_NEED_TEXCOORD0
					#define SHADERPASS_DEPTHONLY

					// Includes
					#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

					// --------------------------------------------------
					// Graph

					// Graph Properties
					CBUFFER_START(UnityPerMaterial)
					float Vector1_579EDB93;
					float2 Vector2_DEE48742;
					float4 Color_9182375C;
					CBUFFER_END
					TEXTURE2D(Texture2D_9BE8AD51); SAMPLER(samplerTexture2D_9BE8AD51); float4 Texture2D_9BE8AD51_TexelSize;
					SAMPLER(_SampleTexture2D_7E068DE1_Sampler_3_Linear_Repeat);

					// Graph Functions

					void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out) {
						Out = UV * Tiling + Offset;
					}

					void Unity_Multiply_float(float A, float B, out float Out) {
						Out = A * B;
					}

					void Unity_Floor_float(float In, out float Out) {
						Out = floor(In);
					}

					void Unity_Flipbook_float(float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out) {
						Tile = fmod(Tile, Width*Height);
						float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
						float tileY = abs(Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
						float tileX = abs(Invert.x * Width - ((Tile - Width * floor(Tile * tileCount.x)) + Invert.x * 1));
						Out = (UV + float2(tileX, tileY)) * tileCount;
					}


					float2 Unity_GradientNoise_Dir_float(float2 p) {
						// Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
						p = p % 289;
						float x = (34 * p.x + 1) * p.x % 289 + p.y;
						x = (34 * x + 1) * x % 289;
						x = frac(x / 41) * 2 - 1;
						return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
					}

					void Unity_GradientNoise_float(float2 UV, float Scale, out float Out) {
						float2 p = UV * Scale;
						float2 ip = floor(p);
						float2 fp = frac(p);
						float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
						float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
						float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
						float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
						fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
						Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
					}

					void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out) {
						Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
					}

					void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG) {
						RGBA = float4(R, G, B, A);
						RGB = float3(R, G, B);
						RG = float2(R, G);
					}

					void Unity_Add_float2(float2 A, float2 B, out float2 Out) {
						Out = A + B;
					}

					// Graph Vertex
					// GraphVertex: <None>

					// Graph Pixel
					struct SurfaceDescriptionInputs {
						float4 uv0;
						float3 TimeParameters;
					};

					struct SurfaceDescription {
						float Alpha;
						float AlphaClipThreshold;
					};

					SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN) {
						SurfaceDescription surface = (SurfaceDescription)0;
						float2 _TilingAndOffset_34358C3_Out_3;
						Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (0, 0), _TilingAndOffset_34358C3_Out_3);
						float _Property_3EA8C021_Out_0 = Vector1_579EDB93;
						float _Multiply_F63E45E3_Out_2;
						Unity_Multiply_float(IN.TimeParameters.x, _Property_3EA8C021_Out_0, _Multiply_F63E45E3_Out_2);
						float _Floor_D6097B84_Out_1;
						Unity_Floor_float(_Multiply_F63E45E3_Out_2, _Floor_D6097B84_Out_1);
						float2 _Flipbook_CA53EA71_Out_4;
						float2 _Flipbook_CA53EA71_Invert = float2 (0, 1);
						Unity_Flipbook_float(_TilingAndOffset_34358C3_Out_3, 2, 2, _Floor_D6097B84_Out_1, _Flipbook_CA53EA71_Invert, _Flipbook_CA53EA71_Out_4);
						float _GradientNoise_F3598870_Out_2;
						Unity_GradientNoise_float(_Flipbook_CA53EA71_Out_4, 3.66, _GradientNoise_F3598870_Out_2);
						float2 _Property_84D032A0_Out_0 = Vector2_DEE48742;
						float _Remap_4240E64A_Out_3;
						Unity_Remap_float(_GradientNoise_F3598870_Out_2, float2 (0, 1), _Property_84D032A0_Out_0, _Remap_4240E64A_Out_3);
						float2 _TilingAndOffset_5A15518C_Out_3;
						Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1.4, 1.4), float2 (0.4, 0.3), _TilingAndOffset_5A15518C_Out_3);
						float2 _Flipbook_11044427_Out_4;
						float2 _Flipbook_11044427_Invert = float2 (0, 1);
						Unity_Flipbook_float(_TilingAndOffset_5A15518C_Out_3, 2, 2, _Floor_D6097B84_Out_1, _Flipbook_11044427_Invert, _Flipbook_11044427_Out_4);
						float _GradientNoise_98CC94B1_Out_2;
						Unity_GradientNoise_float(_Flipbook_11044427_Out_4, 3.66, _GradientNoise_98CC94B1_Out_2);
						float _Remap_E44CF283_Out_3;
						Unity_Remap_float(_GradientNoise_98CC94B1_Out_2, float2 (0, 1), _Property_84D032A0_Out_0, _Remap_E44CF283_Out_3);
						float4 _Combine_85CD1BCB_RGBA_4;
						float3 _Combine_85CD1BCB_RGB_5;
						float2 _Combine_85CD1BCB_RG_6;
						Unity_Combine_float(_Remap_4240E64A_Out_3, _Remap_E44CF283_Out_3, 0, 0, _Combine_85CD1BCB_RGBA_4, _Combine_85CD1BCB_RGB_5, _Combine_85CD1BCB_RG_6);
						float2 _TilingAndOffset_40692F97_Out_3;
						Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (0, 0), _TilingAndOffset_40692F97_Out_3);
						float2 _Add_F7D31AC2_Out_2;
						Unity_Add_float2(_Combine_85CD1BCB_RG_6, _TilingAndOffset_40692F97_Out_3, _Add_F7D31AC2_Out_2);
						float4 _SampleTexture2D_7E068DE1_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_9BE8AD51, samplerTexture2D_9BE8AD51, _Add_F7D31AC2_Out_2);
						float _SampleTexture2D_7E068DE1_R_4 = _SampleTexture2D_7E068DE1_RGBA_0.r;
						float _SampleTexture2D_7E068DE1_G_5 = _SampleTexture2D_7E068DE1_RGBA_0.g;
						float _SampleTexture2D_7E068DE1_B_6 = _SampleTexture2D_7E068DE1_RGBA_0.b;
						float _SampleTexture2D_7E068DE1_A_7 = _SampleTexture2D_7E068DE1_RGBA_0.a;
						surface.Alpha = _SampleTexture2D_7E068DE1_A_7;
						surface.AlphaClipThreshold = 0.5;
						return surface;
					}

					// --------------------------------------------------
					// Structs and Packing

					// Generated Type: Attributes
					struct Attributes {
						float3 positionOS : POSITION;
						float3 normalOS : NORMAL;
						float4 tangentOS : TANGENT;
						float4 uv0 : TEXCOORD0;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : INSTANCEID_SEMANTIC;
						#endif
					};

					// Generated Type: Varyings
					struct Varyings {
						float4 positionCS : SV_Position;
						float4 texCoord0;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : CUSTOM_INSTANCE_ID;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
						#endif
					};

					// Generated Type: PackedVaryings
					struct PackedVaryings {
						float4 positionCS : SV_Position;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : CUSTOM_INSTANCE_ID;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
						#endif
						float4 interp00 : TEXCOORD0;
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
						#endif
					};

					// Packed Type: Varyings
					PackedVaryings PackVaryings(Varyings input) {
						PackedVaryings output;
						output.positionCS = input.positionCS;
						output.interp00.xyzw = input.texCoord0;
						#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
						#endif
						return output;
					}

					// Unpacked Type: Varyings
					Varyings UnpackVaryings(PackedVaryings input) {
						Varyings output;
						output.positionCS = input.positionCS;
						output.texCoord0 = input.interp00.xyzw;
						#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
						#endif
						return output;
					}

					// --------------------------------------------------
					// Build Graph Inputs

					SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input) {
						SurfaceDescriptionInputs output;
						ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

						output.uv0 = input.texCoord0;
						output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
					#else
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
					#endif
					#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

						return output;
					}


					// --------------------------------------------------
					// Main

					#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

					ENDHLSL
				}

		}
			FallBack "Hidden/Shader Graph/FallbackError"
}
