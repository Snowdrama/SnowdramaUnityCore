using UnityEngine;
public static class Vector3Extensions
{
    private static System.Random rand = new System.Random();
    public static Vector3 RandomDirection()
    {
        return new Vector3(
            ((float)rand.NextDouble() * 2.0f) - 1.0f,
            ((float)rand.NextDouble() * 2.0f) - 1.0f,
            ((float)rand.NextDouble() * 2.0f) - 1.0f
        ).normalized;
    }

    public static Vector3 Floor(this Vector3 dir)
    {
        return new Vector3(Mathf.Floor(dir.x), Mathf.Floor(dir.y), Mathf.Floor(dir.z));
    }

    public static Vector3 Ceil(this Vector3 dir)
    {
        return new Vector3(Mathf.Ceil(dir.x), Mathf.Ceil(dir.y), Mathf.Ceil(dir.z));
    }

    public static Vector3 Round(this Vector3 dir)
    {
        return new Vector3(Mathf.Round(dir.x), Mathf.Round(dir.y), Mathf.Round(dir.z));
    }

    public static Vector3 Clamp(this Vector3 val, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y), Mathf.Clamp(val.z, min.z, max.z));
    }

}
