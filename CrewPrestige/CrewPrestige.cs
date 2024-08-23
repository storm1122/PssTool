using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClassLibrary;

public partial class CrewPrestige
{
    static string jsonFile = ConstValue.ConfigPath + "character_data.json";
    // static string[] targetCharacters = { "Cyber Duck","Galactic Succubus","Galactic Succubus"};
    
    public static string[] targetCharacters = {"Rocky"};

    public static int loopCount = 1;
    
    public static string[] inventory =
    {
        // "Huge Hellaloya", 
        // "Mistycball", 
        // "Mistycball", 
        "The Red Baron", 
        "Alibaba", 
        // "Huge Hellaloya", 
        // "Mr Coconut", 
        // "Mama", 
        "Vivien", 
        "Saboteur Pianist", 
        // "Glitch", 
        "Civilian Pilot", 
        // "Cara", 
        "Zombie", 
        // "Miss McAlienface", 
        "Ron", 
        "Lollita", 
        "Visiri Capt'n", 
        "Gentleman", 
        "Robyna Hoots", 
        // "Miss McAlienface", 
        // "Government Troop", 
        // "Zombieee", 
        "Saboteur Pianist", 
        "Arctic Pole Boy", 
        // "Arctic Pole Boy", 
        // "Ardent Starhuntress", 
        // "Dennis", 
        // "Captain Mack Swallow", 
        // "Verunus", 
        "Jesus", 
        "Vivien", 
        "Jaiden", 
        // "Dr Mew", 
    
    };
    
    
    public static Dictionary<string, List<Tuple<int, int>>> synthesisRules = new Dictionary<string, List<Tuple<int, int>>>();
    public static Dictionary<string, int> nameToId = new Dictionary<string, int>();
    public static Dictionary<int, string> idToName = new Dictionary<int, string>();
    public static Dictionary<string, int> lowLevelCharacters = new Dictionary<string, int>();

    public class Node
    {
        public string Name;
        public int Id;
        public int Level;
        public bool Visit;
        public bool SaveFlag;
        public Node Parent;
        public List<Tuple<Node, Node>> Children;
        public int UseCount;
        public bool Disable;
        public List<Tuple<string, string>> CombineForm;
        public int Uid;

        public string GetName()
        {
            return Crews.Get(Name).Info();
        }
        public Node(string name, int level, Node parent)
        {
            Name = name;
            Level = level;
            Parent = parent;
            Children = new List<Tuple<Node, Node>>();
            UseCount = 0;
            Disable = false;
            CombineForm = new List<Tuple<string, string>>();
            Id = nameToId.ContainsKey(name) ? nameToId[name] : 0;
            if (Id == 0)
            {
                Console.WriteLine($"create node ERROR , name:{name}");
            }
            Uid = ++uid;
        }

        public void SetFlag(bool val)
        {
            SaveFlag = val;
        }

        public void AddCount(int val)
        {
            UseCount += val;
        }

        public void AddChildren(Node l, Node r)
        {
            Children.Add(new Tuple<Node, Node>(l, r));
            if (exMode)
            {
                // Implement random shuffle for children here
            }
        }
        public void ShuffleChildren()
        {
            Random rng = new Random();
            int n = Children.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Tuple<Node, Node> value = Children[k];
                Children[k] = Children[n];
                Children[n] = value;
            }
        }

        public void SavePath(Node l, Node r)
        {
            CombineForm.Add(new Tuple<string, string>(l.Name, r.Name));

            AddDicCount(curOwned, l.Name, -1);
            AddDicCount(curOwned, r.Name, -1);
            AddDicCount(curOwned, Name, 1);
            l.AddCount(-1);
            r.AddCount(-1);
            AddCount(1);
            l.SetFlag(true);
            r.SetFlag(true);
        }

