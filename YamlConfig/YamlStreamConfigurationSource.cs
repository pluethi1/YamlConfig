using Microsoft.Extensions.Configuration;

namespace YamlConfig
{
    public class YamlStreamConfigurationSource : StreamConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new YamlStreamConfigurationProvider(this);
        }
    }
}
