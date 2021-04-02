using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        this.texture = this.texture == null || texture.height != settings.biomeColorSettings.biomes.Length ? new Texture2D(textureResolution, settings.biomeColorSettings.biomes.Length) : this.texture;
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 pointOnSphere)
    {
        float heightPercent = (pointOnSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
        float biomeIndex = 0;
        float numBiomes = settings.biomeColorSettings.biomes.Length;
        float blendRange = settings.biomeColorSettings.blendAmount / 2f + .001f;

        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColors()
    {
        Color32[] colors = new Color32[texture.width * texture.height];
        int colorIndex = 0;

        // todo default planet color, biomes override it, if set
        // todo enable disable biomes
        foreach (var biome in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution; i++)
            {
                Color32 gradientColor;
                if (i < textureResolution)
                {
                    gradientColor = settings.oceanColor.Evaluate(i / (textureResolution - 1f));
                }
                else
                {
                    gradientColor = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                }
                Color32 tintColor = biome.tint;
                colors[colorIndex++] = (Color)gradientColor * (1 - biome.tintPercent) + (Color)tintColor * biome.tintPercent;
            }
        }

        if (colors != null && colors.Length > 0)
        {
            texture.SetPixels32(colors);
        }
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }
}
