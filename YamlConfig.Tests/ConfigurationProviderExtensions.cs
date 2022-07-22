using System;
using Microsoft.Extensions.Configuration;

namespace YamlConfig.Tests
{
    internal static class ConfigurationProviderExtensions
    {
        internal static string Get(this IConfigurationProvider provider, string key)
        {
            if (!provider.TryGet(key, out var value))
            {
                throw new InvalidOperationException("Key not found");
            }

            return value;
        }
    }
}
