using System;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    [Flags]
    public enum EmployeeSkillType
    {
        Trading = 1 << 0,
        Analysis = 1 << 1,
        RiskManagement = 1 << 2,
        Communication = 1 << 3,
        Marketing = 1 << 4,
        Hardware = 1 << 5,
        Programming = 1 << 6,
        LegalKnowledge = 1 << 7,
    }
}