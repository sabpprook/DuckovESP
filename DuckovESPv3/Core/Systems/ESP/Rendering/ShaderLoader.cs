using System;
using System.IO;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Rendering
{
    /// <summary>
    /// Shader加载器 - 运行时动态创建Shader（无需Unity编辑器）
    /// </summary>
    public static class ShaderLoader
    {
        private static Shader? _tmpAlwaysOnTopShader;
        private static Shader? _uiAlwaysOnTopShader;
        private static bool _initialized = false;

        /// <summary>
        /// 获取TMP AlwaysOnTop Shader
        /// </summary>
        public static Shader? GetTMPShader()
        {
            EnsureInitialized();
            return _tmpAlwaysOnTopShader;
        }

        /// <summary>
        /// 获取UI AlwaysOnTop Shader
        /// </summary>
        public static Shader? GetUIShader()
        {
            EnsureInitialized();
            return _uiAlwaysOnTopShader;
        }

        /// <summary>
        /// 初始化Shader加载
        /// </summary>
        private static void EnsureInitialized()
        {
            if (_initialized)
                return;

            _initialized = true;

            try
            {
                // 方案1: 尝试从AssetBundle加载（如果用户提供了）
                string modPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string bundlePath = Path.Combine(modPath, "espshaders");

                if (File.Exists(bundlePath))
                {
                    AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
                    if (bundle != null)
                    {
                        _tmpAlwaysOnTopShader = bundle.LoadAsset<Shader>("TMPro_AlwaysOnTop");
                        _uiAlwaysOnTopShader = bundle.LoadAsset<Shader>("UI_AlwaysOnTop");
                        
                        if (_tmpAlwaysOnTopShader != null || _uiAlwaysOnTopShader != null)
                        {
                            Debug.Log($"[ShaderLoader] 从AssetBundle加载Shader成功");
                            return;
                        }
                    }
                }

                // 方案2: 运行时动态创建Shader（无需Unity编辑器）
                Debug.Log("[ShaderLoader] 开始运行时创建Shader...");
                _tmpAlwaysOnTopShader = CreateTMPShaderAtRuntime();
                _uiAlwaysOnTopShader = CreateUIShaderAtRuntime();

                if (_tmpAlwaysOnTopShader != null || _uiAlwaysOnTopShader != null)
                {
                    Debug.Log("[ShaderLoader] 运行时创建Shader成功");
                    return;
                }

                Debug.LogWarning("[ShaderLoader] 无法创建Shader，将使用Fallback方案（材质修改）");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ShaderLoader] 加载Shader失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 运行时创建TMP Shader
        /// </summary>
        private static Shader? CreateTMPShaderAtRuntime()
        {
            try
            {
                // TMP Shader代码（使用CG/HLSL）
                string shaderCode = @"
Shader ""Custom/Runtime_TMPro_AlwaysOnTop""
{
    Properties
    {
        [HDR]_FaceColor(""Face Color"", Color) = (1,1,1,1)
        _MainTex(""Main Texture"", 2D) = ""white"" {}
    }
    
    SubShader
    {
        Tags { ""Queue""=""Overlay+1"" ""RenderType""=""Transparent"" }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include ""UnityCG.cginc""
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _FaceColor;
            
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _FaceColor;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                fixed4 color = i.color;
                color.a *= texColor.a;
                return color;
            }
            ENDCG
        }
    }
    
    Fallback ""UI/Default""
}
";
                return Shader.Find("Custom/Runtime_TMPro_AlwaysOnTop") ?? 
                       CreateShaderFromString(shaderCode, "Custom/Runtime_TMPro_AlwaysOnTop");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ShaderLoader] 创建TMP Shader失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 运行时创建UI Shader
        /// </summary>
        private static Shader? CreateUIShaderAtRuntime()
        {
            try
            {
                string shaderCode = @"
Shader ""Custom/Runtime_UI_AlwaysOnTop""
{
    Properties
    {
        [PerRendererData] _MainTex (""Sprite Texture"", 2D) = ""white"" {}
        _Color (""Tint"", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { ""Queue""=""Overlay"" ""RenderType""=""Transparent"" }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include ""UnityCG.cginc""
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.texcoord) * i.color;
                return color;
            }
            ENDCG
        }
    }
    
    Fallback ""UI/Default""
}
";
                return Shader.Find("Custom/Runtime_UI_AlwaysOnTop") ?? 
                       CreateShaderFromString(shaderCode, "Custom/Runtime_UI_AlwaysOnTop");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ShaderLoader] 创建UI Shader失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 从字符串创建Shader（运行时编译）
        /// </summary>
        private static Shader? CreateShaderFromString(string shaderCode, string shaderName)
        {
            try
            {
                // 使用ShaderUtil创建Shader（如果可用）
                var shaderUtilType = typeof(Shader).Assembly.GetType("UnityEditor.ShaderUtil");
                if (shaderUtilType != null)
                {
                    var createShaderAsset = shaderUtilType.GetMethod("CreateShaderAsset", 
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    
                    if (createShaderAsset != null)
                    {
                        return createShaderAsset.Invoke(null, new object[] { shaderCode, false }) as Shader;
                    }
                }

                // Fallback: 尝试使用Shader.Find（如果游戏已经加载了类似的Shader）
                Debug.LogWarning($"[ShaderLoader] 无法运行时编译Shader，尝试查找现有Shader");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ShaderLoader] 运行时创建Shader失败: {ex.Message}");
                return null;
            }
        }
    }
}
