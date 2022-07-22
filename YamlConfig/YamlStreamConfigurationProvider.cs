using Microsoft.Extensions.Configuration;
using YamlConfig.Resources;
using YamlDotNet.Core;

namespace YamlConfig
{
    public class YamlStreamConfigurationProvider : StreamConfigurationProvider
    {
        public YamlStreamConfigurationProvider(YamlStreamConfigurationSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            try
            {
                Data = YamlConfigurationFileParser.Parse(stream);
            }
            catch (YamlException e)
            {
                throw new FormatException(Strings.Error_YamlParseError, e);
            }
        }
    }
}
