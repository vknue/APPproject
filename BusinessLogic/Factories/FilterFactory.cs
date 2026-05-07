using BusinessLogic.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Factories
{
    public static class FilterFactory
    {
        private static readonly Dictionary<string, Type> _strategies = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => typeof(IImageProcessingStrategy).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
        .ToDictionary(t => t.Name.Replace("Strategy", "").ToLower(), t => t);

        public static IImageProcessingStrategy? GetStrategy(string filter)
        {
            if (_strategies.TryGetValue(filter.ToLower(), out var type))
            {
                return (IImageProcessingStrategy?)Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
