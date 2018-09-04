using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ColoArk.Models
{
    public class GiveawayDrop : BaseModel
    {
        public string Name { get; set; }
        public int LowerBound { get; set; }
        public int HigherBound { get; set; }
        public DropType Type { get; set; }

        public enum DropType
        {
            Armor,
            Blueprint,
            Creature,
            Resource,
            Structure,
            Weapon
        }
    }
}
