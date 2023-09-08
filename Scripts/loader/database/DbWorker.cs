using System;
using System.Collections.Generic;
namespace loader.database
{
    public enum SkillId
    {
        None,
        Miedic,
        Scientist,
        Technic,
        Cook,
        Farmer,
        Cleaner,
        Constructor,
        Carrier,
        Prepper,

        Last
    }

    public class DbSkillVal
    {
        public int Level;
        public int Progress;
        public SkillId Type;
    }

    public class DbWorker : RenderIconContainer
    {
        [NonSerialized] public string Name;
        public List<DbSkillVal> Skills;

        public int Skill(SkillId skillId)
        {
            for (var i = 0; Skills != null && i < Skills.Count; ++i)
            {
                if ((int)Skills[i].Type == (int)skillId)
                {
                    return Skills[i].Level;
                }
            }

            return 0;
        }

        public static DbWorker Random(SkillId prioroty = SkillId.None)
        {
            var worker = new DbWorker();

            worker.Skills = new List<DbSkillVal>();
            for (var s = 1; s <= (int)SkillId.Last; ++s)
            {
                var skillVal = new DbSkillVal
                {
                    Type = (SkillId)s, Level = UnityEngine.Random.Range(0, 5), Progress = 0
                };
                if (s == (int)prioroty)
                {
                    skillVal.Level = 7;
                }
                worker.Skills.Add(skillVal);
            }
            return worker;
        }
    }
}
