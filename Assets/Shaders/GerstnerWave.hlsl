void GerstnerWave_float(
    float3 ObjectPos,
    float2 Direction,
    float Amplitude,
    float Frequency,
    float Speed,
    float Steepness,
    float Time,
    out float3 Displacement,
    out float3 Normal,
    out float3 Tangent)
{
    float2 d = length(Direction) > 0.0001 ? normalize(Direction) : float2(0, 1);
    float f = max(Frequency, 0.0) * 6.28318;
    float maxSteepness = 1.0 / max(f * Amplitude, 0.0001);
    float q = min(Steepness, maxSteepness);
    float phase = dot(d, ObjectPos.xz) * f + Time * Speed;
    float s = sin(phase);
    float c = cos(phase);

    Displacement = float3(
        d.x * q * Amplitude * c,
        Amplitude * s,
        d.y * q * Amplitude * c
    );

    Normal = float3(
        -d.x * f * Amplitude * c,
        1.0 - q * f * Amplitude * s,
        -d.y * f * Amplitude * c
    );

    Tangent = float3(
        1.0 - d.x * d.x * q * f * Amplitude * s,
        d.x * f * Amplitude * c,
        -d.x * d.y * q * f * Amplitude * s
    );
}
