using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClassLibrary;

public partial class CrewPrestigeRandom
{

    public class Node
    {
        private static int _uid = 0;
        public Crew Crew;
        public Tuple<Node, Node> Child;
        public int Layer => (int)Crew.Rarity;

        public Node(Crew crew)
        {
            Crew = crew;
        }

        public void SetChild(Node a, Node b)
        {

            Child = new Tuple<Node, Node>(a, b);
        }

        public void Dispose()
        {
            if (Child != null)
            {
                Child.Item1.Dispose();
                Child.Item2.Dispose();
                Child = null;
            }
        }
    }
    
    public static List<Node> All = new List<Node>();

    public static Dictionary<Tuple<int, int>, int> Rules = new();

    static string jsonFile = ConstValue.ConfigPath + "character_data.json";
    
    static void Test()
    {
        LoadSynthesisRules(jsonFile);

        Recursion(Rarity.Unique);

        PrintNodes();
    }

    static void PrintNodes()
    {
        Console.WriteLine("==================================================================");
        var maxLayer = All.Max(n => n.Layer);

        var maxLayerNodes = All.Where(n => n.Layer == maxLayer && n.Child != null).ToList();

        if (maxLayerNodes.Count == 0)
        {
            Console.WriteLine("==================== 无法合成！ =======================");
            return;
        }
        
        foreach (var node in maxLayerNodes)
        {
            var str = $"";
            int layer = node.Layer;
            PrintNodeRecursion(node,ref str, ref layer);
            Console.WriteLine(str);
        }  
    }

    static void PrintNodeRecursion(Node node, ref string str,ref int layer)
    {
        
        if (node.Child == null)
        {
            str = str + $"{node.Crew.Info()}";
            return;
        }
        else
        {
            str = str + $"{node.Crew.Info()} = ";
        }

        if (layer - node.Layer == 1)
            str += "{";
        else if(layer - node.Layer == 2)
            str += "(";
        else if(layer - node.Layer == 0)
            str += "";
        PrintNodeRecursion(node.Child.Item1,ref str, ref layer);
        str += " + ";
        PrintNodeRecursion(node.Child.Item2,ref str ,ref layer);
        if (layer - node.Layer == 1)
            str += "}";
        else if(layer - node.Layer == 2)
            str += ")";
        else if(layer - node.Layer == 0)
            str += "";
    }


    
    static void Recursion(Rarity curRarity)
    {
        // Console.WriteLine($"================= 检测{curRarity+1} =================");
        var nodes = All.Where(c => c.Crew.Rarity == curRarity).ToList();  
  
        // 随机两两组合  
        Random random = new Random();  
        var combinations = new List<Tuple<Node, Node>>();  
  
        // 确保有足够的元素进行组合  
        while (nodes.Count >= 2)  
        {  
            // 随机选择两个索引  
            int index1 = random.Next(0, nodes.Count);  
            int index2 = random.Next(0, nodes.Count);  
  
            // 确保两个索引不同  
            while (index1 == index2)  
            {  
                index2 = random.Next(0, nodes.Count);  
            }  
  
            // 添加组合到列表中  
            combinations.Add(Tuple.Create(nodes[index1], nodes[index2]));  
            combinations.Add(Tuple.Create(nodes[index2], nodes[index1]));  
  
            // 移除已选择的元素  
            nodes.RemoveAt(Math.Min(index1, index2));  
            if (index1 > index2) index1--; // 如果移除了前面的元素，后面的索引需要减1  
            nodes.RemoveAt(index1);  
        }  
  
        // 输出结果  
        foreach (var combo in combinations)  
        {  
            var k = new Tuple<int, int>(combo.Item1.Crew.IdNum, combo.Item2.Crew.IdNum);
            if (Rules.TryGetValue(k, out var id))
            {
                var newCrew = Crews.GetById(id.ToString());
                if (newCrew != null)
                {
                    var newNode = new Node(newCrew);
                    All.Add(newNode);
                    newNode.SetChild(combo.Item1, combo.Item2);
                    // Console.WriteLine($"{combo.Item1.Crew.Info()} + {combo.Item2.Crew.Info()} = {newCrew.Info()}");
                }
                else
                {
                    Console.WriteLine($"{combo.Item1.Crew.AllInfo()} + {combo.Item2.Crew.AllInfo()} 的组合，找不到对应的ID：{id}"  );
                }
            }
        }

        if (curRarity < Rarity.Hero)
        {
            curRarity++;
            Recursion(curRarity);
        }
    }
    
    
    static void LoadSynthesisRules(string jsonFile)
    {
        string jsonText = File.ReadAllText(jsonFile);
        dynamic data = JsonHelper.DeserializeObject(jsonText);
        foreach (var character in data["characters"])
        {
            foreach (var rule in character["prestige_paths"])
            {
                if (rule.Count != 2)
                {
                    continue;
                }
                var k = new Tuple<int, int>((int)rule[0], (int)rule[1]);
                int v = character["character_design_id"];
                if (!Rules.ContainsKey(k))
                {
                    Rules.Add(k, v);
                }
            }
        }
    }
    
    
    
    public static IEnumerable<(Crew, Crew)> GroupAndPairCrews(List<Crew> crews)  
    {  
        // 根据Rarity分组  
        var groupedCrews = crews.GroupBy(c => c.Rarity)  
            .ToDictionary(g => g.Key, g => g.ToList());  
  
        // 对每个组进行随机化  
        foreach (var group in groupedCrews.Values)  
        {  
            Random rnd = new Random((int)DateTime.Now.Ticks);  
            group.OrderBy(x => rnd.Next()).ToList(); // 注意：这会改变原始列表的顺序，但在这个字典的上下文中它不会持久化  
  
            // 为了使随机化持久化，需要重新赋值给列表  
            groupedCrews[group.First().Rarity] = group.OrderBy(x => rnd.Next()).ToList();  
        }  
  
        // 对每个组进行两两配对  
        foreach (var group in groupedCrews.Values)  
        {  
            for (int i = 0; i < group.Count; i += 2)  
            {  
                if (i + 1 < group.Count) // 确保有第二个元素可以配对  
                {  
                    yield return (group[i], group[i + 1]);  
                }  
                // 如果元素数量为奇数，最后一个元素将不会被配对  
            }  
        }  
    }  
}