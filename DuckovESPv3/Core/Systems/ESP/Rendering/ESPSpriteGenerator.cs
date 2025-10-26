using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Rendering
{
    /// <summary>
    /// ESP Sprite 生成器
    /// 程序化生成用于 ESP 标记的圆形 Sprite
    /// </summary>
    public static class ESPSpriteGenerator
    {
        /// <summary>
        /// 创建圆形 Sprite
        /// </summary>
        /// <param name="size">纹理大小（正方形）</param>
        /// <param name="antialiasing">是否使用抗锯齿</param>
        /// <returns>圆形 Sprite</returns>
        public static Sprite CreateCircleSprite(int size = 64, bool antialiasing = true)
        {
            Texture2D texture = CreateCircleTexture(size, antialiasing);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f), // 中心点
                100f // Pixels Per Unit
            );
            return sprite;
        }

        /// <summary>
        /// 创建圆形纹理
        /// </summary>
        /// <param name="size">纹理大小（正方形）</param>
        /// <param name="antialiasing">是否使用抗锯齿</param>
        /// <returns>圆形纹理</returns>
        private static Texture2D CreateCircleTexture(int size, bool antialiasing)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            
            float center = size / 2f;
            float radius = size / 2f;
            float aaRadius = antialiasing ? 1.5f : 0f; // 抗锯齿边缘宽度

            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = y * size + x;
                    
                    // 计算到中心的距离
                    float dx = x - center;
                    float dy = y - center;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);

                    if (distance <= radius - aaRadius)
                    {
                        // 实心区域
                        pixels[index] = Color.white;
                    }
                    else if (distance <= radius)
                    {
                        // 抗锯齿边缘
                        float alpha = 1f - (distance - (radius - aaRadius)) / aaRadius;
                        pixels[index] = new Color(1f, 1f, 1f, alpha);
                    }
                    else
                    {
                        // 透明区域
                        pixels[index] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            return texture;
        }

        /// <summary>
        /// 创建方形 Sprite（可选）
        /// </summary>
        public static Sprite CreateSquareSprite(int size = 64)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            
            Color[] pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                100f
            );
            return sprite;
        }

        /// <summary>
        /// 创建菱形 Sprite（可选）
        /// </summary>
        public static Sprite CreateDiamondSprite(int size = 64, bool antialiasing = true)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            
            float center = size / 2f;
            float halfSize = size / 2f;
            float aaRadius = antialiasing ? 1.5f : 0f;

            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = y * size + x;
                    
                    // 菱形的曼哈顿距离
                    float dx = Mathf.Abs(x - center);
                    float dy = Mathf.Abs(y - center);
                    float distance = dx + dy;

                    if (distance <= halfSize - aaRadius)
                    {
                        pixels[index] = Color.white;
                    }
                    else if (distance <= halfSize)
                    {
                        float alpha = 1f - (distance - (halfSize - aaRadius)) / aaRadius;
                        pixels[index] = new Color(1f, 1f, 1f, alpha);
                    }
                    else
                    {
                        pixels[index] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                100f
            );
            return sprite;
        }
    }
}
