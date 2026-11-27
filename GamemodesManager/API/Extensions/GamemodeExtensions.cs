using GamemodesManager.API.Interfaces;
using System.Linq;

namespace GamemodesManager.API.Extensions
{
    internal static class GamemodeExtensions
    {
        /// <summary>
        /// Gets config of gamemode
        /// </summary>
        /// <returns>
        /// If the config is exists, then its <see cref="IGamemodeConfig"/> of config will be returned, otherwise it will return <see langword="null"/>
        /// </returns>
        /// <remarks>
        /// Note: it gets config from <see cref="Config"/>, this will always return <see langword="null"/> unless there is a config variable there 
        /// <para>You also can cast returned value to class of your config</para>
        /// </remarks>
        internal static IGamemodeConfig GetGamemodeConfig(this GamemodeElement gamemode) 
        {
            var configVariables = GamemodesManager.Singleton.Config.GetType().GetProperties()
                .Where(property => property.PropertyType.GetInterface(nameof(IGamemodeConfig)) != null);
            var configType = gamemode.GetType().BaseType?.GetGenericArguments().First();
            foreach (var variable in configVariables)
            {
                if (variable.PropertyType == configType && variable.PropertyType.IsAssignableFrom(configType)) 
                {
                    return variable.GetValue(GamemodesManager.Singleton.Config, null) as IGamemodeConfig; 
                }
            }

            return null;
        }
    }
}