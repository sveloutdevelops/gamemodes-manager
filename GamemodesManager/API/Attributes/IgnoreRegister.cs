using System;

namespace GamemodesManager.API.Attributes
{
    /// <summary>
    /// Skips registering gamemode 
    /// </summary>
    /// <remarks>Note: Works only with classes which inherited by <see cref="Gamemode{T}"/></remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class IgnoreRegisterAttribute : Attribute
    {
    }
}
