using System.ComponentModel;

namespace GamemodesManager.API.Interfaces
{
    public interface IGamemodeConfig 
    {
       [Description("Is gamemode available to run?")]
       bool IsEnabled { get; set; }
    }
}