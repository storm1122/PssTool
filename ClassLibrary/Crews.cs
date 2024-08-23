using System;
using System.Collections.Generic;
using System.Xml;

namespace ClassLibrary
{
    public static class Crews
    {
        static public Dictionary<string, string> NameToId = new();
        static public Dictionary<string, string> CNNameToId = new();
        static public Dictionary<string, Crew> All = new();        // id  crew

        public static Crew GetById(string id)
        {
            if (All.ContainsKey(id))
            {
                return All[id];
            }
            return null;
        }
        
        public static Crew Get(string name)
        {
            if (NameToId.TryGetValue(name, out var id))
            {
                return All[id];
            }
            if (CNNameToId.TryGetValue(name, out var id1))
            {
                return All[id1];
            }
            Console.WriteLine($"========= ERROR !!! ==========不存在船员：{name}");
            return null;
        }
        
        public static string GetNameCn(string name)
        {
            if (NameToId.ContainsKey(name))
            {
                return All[NameToId[name]].NameCn;
            }
            
            return string.Empty;
        }
        
        public static void Init()
        {
           InitEn();
           InitCn();
           Console.WriteLine(" ---- init ----");
        }
        
        static void InitEn()
        {
            string path = ConstValue.ConfigPath + "ListAllCharacterDesigns.xml";
            var nodes = XmlHelper.SelectNodes(path, "/CharacterService/ListAllCharacterDesigns/CharacterDesigns/CharacterDesign");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null)
                {
                    continue;
                }
                
                string Get(string attrName)
                {
                    return node.Attributes[attrName]?.Value;
                }

                var id = Get("CharacterDesignId");
                var name = Get("CharacterDesignName");

         
                var crew = new Crew();
                crew.Id = id;
                crew.Name = name;
                var rarityStr = Get("Rarity");
                if (Enum.TryParse<Rarity>(rarityStr, out var ret))
                {
                    crew.Rarity = ret;
                }
                
                All.Add(id, crew);
                
                crew.SetAttr(AttrType.HP, float.Parse(Get("FinalHp")));
                crew.SetAttr(AttrType.Attack, float.Parse(Get("FinalAttack")));
                crew.SetAttr(AttrType.Ability, float.Parse(Get("SpecialAbilityFinalArgument")));
                crew.SetAttr(AttrType.Stamina, 0f);
                crew.SetAttr(AttrType.Pilot, float.Parse(Get("FinalPilot")));
                crew.SetAttr(AttrType.Science, float.Parse(Get("FinalScience")));
                crew.SetAttr(AttrType.Engineer, float.Parse(Get("FinalEngine")));
                crew.SetAttr(AttrType.Weapon, float.Parse(Get("FinalWeapon")));
                crew.SetAttr(AttrType.Repair, float.Parse(Get("FinalRepair")));
                crew.SetAttr(AttrType.FireResistance, float.Parse(Get("FireResistance")));
                crew.SetAttr(AttrType.TrainingCapacity, float.Parse(Get("TrainingCapacity")));
                crew.SetAttr(AttrType.RunSpeed, float.Parse(Get("RunSpeed")));
                crew.SetAttr(AttrType.WalkingSpeed, float.Parse(Get("WalkingSpeed")));
                
                if (NameToId.ContainsKey(name))
                {
                    var previousId = NameToId[name];

                    if (crew.Rarity >= All[previousId].Rarity)
                    {
                        NameToId[name] = id;
                    }
                    
                    // Console.WriteLine($"id:{id},{name}  与  id:{previousId},{name} 名字重复!!!!  根据星级规则保留{NameToId[name]}");
                }
                else
                {
                    NameToId.Add(name, id);
                }
            }

        }

        static void InitCn()
        {
            string path = ConstValue.ConfigPath + "ListAllCharacterDesigns_cn.xml";
            var nodes = XmlHelper.SelectNodes(path, "/CharacterService/ListAllCharacterDesigns/CharacterDesigns/CharacterDesign");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null)
                {
                    continue;
                }
                
                string Get(string attrName)
                {
                    return node.Attributes[attrName]?.Value;
                }

                var id = Get("CharacterDesignId");
                var name = Get("CharacterDesignName");

                if (All.TryGetValue(id, out var crew))
                {
                    crew.NameCn = name;
                    
                    
                        
                    if (CNNameToId.ContainsKey(name))
                    {
                        var previousId = CNNameToId[name];

                        if (crew.Rarity >= All[previousId].Rarity)
                        {
                            CNNameToId[name] = id;
                        }
                    
                        // Console.WriteLine($"id:{id},{name}  与  id:{previousId},{name} 名字重复!!!!  根据星级规则保留{CNNameToId[name]}");
                    }
                    else
                    {
                        CNNameToId.Add(name, id);
                    }
                }
            }
        }
        
    }
}