using UnityEngine;
using UnityEngine.Rendering;

namespace DuckovESPv3.Core.Systems.ESP.Rendering
{
    /// <summary>
    /// Material配置器 - 通过修改Material属性实现AlwaysOnTop效果
    /// 无需自定义Shader，兼容性最强
    /// </summary>
    public static class MaterialConfigurator
    {
        /// <summary>
        /// 配置TMP材质为AlwaysOnTop模式
        /// </summary>
        public static void ConfigureTMPMaterial(Material material, Color color)
        {
            if (material == null) return;

            try
            {
                // 基础设置 - 使用最高渲染队列确保不被遮挡
                material.renderQueue = 5000;  // 远超Overlay(4000)，确保最后渲染
                
                // 深度测试设置
                material.SetInt("_ZTest", (int)CompareFunction.Always);  // 永远通过
                material.SetInt("_ZWrite", 0);  // 不写入深度
                material.SetInt("_Cull", (int)CullMode.Off);  // 双面渲染
                
                // 禁用剪裁
                material.DisableKeyword("UNITY_UI_CLIP_RECT");
                material.DisableKeyword("UNITY_UI_ALPHACLIP");
                
                // 尝试设置各种可能的颜色属性
                if (material.HasProperty("_FaceColor"))
                    material.SetColor("_FaceColor", color);
                if (material.HasProperty("_Color"))
                    material.SetColor("_Color", color);
                if (material.HasProperty("_TintColor"))
                    material.SetColor("_TintColor", color);
                
                // TMP特殊属性
                if (material.HasProperty("_FaceDilate"))
                    material.SetFloat("_FaceDilate", 0);
                if (material.HasProperty("_OutlineWidth"))
                    material.SetFloat("_OutlineWidth", 0);
                
                // 强制更新材质
                material.SetPass(0);
                
                Debug.Log($"[MaterialConfigurator] TMP材质配置完成，ZTest={(CompareFunction)material.GetInt("_ZTest")}, RenderQueue={material.renderQueue}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[MaterialConfigurator] 配置TMP材质时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 配置UI材质为AlwaysOnTop模式
        /// </summary>
        public static void ConfigureUIMaterial(Material material, Color color)
        {
            if (material == null) return;

            try
            {
                // 基础设置 - 使用极高渲染队列确保不被遮挡
                material.renderQueue = 4999;  // 略低于TMP文本，但仍在最前
                
                // 深度测试设置
                material.SetInt("_ZTest", (int)CompareFunction.Always);
                material.SetInt("_ZWrite", 0);
                material.SetInt("_Cull", (int)CullMode.Off);  // 双面渲染
                
                // 禁用剪裁
                material.DisableKeyword("UNITY_UI_CLIP_RECT");
                material.DisableKeyword("UNITY_UI_ALPHACLIP");
                
                // 设置颜色
                if (material.HasProperty("_Color"))
                    material.SetColor("_Color", color);
                if (material.HasProperty("_TintColor"))
                    material.SetColor("_TintColor", color);
                
                // 混合模式
                if (material.HasProperty("_SrcBlend"))
                    material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                if (material.HasProperty("_DstBlend"))
                    material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                
                // 强制更新
                material.SetPass(0);
                
                Debug.Log($"[MaterialConfigurator] UI材质配置完成，ZTest={(CompareFunction)material.GetInt("_ZTest")}, RenderQueue={material.renderQueue}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[MaterialConfigurator] 配置UI材质时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 验证材质配置是否生效
        /// </summary>
        public static bool VerifyMaterialConfiguration(Material material)
        {
            if (material == null) return false;

            try
            {
                int zTest = material.GetInt("_ZTest");
                int renderQueue = material.renderQueue;
                
                bool isConfigured = 
                    zTest == (int)CompareFunction.Always && 
                    renderQueue >= 4500;  // 更新验证阈值
                
                if (!isConfigured)
                {
                    Debug.LogWarning($"[MaterialConfigurator] 材质配置未生效: ZTest={zTest}, RenderQueue={renderQueue}");
                }
                
                return isConfigured;
            }
            catch
            {
                return false;
            }
        }
    }
}
