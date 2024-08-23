using System;
using System.Collections.Generic;
using ClassLibrary;

public partial class CrewTraining
{
    private static TrainingType _curTrain = TrainingType.None;
    private static ExpressionType _curExpression = ExpressionType.None;

    public static void Test(string crewName, List<(AttrType,int)> list)
    {
        var crew = Crews.Get(crewName);
        var attr = new TrainAttr();
        attr[AttrType.Max] = (int)crew.GetAttr(AttrType.TrainingCapacity);

        for (int i = 0; i < list.Count; i++)
        {
            var ele = list[i];
            _curTrain = TrainingType.绿训;
            _curExpression = ExpressionType.微笑;
            attr.Train(ele.Item1, _curTrain, _curExpression, ele.Item2);
        }
        
        
        string str = "";
        for (int i = 0; i <= (int)AttrType.Max; i++)
        {
            if (attr[(AttrType)i] > 0)
            {
                var attrType = (AttrType)i;
                var trainVal = attr[(AttrType)i];
                var val = crew.GetAttr(attrType) + crew.GetAttr(attrType) * trainVal * 0.01f;
                if (val > 0)
                {
                    str += $"{(AttrType)i}: {trainVal} => {val}\n";
                }
                else
                {
                    str += $"{(AttrType)i}:{trainVal}   ";
                }
            }
        }
        
        
        Console.WriteLine($"训练度  {attr.TrainedTotalAmount}/{attr.TotalTrainingAmount}");
        
        Console.WriteLine(str);
    }


    public static void Test0()
    {
        var crew = Crews.Get("金刚");
        
        var attr = new TrainAttr();
        attr[AttrType.Max] = (int)crew.GetAttr(AttrType.TrainingCapacity);

        _curTrain = TrainingType.绿训;
        _curExpression = ExpressionType.微笑;
        attr.Train(AttrType.HP, _curTrain, _curExpression, 34);
        
        _curTrain = TrainingType.绿训;
        _curExpression = ExpressionType.微笑;
        attr.Train(AttrType.Attack, _curTrain, _curExpression, 48);

        
        _curTrain = TrainingType.绿训;
        _curExpression = ExpressionType.微笑;
        attr.Train(AttrType.Stamina, _curTrain, _curExpression, 20);


        string str = "";
        for (int i = 0; i <= (int)AttrType.Max; i++)
        {
            if (attr[(AttrType)i] > 0)
            {
                var attrType = (AttrType)i;
                var trainVal = attr[(AttrType)i];
                var val = crew.GetAttr(attrType) + crew.GetAttr(attrType) * trainVal * 0.01f;
                if (val > 0)
                {
                    str += $"{(AttrType)i}: {trainVal} => {val}\n";
                }
                else
                {
                    str += $"{(AttrType)i}:{trainVal}   ";
                }
            }
        }
        
        
        Console.WriteLine($"训练度  {attr.TrainedTotalAmount}/{attr.TotalTrainingAmount}");
        
        Console.WriteLine(str);
    }



    
    public class TrainAttr
    {
        private Dictionary<AttrType, int> Dic = new();
        public Crew Crew;
    
        public int this[AttrType attrType]
        {
            get => GetByKey(attrType);
            set => Insert(attrType, value);
        }

        public int TotalTrainingAmount => GetByKey(AttrType.Max);

        public int TrainedTotalAmount 
        {
            get
            {
                var val = 0;
                foreach (var ele in Dic)
                    if (ele.Key != AttrType.Max)
                        val += ele.Value;
                return val;
            }
        }

        int GetByKey(AttrType attrType)
        {
            Dic.TryGetValue(attrType, out var value);
            return value;
        }

        void Insert(AttrType attrType, int val)
        {
            if (!Dic.ContainsKey(attrType))
            {
                Dic.Add(attrType, 0);
            }
            Dic[attrType] = val;
        }

        public (double,double) CheckRet(AttrType trainingAttribute, TrainingType trainingType, ExpressionType expression)
        {
            double result1 = CalculateAttributeIncrease(trainingAttribute,TotalTrainingAmount, TrainedTotalAmount, this[trainingAttribute], trainingType, expression);
            double result2 = CalculateAttributeDeviation(trainingAttribute,TotalTrainingAmount,  TrainedTotalAmount, trainingType, expression);
            
            Console.WriteLine($"{this[trainingAttribute]} {expression}\t {result1:0.00} / {result2:0.00}");
            return (result1, result2);
        }
        public void Train(AttrType trainingAttribute, TrainingType trainingType, ExpressionType expression , int targetVal = 0 )
        {
            if (targetVal == 0)
            {
                targetVal = this[AttrType.Max];
            }

            if (this[trainingAttribute] >= targetVal)
            {
                Console.WriteLine($"{_curExpression} {_curTrain}  {trainingAttribute}:{this[trainingAttribute]} ");
                return;
            }
   
            double result1 = CalculateAttributeIncrease(trainingAttribute, TotalTrainingAmount, TrainedTotalAmount, this[trainingAttribute], trainingType, expression);
            double result2 = CalculateAttributeDeviation(trainingAttribute, TotalTrainingAmount,  TrainedTotalAmount, trainingType, expression);
        
        
            if (result1 >= 1 && result2 < 1 )
            {

                // if (trainingType == TrainingType.lv6药)
                // {
                //     this[trainingAttribute] += 2;
                // }
                // else if (trainingType == TrainingType.lv7药)
                // {
                //     this[trainingAttribute] += 4;
                // }
                // else
                {
                    this[trainingAttribute]++;
                }

                if (_curTrain != trainingType || _curExpression != expression)
                {
                    if (this[trainingAttribute] - 1 > 0)
                    {
                        Console.WriteLine($"{_curExpression} {_curTrain}  {trainingAttribute}:{this[trainingAttribute] - 1} ");
                    }
                    _curTrain = trainingType;
                    _curExpression = expression;
                }
            
                expression = ExpressionType.大笑;
                Train(trainingAttribute, trainingType, expression, targetVal);
                return;
            }

            if (result1 >= 1 && result2 >= 1 && expression + 1 < ExpressionType.Max)
            {
                expression++;
                Train(trainingAttribute, trainingType, expression, targetVal);
                return;
            }

            if (result1 < 1 && trainingType + 1 < TrainingType.lv1药)
            {
                trainingType++;
                expression = ExpressionType.大笑;
                Train(trainingAttribute, trainingType, expression, targetVal);
                return;
            }

            if (this[trainingAttribute] > 0)
            {
                Console.WriteLine($"{_curExpression} {_curTrain}  {trainingAttribute}:{this[trainingAttribute]} ");
            }
        }
    }
    
