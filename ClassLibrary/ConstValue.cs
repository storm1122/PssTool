using System;  
using System.IO;  
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary  
{  
    public class ConstValue  
    {  
        private const string PrefsFile = "Prefs.txt";

        public const string PrefsDesignVersion = "DesignVersion";
        // public const string ConfigPath = @"../../../../Config/";
        
#if RELEASE
        public static string ConfigPath = Path.Combine(AppContext.BaseDirectory, @"../../../Config/");
#else
        public static string ConfigPath = Path.Combine(AppContext.BaseDirectory, @"../../../像素星舰小工具/Config/");
#endif
        public static string TxtPath = Path.Combine(ConfigPath, @"../");

        public enum OutputLanguage
        {
            en,
            cn
        }
        public static OutputLanguage OutputLan = OutputLanguage.en;  //  en cn
        
        public static bool OutputEn => OutputLan == OutputLanguage.en;
        
        public static async Task Set(string key, string val)  
        {  
            if (!Directory.Exists(ConfigPath))  
            {  
                Directory.CreateDirectory(ConfigPath);  
            }  
  
            string filePath = ConfigPath + PrefsFile;  
            string[] lines = File.Exists(filePath) ? File.ReadAllLines(filePath) : new string[0];  
  
            // 移除旧值（如果有）  
            lines = lines.Where(line => !line.StartsWith(key + "=")).ToArray();  
  
            // 添加新值  
            lines = lines.Concat(new[] { key + "=" + val }).ToArray();  
  
            await File.WriteAllLinesAsync(filePath, lines);  
        }  
  
        public static string Get(string key)  
        {  
            string filePath = ConfigPath + PrefsFile;  
            if (!File.Exists(filePath))  
                return null;  
  
            string[] lines = File.ReadAllLines(filePath);  
            foreach (string line in lines)  
            {  
                if (line.StartsWith(key + "="))  
                {  
                    return line.Substring(key.Length + 1);  
                }  
            }  
            return null;  
        }  
    }  
}