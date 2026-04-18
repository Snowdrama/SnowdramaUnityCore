using System;
using UnityEngine;

public class PhysicsTools
{
    /// <summary>
    /// Derives a value for Gravity based on how high you want 
    /// the player to jump and how long you want the player 
    /// to take 
    /// </summary>
    /// <param name="fallHeight">The height of the jump in terms of
    /// how far the player would fall from the peak of the jump</param>
    /// <param name="timeToReachGround">how long it would take to go from 
    /// the peak of the jump to the ground</param>
    /// <returns>A value for gravity</returns>
    public static double DeriveGravity(double fallHeight, double timeToReachGround)
    {
        return 2 * fallHeight / (timeToReachGround * timeToReachGround);
    }

    public static double DeriveInitialVelocity(double jumpHeight, double timeToReachPeak, double gravity = -9.8f)
    {
        return (jumpHeight - (0.5f * gravity * timeToReachPeak * timeToReachPeak)) / timeToReachPeak;
    }

    public static float DeriveGravity(float fallHeight, float timeToReachGround)
    {
        return 2 * fallHeight / (timeToReachGround * timeToReachGround);
    }

    public static float DeriveInitialVelocity(float jumpHeight, float timeToReachPeak, float gravity = -9.8f)
    {
        return (jumpHeight - (0.5f * gravity * timeToReachPeak * timeToReachPeak)) / timeToReachPeak;
    }
}
