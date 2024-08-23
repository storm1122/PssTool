using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace ClassLibrary
{
    public class GenerateConfig
    {
        class _Character  
        {  
            public int character_design_id { get; set; }  
            public string character_design_name { get; set; }  
            public string rarity { get; set; }  
            public List<List<int>> prestige_paths { get; set; } = new List<List<int>>(); // 初始化为空列表  
        }

        public static async Task CheckDesignVersion()
        {
            string url = "https://api.pixelstarships.com/SettingService/GetLatestVersion3?deviceType=DeviceTypeAndroid&languageKey=en";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    Console.WriteLine($"检测CharacterDesignVersion {url}");
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var attributes = XmlHelper.SelectSingleNode(responseBody, "/SettingService/GetLatestSetting/Setting");
                    if (attributes == null)
                    {
                        Console.WriteLine("获取CharacterDesignVersion失败");
                        return;
                    }
                    
                    
                    string characterDesignVersion = attributes["CharacterDesignVersion"].Value;

                    var _version = ConstValue.Get(ConstValue.PrefsDesignVersion);

                    Console.WriteLine("当前CharacterDesignVersion: " + characterDesignVersion + " 缓存的Version:" + _version);
                    
                    if (!string.IsNullOrEmpty(_version) && _version.Equals(characterDesignVersion))
                    {
                        return;
                    }
                    
                    await GeneratePrestigeData(true);

                    await ConstValue.Set(ConstValue.PrefsDesignVersion, characterDesignVersion);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static async Task Download()
        {
            // 创建一个HttpClient实例
            using (HttpClient client = new HttpClient())
            {
                // 设置请求的URL
                string url = "https://api.pixelstarships.com/CharacterService/ListAllCharacterDesigns2";
                string url_cn = "https://api.pixelstarships.com/CharacterService/ListAllCharacterDesigns2?languageKey=zh-cn";
            

                try
                {
                    // 发送GET请求
                    HttpResponseMessage response = await client.GetAsync(url);

                    // 确保请求成功
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // 输出响应内容
                    string savePath = ConstValue.ConfigPath + "ListAllCharacterDesigns.xml";
                    
                    
                    await File.WriteAllTextAsync(savePath, responseBody);
                    Console.WriteLine("saved to: " + savePath);
                
                    // ==========
                    response = await client.GetAsync(url_cn);

                    // 确保请求成功
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容
                    responseBody = await response.Content.ReadAsStringAsync();

                    // 输出响应内容
                    savePath = ConstValue.ConfigPath + "ListAllCharacterDesigns_cn.xml";
                    await File.WriteAllTextAsync(savePath, responseBody);
                }
                catch (HttpRequestException e)
                {
                    // 捕获并处理请求异常
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }
        
        public static async Task GeneratePrestigeData(bool delete = false)
        {
            string path = ConstValue.ConfigPath + "ListAllCharacterDesigns.xml";

            if (delete && Directory.Exists(ConstValue.ConfigPath))
            {
                Directory.Delete(ConstValue.ConfigPath, true);
            }


            if (!Directory.Exists(ConstValue.ConfigPath))
            {
                Directory.CreateDirectory(ConstValue.ConfigPath);
            }
            

            if (!File.Exists(path))
            {
                await Download();
            }
            
            string urlFormat = "https://api.pixelstarships.com/CharacterService/PrestigeCharacterTo?characterDesignId={0}";
            
            var nodes = XmlHelper.SelectNodes(path, "/CharacterService/ListAllCharacterDesigns/CharacterDesigns/CharacterDesign");
          
            
            var charactersData = new List<_Character>();
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null)
                {
                    continue;
                }
                
                var id = node.Attributes["CharacterDesignId"]?.Value;
                var rarity = node.Attributes["Rarity"]?.Value;
                var name = node.Attributes["CharacterDesignName"]?.Value;
                
                
                if (rarity.Equals("Special") || rarity.Equals("Common") || rarity.Equals("Unique") ||
                    rarity.Equals("Elite"))
                {
                    continue;
                }
                
                var c = new _Character();
                c.rarity = rarity;
                c.character_design_id = int.Parse(id);
                c.character_design_name = name;
                charactersData.Add(c);
                

                var url = string.Format(urlFormat, id);
                Console.WriteLine($"{name}:{url}");

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        
                        var nodeList = XmlHelper.SelectNodesByString(responseBody, "/CharacterService/PrestigeCharacterTo/Prestiges/Prestige");

                        foreach (XmlNode _node in nodeList)
                        {
                            if (_node.Attributes == null)
                            {
                                continue;
                            }

                            var id1 = _node.Attributes["CharacterDesignId1"]?.Value;
                            var id2 = _node.Attributes["CharacterDesignId2"]?.Value;

                            
                            if (!c.prestige_paths.Any(path => path.Count == 2 && path.Contains(int.Parse(id1)) && path.Contains(int.Parse(id2))))  
                            {  
                                c.prestige_paths.Add(new List<int> { int.Parse(id1), int.Parse(id2) });  
                            }  
                            
                            // c.prestige_paths.Add(new List<int>(){int.Parse(id1), int.Parse(id2)});
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            
            // 创建一个匿名对象，其包含一个名为"characters"的属性，该属性值为上面的列表  
            var rootObject = new { characters = charactersData };  
      
            // 序列化对象为JSON字符串  
            string jsonString = JsonConvert.SerializeObject(rootObject, Newtonsoft.Json.Formatting.Indented);  
      
            var jsonsavePath = ConstValue.ConfigPath + "character_data.json";
            await File.WriteAllTextAsync(jsonsavePath, jsonString);
        }
        
    }
    
    
}