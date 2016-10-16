using System;
using UnityEngine;
using UnityEditor;

public class SpriteShaderGUI : ShaderGUI
{
	private static readonly string kShaderVertexLit = "Game/Sprite Vertex Lit";
	private static readonly string kShaderPixelLit = "Game/Sprite Pixel Lit";
	private static readonly string kShaderUnlit = "Game/Sprite Unlit";
	private static readonly int kSolidQueue = 2000;
	private static readonly int kAlphaTestQueue = 2450;
	private static readonly int kTransparentQueue = 3000;

	private enum eBlendMode
	{
		PreMultipliedAlpha,
		StandardAlpha,
		Solid,
		Additive,
		SoftAdditive,
		Multiply,
		Multiplyx2,
	};

	private enum eLightMode
	{
		VertexLit,
		PixelLit,
		Unlit,
	};

	private enum eCulling
	{
		Off = 0,
		Back = 2,
		Front = 1,
	};

	private MaterialProperty _mainTexture = null;
	private MaterialProperty _color = null;
	private MaterialProperty _blendMode = null;

	private MaterialProperty _emissionMap = null;
	private MaterialProperty _emissionColor = null;
	private MaterialProperty _emissionPower = null;

	private MaterialProperty _writeToDepth = null;
	private MaterialProperty _depthAlphaCutoff = null;
	private MaterialProperty _shadowAlphaCutoff = null;
	private MaterialProperty _renderQueue = null;
	private MaterialProperty _culling = null;

	private MaterialProperty _overlayColor = null;
	private MaterialProperty _hue = null;
	private MaterialProperty _saturation = null;
	private MaterialProperty _brightness = null;

	private MaterialProperty _rimPower = null;
	private MaterialProperty _rimColor = null;

	private MaterialEditor _materialEditor;

	//Normals
	private MaterialProperty _bumpMap = null;
	private MaterialProperty _diffuseRamp = null;
	private MaterialProperty _fixedNormal = null;

	//Blend texture
	private MaterialProperty _blendTexture = null;
	private MaterialProperty _blendTextureLerp = null;
	
	private bool _firstTimeApply = true;
	private eLightMode _lightMode;

	#region ShaderGUI
	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
	{
		FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
		_materialEditor = materialEditor;
		Material material = materialEditor.target as Material;

		ShaderPropertiesGUI(material);

		// Make sure that needed keywords are set up if we're switching some existing
		// material to a standard shader.
		if (_firstTimeApply)
		{
			SetMaterialKeywords(material);
			SetLightModeFromShader(material);
			_firstTimeApply = false;
		}
	}

	public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
	{
		base.AssignNewShaderToMaterial(material, oldShader, newShader);
		SetMaterialKeywords(material);
		SetLightModeFromShader(material);
	}
	#endregion

	#region Virtual Interface
	protected virtual void FindProperties(MaterialProperty[] props)
	{
		_mainTexture = FindProperty("_MainTex", props);
		_color = FindProperty("_Color", props);
		_blendMode = FindProperty("_BlendMode", props);

		_emissionMap = FindProperty("_EmissionMap", props, false);
		_emissionColor = FindProperty("_EmissionColor", props, false);
		_emissionPower = FindProperty("_EmissionPower", props, false);		

		_writeToDepth = FindProperty("_ZWrite", props);
		_depthAlphaCutoff = FindProperty("_Cutoff", props);
		_shadowAlphaCutoff = FindProperty("_ShadowAlphaCutoff", props);
		_renderQueue = FindProperty("_RenderQueue", props);
		_culling = FindProperty("_Cull", props);

		_bumpMap = FindProperty("_BumpMap", props, false);
		_diffuseRamp = FindProperty("_DiffuseRamp", props, false);
		_fixedNormal = FindProperty("_FixedNormal", props, false);
		_blendTexture = FindProperty("_BlendTex", props, false);
		_blendTextureLerp = FindProperty("_BlendAmount", props, false);

		_overlayColor = FindProperty("_OverlayColor", props, false);
		_hue = FindProperty("_Hue", props, false);
		_saturation = FindProperty("_Saturation", props, false);
		_brightness = FindProperty("_Brightness", props, false);

		_rimPower = FindProperty("_RimPower", props, false);
		_rimColor = FindProperty("_RimColor", props, false);
	}

