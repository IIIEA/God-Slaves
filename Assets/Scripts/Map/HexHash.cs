using UnityEngine;

public struct HexHash
{
    public float A, B, C, D, E;

    public static HexHash Create()
    {
        HexHash hash;
        hash.A = Random.value * 0.999f;
        hash.B = Random.value * 0.999f;
        hash.C = Random.value * 0.999f;
        hash.D = Random.value * 0.999f;
        hash.E = Random.value * 0.999f;
        return hash;
    }
}