using UnityEngine;
using System.Collections;

public interface INoiseFilter
{
    float Evaluate(Vector3 point);
}
