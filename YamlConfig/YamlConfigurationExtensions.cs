using Microsoft.Extensions.FileProviders;
using YamlConfig;
using YamlConfig.Resources;

namespace Microsoft.Extensions.Configuration
{
    public static class YamlConfigurationExtensions
    {
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path)
        {
            return AddYamlFile(builder, null, path, false, false);
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddYamlFile(builder, null, path, optional, false);
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddYamlFile(builder, null, path, optional, reloadOnChange);
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, IFileProvider? provider, string path, bool optional, bool reloadOnChange)
        {
            ArgumentNullException.ThrowIfNull(builder);

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Strings.Error_InvalidFilePath, nameof(path));
            }

            return builder.AddYamlFile(s =>
            {
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ResolveFileProvider();
            });
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, Action<YamlConfigurationSource>? configureSource) => builder.Add(configureSource);

        public static IConfigurationBuilder AddYamlStream(this IConfigurationBuilder builder, Stream stream)
        {
            ArgumentNullException.ThrowIfNull(builder);
            return builder.Add<YamlStreamConfigurationSource>(s => s.Stream = stream);
        }
    }
}
