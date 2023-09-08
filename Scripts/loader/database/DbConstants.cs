using System;
namespace loader.database
{
    [Serializable]
    internal class DbSkill
    {
        public SkillId Id;
        public int Zero = 10;
        public float Penalty = 1;
        public float Bonus = 1;
    }

    [Serializable]
    internal class DbConstants
    {
        public float TakeResTime;
        public float CraftResTime;
        public float ConstructBaseTime;
        public float BuildTime;
        public DbSkill[] Skills;

        public DbSkill GetSkill(SkillId skillId)
        {
            for (var i = 0; i < Skills.Length; ++i)
            {
                if (Skills[i].Id == skillId)
                {
                    return Skills[i];
                }
            }

            return new DbSkill
            {
                Id = skillId
            };
        }
    }
}