        public void ResetData()
        {
            if (Disable)
                return;

            Disable = true;
            AddDicCount(curOwned, Name, -UseCount);
            UseCount = 0;
            SetFlag(false);
            foreach (var child in Children)
            {
                child.Item1.ResetData();
                child.Item2.ResetData();
            }
        }
    }

    static int uid = 0;
    static bool exMode = false;
    static Dictionary<string, int> curOwned = new Dictionary<string, int>();

  
    public static void Test()
    {

        for (int i = 0; i < targetCharacters.Length; i++)
        {
            var name = targetCharacters[i];
            if (Crews.CNNameToId.ContainsKey(name))
            {
                var id = Crews.CNNameToId[name];
                targetCharacters[i] = Crews.All[id].Name;
            }
        }
        
        for (int i = 0; i < inventory.Length; i++)
        {
            var name = inventory[i];
            if (Crews.CNNameToId.ContainsKey(name))
            {
                var id = Crews.CNNameToId[name];
                inventory[i] = Crews.All[id].Name;
            }
        }
        
        LoadSynthesisRules(jsonFile);
        InitLowLevelCharacters(inventory);

        exMode = true;
        for (int i = 0; i < loopCount; i++)
        {
            Mode1(i);
        }
    }

    static void LoadSynthesisRules(string jsonFile)
    {
        string jsonText = File.ReadAllText(jsonFile);
        dynamic data = JsonHelper.DeserializeObject(jsonText);

        foreach (var character in data["characters"])
        {
            string name = character["character_design_name"];
            var rules = new List<Tuple<int, int>>();
            foreach (var rule in character["prestige_paths"])
            {
                if (rule.Count == 2)
                {
                    rules.Add(new Tuple<int, int>((int)rule[0], (int)rule[1]));
                }
            }
            synthesisRules[name] = rules;
        }
    }

    static void InitLowLevelCharacters(string[] inventory)
    {
        foreach (var v in inventory)
        {
            if (!lowLevelCharacters.ContainsKey(v))
            {
                lowLevelCharacters[v] = 0;
            }
            lowLevelCharacters[v]++;
        }
    }

    static void AddDicCount(Dictionary<string, int> dic, string name, int val)
    {
        if (!dic.ContainsKey(name))
        {
            dic[name] = 0;
        }
        dic[name] += val;
    }

    static void Mode1(int idx)
    {
        curOwned = new Dictionary<string, int>(lowLevelCharacters);
        List<string> validOutput = new List<string>();
        int costCrewCount = 0;
        foreach (var targetCharacter in targetCharacters)
        {
            Node root = CreateNode(synthesisRules, targetCharacter, 6, null);
            VisitNodes(root);
            root.SetFlag(true);
            root.Disable = false;
            int count = 0;
            PrintPath(root, new List<string>(), ref count, validOutput, ref costCrewCount);
            if (count >= 1)
            {
                validOutput.Add("");
            }
            else
            {
                validOutput.Clear();
                break;
            }
        }

        foreach (var s in validOutput)
        {
            Console.WriteLine(s);
        }
        
        if (validOutput.Count > 0)
        {
            Console.WriteLine($" -----------------success {idx + 1} ----------------- 总消耗：{costCrewCount}");
        }
        else
        {
            Console.Write($" {idx + 1}.");
        }
    }

    static Node CreateNode(Dictionary<string, List<Tuple<int, int>>> rules, string current, int lv, Node parent)
    {
        Node node = new Node(current, lv, parent);

        if (rules.ContainsKey(current))
        {
            foreach (var rule in rules[current])
            {
                Node l = CreateNode(rules, idToName.ContainsKey(rule.Item1) ? idToName[rule.Item1] : rule.Item1.ToString(), lv - 1, node);
                Node r = CreateNode(rules, idToName.ContainsKey(rule.Item2) ? idToName[rule.Item2] : rule.Item2.ToString(), lv - 1, node);
                node.AddChildren(l, r);
            }
        }
        node.ShuffleChildren();
        return node;
    }

    static void PrintPath(Node node, List<string> useList, ref int printPathIdx, List<string> strOut,ref int costCrewCount)
    {
        if (node.Disable)
            return;
        
        foreach (var child in node.Children)
        {
            PrintPath(child.Item1, useList, ref printPathIdx, strOut, ref costCrewCount);
            
            PrintPath(child.Item2, useList, ref printPathIdx, strOut, ref costCrewCount);
            if (child.Item1.SaveFlag && child.Item2.SaveFlag)
            {
                strOut.Add($"{node.GetName()} = {child.Item1.GetName()} + {child.Item2.GetName()}");

                if (lowLevelCharacters.ContainsKey(child.Item1.Name))
                {
                    useList.Add(child.Item1.GetName());
                }
                if (lowLevelCharacters.ContainsKey(child.Item2.Name))
                {
                    useList.Add(child.Item2.GetName());
                }

                if (node.Parent == null)
                {
                    strOut.Add($"消耗:{useList.Count} {string.Join(", ", useList)}");
                    costCrewCount += useList.Count;
                    useList.Clear();
                    printPathIdx++;
                }
            }
        }

    }

    static void VisitNodes(Node node, Node brother = null)
    {
        if (node.Disable)
            return;

        foreach (var child in node.Children)
        {
            if (node.Disable)
                return;

            VisitNodes(child.Item1);
            if (node.Disable)
                return;

            VisitNodes(child.Item2, child.Item1);

            bool savePath = false;
            bool isOwned = GetDicCount(curOwned, node.Name) > 0;

            if (brother != null && brother.Name == node.Name && GetDicCount(curOwned, node.Name) == 1)
            {
                isOwned = false; // 强行解决一下问题
            }

            if (!isOwned || node.Parent == null)
            {
                bool condition;
                if (child.Item1.Name == child.Item2.Name)
                {
                    condition = GetDicCount(curOwned, child.Item1.Name) > 1;
                }
                else
                {
                    condition = GetDicCount(curOwned, child.Item1.Name) > 0 && GetDicCount(curOwned, child.Item2.Name) > 0;
                }

                if (condition)
                {
                    node.SavePath(child.Item1, child.Item2);
                    savePath = true;
                }
            }

            if (savePath || isOwned)
            {
                if (node.Parent == null)
                {
                    if (!exMode)
                    {
                        curOwned = new Dictionary<string, int>(lowLevelCharacters);
                    }
                    if (exMode)
                    {
                        node.Disable = true;
                    }
                }
                else
                {
                    break;
                }
            }
            else
            {
                child.Item1.ResetData();
                child.Item2.ResetData();
            }
        }
    }

    static int GetDicCount(Dictionary<string, int> dic, string name)
    {
        return dic.ContainsKey(name) ? dic[name] : 0;
    }
}
