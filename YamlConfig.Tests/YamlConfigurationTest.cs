using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace YamlConfig.Tests
{
    public class YamlConfigurationTest
    {
        private static YamlConfigurationProvider LoadProvider(string yaml)
        {
            var p = new YamlConfigurationProvider(new YamlConfigurationSource { Optional = true });
            p.Load(TestUtil.StringToStream(yaml));
            return p;
        }

        [Fact]
        public void CanLoadValidYamlFromStreamProvider()
        {
            var yaml = @"
name: John Smith
contact:
    home:   1012355532
    office: 5002586256
address:
    street: |
        123 Tornado Alley
        Suite 16
    city: East Centerville
    state: KS
";
            var config = new ConfigurationBuilder().AddYamlStream(TestUtil.StringToStream(yaml)).Build();
            Assert.Equal("John Smith", config["name"]);
            Assert.Equal("1012355532", config["contact:home"]);
            Assert.Equal("5002586256", config["contact:office"]);
            Assert.Equal("123 Tornado Alley\nSuite 16\n", config["address:street"]);
            Assert.Equal("East Centerville", config["address:city"]);
            Assert.Equal("KS", config["address:state"]);
        }

        [Fact]
        public void ReloadThrowsFromStreamProvider()
        {
            var yaml = @"
firstname: test
";
            var config = new ConfigurationBuilder().AddYamlStream(TestUtil.StringToStream(yaml)).Build();
            Assert.Throws<InvalidOperationException>(() => config.Reload());
        }

        [Fact]
        public void LoadKeyValuePairsFromValidYaml()
        {
            var yaml = @"
name: John Smith
contact:
    home:   1012355532
    office: 5002586256
address:
    street: |
        123 Tornado Alley
        Suite 16
    city: East Centerville
    state: KS
";

            var yamlConfigSrc = LoadProvider(yaml);

            Assert.Equal("John Smith", yamlConfigSrc.Get("name"));
            Assert.Equal("1012355532", yamlConfigSrc.Get("contact:home"));
            Assert.Equal("5002586256", yamlConfigSrc.Get("contact:office"));
            Assert.Equal("123 Tornado Alley\nSuite 16\n", yamlConfigSrc.Get("address:street"));
            Assert.Equal("East Centerville", yamlConfigSrc.Get("address:city"));
            Assert.Equal("KS", yamlConfigSrc.Get("address:state"));

        }

        [Fact]
        public void LoadMethodCanHandleEmptyValue()
        {
            var yaml = @"
firstname: ''
";

            var yamlConfigSrc = LoadProvider(yaml);
            Assert.Equal(string.Empty, yamlConfigSrc.Get("firstname"));
        }

        [Fact]
        public void LoadWithCulture()
        {
            var previousCulture = CultureInfo.CurrentCulture;

            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                var yaml = @"
number: 3,14
";
                var yamlConfigSrc = LoadProvider(yaml);
                Assert.Equal("3,14", yamlConfigSrc.Get("number"));
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }
        }

        [Fact]
        public void NonMappingRootIsInvalid()
        {
            var yaml = "test";
            var exception = Assert.Throws<FormatException>(() => LoadProvider(yaml));
            Assert.NotNull(exception.Message);
        }
    }
}
