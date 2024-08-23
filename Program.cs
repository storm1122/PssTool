using System;  
using System.Threading.Tasks;
using ClassLibrary;

class Program  
{
    static async Task Main(string[] args)
    {
        
        if(Enum.TryParse(ConstValue.Get("Lan"),out ConstValue.OutputLanguage lan))
        {
            ConstValue.OutputLan = lan;
        }
        
        
        if (args.Length == 0)
        {
            // CrewPrestige.Enter(false, 5000);
            CrewTraining.Enter();
            // CrewPrestigeRandom.Enter();
            // CrewPrestige.Enter();
            return;
        }
        
        if (args[0].Equals("CheckConfig"))
        {
           
            await GenerateConfig.CheckDesignVersion();
            
            if (args.Length >= 2)
            {
                if(Enum.TryParse(args[1],out ConstValue.OutputLanguage ret))
                {
                    await ConstValue.Set("Lan", ret.ToString());
                    ConstValue.OutputLan = ret;
                }
            }
            Crews.Init();
        }
        else if (args[0].Equals("Training"))
        {
            CrewTraining.Enter();
        }
        else if (args[0].Equals("PrestigeRandom"))
        {
            CrewPrestigeRandom.Enter();
        }
        else if (args[0].Equals("Prestige"))
        {
            if (args.Length >= 2)
            {
                int count = int.Parse(args[1]);
                CrewPrestige.Enter(count);
            }
            else
            {
                CrewPrestige.Enter(5000);
            }
        }
        Console.WriteLine(" ===========================");
        Console.WriteLine(" ========== 结束 ============");
        Console.WriteLine(" ===========================");
    }

  
}

