using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ShapeSettings : ScriptableObject
{
    [Range(1,10)]
    public float planetRadius = 1;
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask = false;
        public NoiseSettings noiseSettings;
    }
}
