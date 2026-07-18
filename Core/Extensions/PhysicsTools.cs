using System;
using UnityEngine;

public class PhysicsTools
{
    /// <summary>
    /// Derives a value for Gravity based on how high you want 
    /// the player to jump and how long you want the player 
    /// to take 
    /// </summary>
    /// <param name="jumpHeight">
    /// The height of the jump
    /// 
    /// how far the player would fall from the peak of the jump
    /// </param>
    /// <param name="timeToReachGround">
    /// Half the time of the whole jump.
    /// 
    /// How long it would take to go from
    /// the ground to the peak or from
    /// the peak of the jump to the ground.
    /// </param>
    /// <returns>A value for gravity NOTE: THIS IS NEGATIVE FOR A REASON GRAVITY IS DOWN</returns>
    public static double DeriveGravity(double jumpHeight, double timeToReachPeak)
    {
        return -(2 * jumpHeight / (timeToReachPeak * timeToReachPeak));
    }


    /// <summary>
    /// Derives a value for Gravity based on how high you want 
    /// the player to jump and how long you want the player 
    /// to take 
    /// 
    /// Combined with PhysicsTools.DeriveInitialVelocity
    /// This creates a parabola where you jump with some force from the ground
    /// reach some 'jumpHeight' and then return to the ground in 'jumpTime'
    /// </summary>
    /// <param name="jumpHeight">
    /// The height of the jump
    /// 
    /// In terms of the parabola of the player's height
    /// this is the peak of the parabola
    /// </param>
    /// <param name="timeToReachPeak">
    /// Half the time of the whole jump.
    /// 
    /// How long it would take to go from
    /// the ground to the peak or from
    /// the peak of the jump to the ground.
    /// </param>
    /// <returns>A value for gravity NOTE: THIS IS NEGATIVE FOR A REASON GRAVITY IS DOWN</returns>
    public static float DeriveGravity(float jumpHeight, float timeToReachPeak)
    {
        return -(2 * jumpHeight / (timeToReachPeak * timeToReachPeak));
    }

    /// <summary>
    /// Derives the force needed to reach a certain height given some gravity and some time
    /// </summary>
    /// <param name="jumpHeight">
    /// The height of the jump
    /// 
    /// In terms of the parabola of the player's height
    /// this is the peak of the parabola
    /// </param>
    /// <param name="timeToReachPeak">
    /// Half the time of the whole jump.
    /// 
    /// How long it would take to go from
    /// the ground to the peak or from
    /// the peak of the jump to the ground.
    /// </param>
    /// <param name="gravity">
    /// THIS MUST BE NEGATIVE TO WORK!
    /// 
    /// How strong the influence of gravity is.
    /// 
    /// Use PhysicsTools.DeriveGravity to get a gravity 
    /// from the jump height and timeToReachPeak that matches
    /// the inputs for PhysicsTools.DeriveInitialVelocity
    /// </param>
    /// <returns>The velocity to use to match the parabola described by the jump parameters</returns>
    public static double DeriveInitialVelocity(double jumpHeight, double timeToReachPeak, double gravity = -9.8f)
    {
        return (jumpHeight - (0.5f * gravity * timeToReachPeak * timeToReachPeak)) / timeToReachPeak;
    }

    /// <summary>
    /// Derives the force needed to reach a certain height given some gravity and some time
    /// </summary>
    /// <param name="jumpHeight">
    /// The height of the jump
    /// 
    /// In terms of the parabola of the player's height
    /// this is the peak of the parabola
    /// </param>
    /// <param name="timeToReachPeak">
    /// Half the time of the whole jump.
    /// 
    /// How long it would take to go from
    /// the ground to the peak or from
    /// the peak of the jump to the ground.
    /// </param>
    /// <param name="gravity">
    /// THIS MUST BE NEGATIVE TO WORK!
    /// 
    /// How strong the influence of gravity is.
    /// 
    /// Use PhysicsTools.DeriveGravity to get a gravity 
    /// from the jump height and timeToReachPeak that matches
    /// the inputs for PhysicsTools.DeriveInitialVelocity
    /// </param>
    /// <returns>The velocity to use to match the parabola described by the jump parameters</returns>
    public static float DeriveInitialVelocity(float jumpHeight, float timeToReachPeak, float gravity = -9.8f)
    {
        return (jumpHeight - (0.5f * gravity * timeToReachPeak * timeToReachPeak)) / timeToReachPeak;
    }
}

