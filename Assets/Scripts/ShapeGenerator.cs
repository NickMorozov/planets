using UnityEngine;

public class ShapeGenerator
{
    private ShapeSettings settings;
    private INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax = new MinMax();

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        this.noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation += firstLayerValue;
            }
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnSphere) * mask;
            }
        }
        elevation = settings.planetRadius * (elevation + 1);
        elevationMinMax.AddValue(elevation);

        return pointOnSphere * elevation;
    }
}
