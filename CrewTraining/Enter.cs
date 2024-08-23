using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ClassLibrary;

public partial class CrewTraining
{
    public static void Enter()
    {
        Crews.Init();
        string filePath =  ConstValue.TxtPath + @"training.txt"; // 替换为你的文件路径
        string target = string.Empty;
        string attrtext = string.Empty;

        bool isInTargetsSection = false;  

        using (StreamReader reader = new StreamReader(filePath))
        {
            
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().Contains("------------------目标：------------------"))  
                {  
                    isInTargetsSection = true;  
                    continue; // 跳过这一行，不加入任何数组  
                } 
                else if (line.Trim().Contains("------------------属性：-------------------"))  
                {  
                    isInTargetsSection = false;  
                    continue; // 跳过这一行，不加入任何数组  
                }  
                else if (line.StartsWith("###"))
                {
                    continue;
                }
                if (isInTargetsSection)  
                {
                    if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        target = line.Trim();
                    }
                }  
                else  
                {
                    if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        attrtext += line.Trim();
                    }
                }  
            }
        }
        
  
        Console.WriteLine(target);
        Console.WriteLine(attrtext);
        
        string pattern = @"(HP|Attack|Repair|Ability|Stamina|Pilot|Science|Engineer|Weapon)\s*[=，：]+\s*(\d+)";  
  
        MatchCollection matches = Regex.Matches(attrtext, pattern);  
  
        var attrinfos = new List<(AttrType, int)>();  
  
        foreach (Match match in matches)  
        {  
            string key = match.Groups[1].Value;  
            if (Enum.TryParse<AttrType>(key, out AttrType attrType))  
            {  
                int value = int.Parse(match.Groups[2].Value);  
                attrinfos.Add((attrType, value)); // 直接添加到列表中，保持顺序  
            }  
            else  
            {  
                Console.WriteLine($"Unknown attribute: {key}");  
            }  
        }  
  
        CrewTraining.Test(target, attrinfos);  
    }
  
}