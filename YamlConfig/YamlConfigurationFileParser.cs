using Microsoft.Extensions.Configuration;
using YamlConfig.Resources;
using YamlDotNet.RepresentationModel;

namespace YamlConfig
{
    internal sealed class YamlConfigurationFileParser
    {
        private YamlConfigurationFileParser() { }

        private readonly Dictionary<string, string?> _data = new(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _paths = new();

        public static IDictionary<string, string?> Parse(Stream input) => new YamlConfigurationFileParser().ParseStream(input);

        private IDictionary<string, string?> ParseStream(Stream input)
        {
            using var reader = new StreamReader(input);
            var stream = new YamlStream();
            stream.Load(reader);

            var document = stream.Documents[0];

            if (document.RootNode.NodeType != YamlNodeType.Mapping)
            {
                throw new FormatException(string.Format(Strings.Error_InvalidTopLevelYamlElement, document.RootNode.NodeType));
            }

            VisitMappingNode((YamlMappingNode)document.RootNode);

            return _data;
        }

        private void VisitMappingNode(YamlMappingNode node)
        {
            var isEmpty = true;
            foreach (var item in node)
            {
                isEmpty = false;
                EnterContext(((YamlScalarNode)item.Key).Value!);
                VisitValue(item.Value);
                ExitContext();
            }

            SetNullIfNodeIsEmpty(isEmpty);
        }

        private void VisitSequenceNode(YamlSequenceNode node)
        {
            int index = 0;
            foreach (var item in node)
            {
                EnterContext(index.ToString());
                VisitValue(item!);
                ExitContext();
                index++;
            }

            SetNullIfNodeIsEmpty(index == 0);
        }

        private void SetNullIfNodeIsEmpty(bool isEmpty)
        {
            if (isEmpty && _paths.Count > 0)
            {
                _data[_paths.Peek()] = null;
            }
        }

        private void VisitValue(YamlNode value)
        {
            switch (value.NodeType)
            {
                case YamlNodeType.Mapping:
                    VisitMappingNode((YamlMappingNode)value);
                    break;

                case YamlNodeType.Sequence:
                    VisitSequenceNode((YamlSequenceNode)value);
                    break;

                case YamlNodeType.Scalar:
                    var key = _paths.Peek();
                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException(string.Format(Strings.Error_KeyIsDuplicated, key));
                    }
                    _data[key] = ((YamlScalarNode)value).Value;
                    break;

                default:
                    throw new FormatException(string.Format(Strings.Error_UnsupportedYamlToken, value.NodeType));
            }
        }

        private void EnterContext(string context)
        {
            _paths.Push(_paths.Count > 0 ? _paths.Peek() + ConfigurationPath.KeyDelimiter + context : context);
        }

        private void ExitContext() => _paths.Pop();
    }
}