	protected virtual void ShaderPropertiesGUI(Material material)
	{
		// Use default labelWidth
		EditorGUIUtility.labelWidth = 0f;

		// Detect any changes to the material
		EditorGUI.BeginChangeCheck();
		{
			GUILayout.Label("Modes", EditorStyles.boldLabel);
			{
				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderModes();

				EditorGUI.indentLevel = origIndent;
			}

			GUILayout.Label("Textures", EditorStyles.boldLabel);
			{
				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderTextureProperties(material);

				EditorGUI.indentLevel = origIndent;
			}

			GUILayout.Label("Sorting", EditorStyles.boldLabel);
			{
				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderSortingProperties(material);

				EditorGUI.indentLevel = origIndent;
			}

			GUILayout.Label("Depth", EditorStyles.boldLabel);
			{
				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderDepthProperties(material);

				EditorGUI.indentLevel = origIndent;
			}

			if (_fixedNormal != null)
			{
				GUILayout.Label("Normals", EditorStyles.boldLabel);

				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderNormalsProperties(material);

				EditorGUI.indentLevel = origIndent;
			}

			GUILayout.Label("Shadows", EditorStyles.boldLabel);
			{
				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderShadowsProperties(material);

				EditorGUI.indentLevel = origIndent;
			}

			GUILayout.Label("Color Adjustment", EditorStyles.boldLabel);
			{
				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderColorProperties(material);

				EditorGUI.indentLevel = origIndent;
			}

			if (_emissionMap != null && _emissionColor != null)
			{
				GUILayout.Label("Emission", EditorStyles.boldLabel);
				{
					int origIndent = EditorGUI.indentLevel;
					EditorGUI.indentLevel++;

					RenderEmissionProperties(material);

					EditorGUI.indentLevel = origIndent;
				}
			}

			if (_rimColor != null)
			{
				GUILayout.Label("Rim Lighting", EditorStyles.boldLabel);
				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderRimLightingProperties(material);

				EditorGUI.indentLevel = origIndent;
			}

			GUILayout.Label("Fog", EditorStyles.boldLabel);
			{
				int origIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;

				RenderFogProperties(material);

				EditorGUI.indentLevel = origIndent;
			}
		}

		if (EditorGUI.EndChangeCheck())
		{
			foreach (var obj in _blendMode.targets)
				MaterialChanged((Material)obj);
		}
	}

	protected virtual void RenderModes()
	{
		LightingModePopup();
		BlendModePopup();
	}

