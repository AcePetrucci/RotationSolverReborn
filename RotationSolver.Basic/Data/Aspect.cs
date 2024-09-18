﻿namespace RotationSolver.Basic.Data;

/// <summary>
/// Represents the elemental or damage aspect of an action.
/// </summary>
public enum Aspect : byte
{
    /// <summary>
    /// Fire aspect.
    /// </summary>
    Fire = 1,

    /// <summary>
    /// Ice aspect.
    /// </summary>
    Ice = 2,

    /// <summary>
    /// Wind aspect.
    /// </summary>
    Wind = 3,

    /// <summary>
    /// Earth aspect.
    /// </summary>
    Earth = 4,

    /// <summary>
    /// Lightning aspect.
    /// </summary>
    Lightning = 5,

    /// <summary>
    /// Water aspect.
    /// </summary>
    Water = 6,

    /// <summary>
    /// Piercing aspect.
    /// </summary>
    Piercing = 7,
}