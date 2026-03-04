using UnityEngine;

public static class QuaternionExtensions
{

    public static Quaternion RandomRotation()
    {
        return Quaternion.LookRotation(Vector3Extensions.RandomDirection(), Vector3Extensions.RandomDirection());
    }
}
