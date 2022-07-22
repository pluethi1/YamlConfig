 # YamlConfig
 YamlConfig provides a possiblity to use Yaml files in your configurations utilizing Microsoft.Extensions.Configuration.
 Simply reference the assembly and add Yaml files and streams to your configuration.
 
 ## Example usage
 
 ### Adding file via path
 ```csharp
 using Microsoft.Extensions.Configuration;
 
 var config = new ConfigurationBuilder();
                  .AddYamlFile("config/config.yaml")
                  .Build();
                  
//Use config here                 
 ```
 
 ### Adding file via stream
 ```csharp
 using System.IO;
 using Microsoft.Extensions.Configuration;
 
 var stream = File.OpenRead("config/config.yaml");
 
 var config = new ConfigurationBuilder()
                  .AddYamlStream(stream)
                  .Build();
//Use config here
 ```
