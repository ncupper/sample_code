using loader;
using loader.database;
namespace game.helpers
{
    internal static class HelperRpg
    {
        public static float CalcSkill(float val, SkillId skillId, DbWorker worker)
        {
            int skillVal = worker.Skill(skillId);
            DbSkill skillData = DataStorage.Constants.GetSkill(skillId);
            while (skillVal < skillData.Zero)
            {
                val *= skillData.Penalty;
                ++skillVal;
            }
            while (skillVal > skillData.Zero)
            {
                val *= skillData.Bonus;
                --skillVal;
            }

            return val;
        }
    }
}
