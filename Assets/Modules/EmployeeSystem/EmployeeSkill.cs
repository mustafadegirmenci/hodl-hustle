namespace SunkCost.HH.Modules.EmployeeSystem
{
    public class EmployeeSkill
    {
        public readonly EmployeeSkillType Type;
        public bool IsPrimary;
        public int Level;

        public EmployeeSkill(EmployeeSkillType type, bool isPrimary = false, int level = 0)
        {
            IsPrimary = isPrimary;
            Type = type;
            Level = level;
        }
    }
}
