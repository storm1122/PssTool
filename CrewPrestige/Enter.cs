using System;
using System.Collections.Generic;
using System.IO;
using ClassLibrary;

public partial class CrewPrestige
{
    public static void Enter(int loopCount = 5000)
    {
        CrewPrestige.loopCount = loopCount;
        
        Crews.Init();
        
        foreach (var ele in Crews.NameToId)
        {
            CrewPrestige.nameToId[ele.Key] = int.Parse(ele.Value);
            CrewPrestige.idToName[int.Parse(ele.Value)] = ele.Key;
        }

        
        string filePath =  ConstValue.TxtPath + @"crew.txt"; // 替换为你的文件路径
        List<string> targets = new List<string>();  
        List<string> possessions = new List<string>();  
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
                else if (line.Trim().Contains("------------------拥有：-------------------"))  
                {  
                    isInTargetsSection = false;  
                    continue; // 跳过这一行，不加入任何数组  
                }  

                if (isInTargetsSection)  
                {
                    if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        targets.Add(line.Trim());  
                    }
                }  
                else  
                {
                    if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        possessions.Add(line.Trim());  
                    }
                }  
            }  
        }  

        // 将List转换为Array（如果需要的话）  
        string[] targetArray = targets.ToArray();  
        string[] possessionArray = possessions.ToArray();  

        // 输出结果以验证  
        Console.WriteLine("Targets:");  
        foreach (var target in targetArray)
        {
            var crew = Crews.Get(target);
            Console.WriteLine($"no:{crew.Id} {crew.Name} {crew.NameCn} {crew.RarityStar()}");  
        }  

        Console.WriteLine("\nPossessions:");
        foreach (var possession in possessionArray)  
        {  
            var crew = Crews.Get(possession);
            Console.WriteLine($"no:{crew.Id} {crew.Name} {crew.NameCn} {crew.RarityStar()}");  
        }


        CrewPrestige.targetCharacters = targetArray;

        CrewPrestige.inventory = possessionArray;

        CrewPrestige.Test();
      
    }
}