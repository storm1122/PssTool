using System.Collections.Generic;

namespace ClassLibrary
{
    public enum AttrType
    {
        HP,
        Attack,
        Repair,
        Ability,
        Stamina,
        Pilot,
        Science,
        Engineer,
        Weapon,
        FireResistance,
        TrainingCapacity,
        RunSpeed,
        WalkingSpeed,
        Max,
    }

    public enum Rarity
    {
        Unknow,
        Special,
        Common,//1
        Elite,//2
        Unique,//3
        Epic,//4
        Hero,//5
        Legendary,//7
    }
    
    public class Crew
    {
        public string Id;
        public string Name;
        public string NameCn;
        public Rarity Rarity;
        private Dictionary<AttrType, float> Attr = new();

        public int IdNum => int.Parse(Id);
        
        public string RarityStar()
        {
            int star = 0;
            if (Rarity == Rarity.Common || Rarity == Rarity.Elite || 
                Rarity == Rarity.Unique || Rarity == Rarity.Epic ||
                Rarity == Rarity.Hero)
            {
                star = (int)Rarity - 1;
            }
            else if (Rarity == Rarity.Special)
                star = 6;
            else if (Rarity == Rarity.Legendary)
                star = 7;

            if (star > 0)
            {
                return $"{star}*";
            }
            return "Unknow";
        }

        public string AllInfo()
        {
            var str = $"no:{Id} {Name}{NameCn} {RarityStar()}";
            return str;
        }
        public string Info()
        {
            if (ConstValue.OutputEn)
            {
                return $"{Name}{RarityStar()}";
            }
            else
            {
                return $"{NameCn}{RarityStar()}";
            }
        }
        
        public float GetAttr(AttrType attrType)
        {
            Attr.TryGetValue(attrType, out var value);
            return value;
        }

        public void SetAttr(AttrType attrType, float val)
        {
            if (!Attr.ContainsKey(attrType))
            {
                Attr.Add(attrType, 0);
            }
            Attr[attrType] = val;
        }
    }
}