	protected virtual void RenderTextureProperties(Material material)
	{
		_materialEditor.TexturePropertySingleLine(new GUIContent("Main Texture"), _mainTexture, _color);

		if (_bumpMap != null)
			_materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), _bumpMap);

		if (_diffuseRamp != null)
			_materialEditor.TexturePropertySingleLine(new GUIContent("Diffuse Ramp"), _diffuseRamp);

		if (_blendTexture != null)
		{
			_materialEditor.TexturePropertySingleLine(new GUIContent("Blend Texture"), _blendTexture, _blendTextureLerp);
		}

		_materialEditor.TextureScaleOffsetProperty(_mainTexture);
	}

	protected virtual void RenderEmissionProperties(Material material)
	{
		bool emission = material.IsKeywordEnabled("_EMISSION");

		EditorGUI.BeginChangeCheck();
		emission = EditorGUILayout.Toggle("Emission", emission);
		if (EditorGUI.EndChangeCheck())
		{
			SetKeyword(material, "_EMISSION", emission);
		}

		if (emission)
		{
			_materialEditor.TexturePropertySingleLine(new GUIContent("Emission"), _emissionMap, _emissionColor);
			_materialEditor.FloatProperty(_emissionPower, "Emission Power");				
		}
	}

	protected virtual void RenderSortingProperties(Material material)
	{	
		EditorGUI.BeginChangeCheck();
		eCulling culling = (eCulling)Mathf.RoundToInt(_culling.floatValue);
		culling = (eCulling)EditorGUILayout.EnumPopup("Culling", culling);
		if (EditorGUI.EndChangeCheck())
		{
			material.SetInt("_Cull", (int)culling);
		}

		EditorGUI.BeginChangeCheck();
		int renderQueue = EditorGUILayout.IntSlider("Renderer Queue", (int)_renderQueue.floatValue, 0, 49);
		if (EditorGUI.EndChangeCheck())
		{
			material.SetInt("_RenderQueue", renderQueue);
		}
	}

	protected virtual void RenderDepthProperties(Material material)
	{
		EditorGUI.BeginChangeCheck();
		bool writeTodepth = EditorGUILayout.Toggle("Write to Depth", _writeToDepth.floatValue != 0.0f);
		if (EditorGUI.EndChangeCheck())
		{
			material.SetInt("_ZWrite", writeTodepth ? 1 : 0);
		}

		if (writeTodepth)
		{
			_materialEditor.RangeProperty(_depthAlphaCutoff, "Depth Alpha Cutoff");
		}
	}

	protected virtual void RenderNormalsProperties(Material material)
	{
		EditorGUI.BeginChangeCheck();
		bool fixedNormals = material.IsKeywordEnabled("_FIXED_NORMALS");
		bool fixedNormalsBackRendering = material.IsKeywordEnabled("_FIXED_NORMALS_BACK_RENDERING");

		bool meshNormals = EditorGUILayout.Toggle("Use Mesh Normals", !fixedNormals && !fixedNormalsBackRendering);
		if (EditorGUI.EndChangeCheck())
		{
			SetKeyword(material, "_FIXED_NORMALS", meshNormals ? false : fixedNormalsBackRendering ? false : true);
			SetKeyword(material, "_FIXED_NORMALS_BACK_RENDERING", meshNormals ? false : fixedNormalsBackRendering);
		}

		if (!meshNormals)
		{
			Vector3 normal = EditorGUILayout.Vector3Field("Normal", _fixedNormal.vectorValue);
			_fixedNormal.vectorValue = new Vector4(normal.x, normal.y, normal.z, 1.0f);

			EditorGUI.BeginChangeCheck();
			bool backRendering = EditorGUILayout.Toggle("Allow Back Rendering", material.IsKeywordEnabled("_FIXED_NORMALS_BACK_RENDERING"));
			if (EditorGUI.EndChangeCheck())
			{
				SetKeyword(material, "_FIXED_NORMALS_BACK_RENDERING", backRendering);
				SetKeyword(material, "_FIXED_NORMALS", !backRendering);
			}
		}
	}

	protected virtual void RenderShadowsProperties(Material material)
	{
		_materialEditor.FloatProperty(_shadowAlphaCutoff, "Shadow Alpha Cutoff");
	}

	protected virtual void RenderColorProperties(Material material)
	{
		EditorGUI.BeginChangeCheck();
		bool colorAdjust = EditorGUILayout.Toggle("Enable Color Adjustment", material.IsKeywordEnabled("_COLOR_ADJUST"));
		if (EditorGUI.EndChangeCheck())
		{
			SetKeyword(material, "_COLOR_ADJUST", colorAdjust);	
		}

		if (colorAdjust)
		{
			_materialEditor.ColorProperty(_overlayColor, "Overlay Color");
			_materialEditor.RangeProperty(_hue, "Hue");
			_materialEditor.RangeProperty(_saturation, "Saturation");
			_materialEditor.RangeProperty(_brightness, "Brightness");
		}
	}

	protected virtual void RenderRimLightingProperties(Material material)
	{
		EditorGUI.BeginChangeCheck();
		bool rimLighting = EditorGUILayout.Toggle("Enable Rim Lighting", material.IsKeywordEnabled("_RIM_LIGHTING"));
		if (EditorGUI.EndChangeCheck())
		{
			SetKeyword(material, "_RIM_LIGHTING", rimLighting);
		}

		if (rimLighting)
		{
			_materialEditor.ColorProperty(_rimColor, "Rim Color");
			_materialEditor.FloatProperty(_rimPower, "Rim Power");
		}
	}

	protected virtual void RenderFogProperties(Material material)
	{
		EditorGUI.BeginChangeCheck();
		bool fog = EditorGUILayout.Toggle("Enable Fog", material.IsKeywordEnabled("_FOG"));
		if (EditorGUI.EndChangeCheck())
		{
			SetKeyword(material, "_FOG", fog);
		}
	}
	#endregion

	private void SetLightModeFromShader(Material material)
	{
		if (material.shader.name == kShaderPixelLit)
		{
			_lightMode = eLightMode.PixelLit;
		}
		else if (material.shader.name == kShaderUnlit)
		{
			_lightMode = eLightMode.Unlit;
		}
		else
		{
			_lightMode = eLightMode.VertexLit;
		}
	}

	private void SetShaderFromLightMode()
	{
		Material material = _materialEditor.target as Material;

		switch (_lightMode)
		{
			case eLightMode.VertexLit:
				if (material.shader.name != kShaderVertexLit)
					_materialEditor.SetShader(Shader.Find(kShaderVertexLit), false);
				break;
			case eLightMode.PixelLit:
				if (material.shader.name != kShaderPixelLit)
					_materialEditor.SetShader(Shader.Find(kShaderPixelLit), false);
				break;
			case eLightMode.Unlit:
				if (material.shader.name != kShaderUnlit)
					_materialEditor.SetShader(Shader.Find(kShaderUnlit), false);
				break;
		}

		MaterialChanged(material);
	}

	private static void SetMaterialKeywords(Material material)
	{
		eBlendMode blendMode = (eBlendMode)material.GetFloat("_BlendMode");

		bool normalMap = material.HasProperty("_BumpMap") && material.GetTexture("_BumpMap") != null;
		SetKeyword (material, "_NORMALMAP", normalMap);

		bool zWrite = material.GetFloat("_ZWrite") > 0.0f;
		bool clipAlpha = zWrite && material.GetFloat("_Cutoff") > 0.0f;
		SetKeyword(material, "_ALPHA_CLIP", clipAlpha);

		bool diffuseRamp = material.HasProperty("_DiffuseRamp") && material.GetTexture("_DiffuseRamp") != null;
		SetKeyword(material, "_DIFFUSE_RAMP", diffuseRamp);

		bool blendTexture = material.HasProperty("_BlendTex") && material.GetTexture("_BlendTex") != null;
		SetKeyword(material, "_TEXTURE_BLEND", blendTexture);

		SetKeyword(material, "_ALPHAPREMULTIPLY_ON", blendMode == eBlendMode.PreMultipliedAlpha);
		SetKeyword(material, "_MULTIPLYBLEND", blendMode == eBlendMode.Multiply);
		SetKeyword(material, "_MULTIPLYBLEND_X2", blendMode == eBlendMode.Multiplyx2);
		SetKeyword(material, "_ADDITIVEBLEND", blendMode == eBlendMode.Additive);
		SetKeyword(material, "_ADDITIVEBLEND_SOFT", blendMode == eBlendMode.SoftAdditive);

		int renderQueue;

		switch (blendMode)
		{
			case eBlendMode.Solid:
				{
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					material.SetOverrideTag("RenderType", "Opaque");
					renderQueue = kSolidQueue;
				}
				break;
			case eBlendMode.Additive:
				{ 
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
					material.SetOverrideTag("RenderType", "Transparent");
					renderQueue = kTransparentQueue;
				}
				break;
			case eBlendMode.SoftAdditive:
				{
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor);
					material.SetOverrideTag("RenderType", "Transparent");
					renderQueue = kTransparentQueue;
				}			
				break;
			case eBlendMode.Multiply:
				{
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcColor);
					material.SetOverrideTag("RenderType", "Transparent");
					renderQueue = kTransparentQueue;
				}			
				break;
			case eBlendMode.Multiplyx2:
				{
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcColor);
					material.SetOverrideTag("RenderType", "Transparent");
					renderQueue = kTransparentQueue;
				}			
				break;
			case eBlendMode.PreMultipliedAlpha:
			case eBlendMode.StandardAlpha:
			default:
				{
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					material.SetOverrideTag("RenderType", zWrite ? "TransparentCutout" : "Transparent");
					renderQueue = zWrite ? kAlphaTestQueue : kTransparentQueue;
				}
				break;
		}
		
		material.renderQueue = renderQueue + material.GetInt("_RenderQueue");
	}

	private static void MaterialChanged(Material material)
	{		
		SetMaterialKeywords(material);
	}

	private static void SetKeyword(Material m, string keyword, bool state)
	{
		if (state)
			m.EnableKeyword (keyword);
		else
			m.DisableKeyword (keyword);
	}

	private void LightingModePopup()
	{
		EditorGUI.BeginChangeCheck();
		_lightMode = (eLightMode)EditorGUILayout.Popup("Lighting Mode", (int)_lightMode, Enum.GetNames(typeof(eLightMode)));
		if (EditorGUI.EndChangeCheck())
		{
			SetShaderFromLightMode();
		}
	}

	private void BlendModePopup()
	{
		eBlendMode mode = (eBlendMode)_blendMode.floatValue;
		EditorGUI.BeginChangeCheck();
		mode = (eBlendMode)EditorGUILayout.Popup("Blend Mode", (int)mode, Enum.GetNames(typeof(eBlendMode)));
		if (EditorGUI.EndChangeCheck())
		{
			_materialEditor.RegisterPropertyChangeUndo("Blend Mode");
			_blendMode.floatValue = (float)mode;
		}
	}
}