    public enum TrainingType
    {
        None,
        绿训,
        蓝训,
        金训,
        lv1药,
        lv2药,
        lv3药,
        lv4药,
        lv5药,
        lv6药,
        lv7药,
        Max,
    }

    public enum ExpressionType
    {
        None,
        大笑,              //大笑
        微笑,     //微笑
        平嘴,
        竖眉,
        八字眉,
        三条线,
        流汗,
        小哭,
        大哭,
        黑脸,
        升天,
        Max
    }
    
   

    
    // Training attribute table
    static Dictionary<TrainingType, (int increaseValue, int deviationValue)> trainingAttributeTable = new Dictionary<TrainingType, (int, int)>
    {
        {TrainingType.绿训, (4, 1)},
        {TrainingType.蓝训, (8, 1)},
        {TrainingType.金训, (12, 2)},
        {TrainingType.lv1药, (2, 1)},
        {TrainingType.lv2药, (4, 2)},
        {TrainingType.lv3药, (8, 3)},
        {TrainingType.lv4药, (16, 5)},
        {TrainingType.lv5药, (25, 12)},
        {TrainingType.lv6药, (50, 8)},
        {TrainingType.lv7药, (100, 3)},
    };
    
    
    static Dictionary<TrainingType, (int increaseValue, int deviationValue)> trainingAttributeTable2 = new Dictionary<TrainingType, (int, int)>
    {
        {TrainingType.绿训, (5, 3)},
        {TrainingType.蓝训, (8, 2)},
        {TrainingType.金训, (16, 3)},
        {TrainingType.lv1药, (8, 1)},
        {TrainingType.lv2药, (12, 1)},
        {TrainingType.lv3药, (16, 2)},
        {TrainingType.lv4药, (24, 3)},
        {TrainingType.lv5药, (25, 4)},
        {TrainingType.lv6药, (50, 3)},
        {TrainingType.lv7药, (110, 2)},
    };

    // Expression multiplier table
    static Dictionary<ExpressionType, double> expressionMultiplierTable = new Dictionary<ExpressionType, double>
    {
        {ExpressionType.大笑, 1},
        {ExpressionType.微笑, 0.5},
        {ExpressionType.平嘴, 0.5},
        {ExpressionType.竖眉, 0.5},
        {ExpressionType.八字眉, 0.5},
        {ExpressionType.三条线, 0.5},
        {ExpressionType.流汗, 0.33},
        {ExpressionType.小哭, 0.33},
        {ExpressionType.大哭, 0.33},
        {ExpressionType.黑脸, 0.33},
        {ExpressionType.升天, 0.33},
        // Other data...
    };
    static double CalculateAttributeIncrease(AttrType trainingAttribute, int totalTrainingAmount, int trainedTotalAmount, int currentTrainingLevel, TrainingType trainingType, ExpressionType expression)
    {
        int attributeTrainedTotalAmount = currentTrainingLevel; // Simplified processing, assuming currentTrainingLevel is the total amount of attribute training
        int attributeTotalTrainingAmount = totalTrainingAmount;

        double part1 = (attributeTotalTrainingAmount - attributeTrainedTotalAmount) / (double)attributeTotalTrainingAmount;
        double part2 = (attributeTotalTrainingAmount - trainedTotalAmount) / (double)attributeTotalTrainingAmount;

        int increaseValue = trainingAttributeTable[trainingType].increaseValue;
        
        if (trainingAttribute == AttrType.Stamina)
        {
            increaseValue = trainingAttributeTable2[trainingType].increaseValue;
        }
        
        double expressionMultiplier = expressionMultiplierTable[expression];

        double part3 = (increaseValue * 0.01) * (expressionMultiplier * 100);

        return part1 * part2 * part3;
    }

    static double CalculateAttributeDeviation(AttrType trainingAttribute, int totalTrainingAmount, int trainedTotalAmount, TrainingType trainingType, ExpressionType expression)
    {
        double part1 = (totalTrainingAmount - 0) / (double)totalTrainingAmount;
        double part2 = (totalTrainingAmount - trainedTotalAmount) / (double)totalTrainingAmount;

        int deviationValue = trainingAttributeTable[trainingType].deviationValue;
        
        // if (trainingAttribute == AttrType.Stamina)
        // {
        //     deviationValue = trainingAttributeTable2[trainingType].deviationValue;
        // }
        
        double expressionMultiplier = expressionMultiplierTable[expression];

        double part3 = (deviationValue * 0.01) * (expressionMultiplier * 100);

        return part1 * part2 * part3;
    }
}
