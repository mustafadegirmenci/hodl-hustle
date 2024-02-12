using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SunkCost.HH.Modules.CharacterSystem;
using SunkCost.HH.Modules.PhotographySystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    [Serializable]
    public class EmployeeData
    {
        private string _id;
        private string _customizationData;
        private int _qualityLevelBetween0To100;
        private string _name;
        private string _surname;
        private int _age;
        private EmployeeRoleType _role;

        private List<EmployeeSkill> _skills = new()
        {
            new EmployeeSkill(EmployeeSkillType.Trading),
            new EmployeeSkill(EmployeeSkillType.Analysis),
            new EmployeeSkill(EmployeeSkillType.RiskManagement),
            new EmployeeSkill(EmployeeSkillType.Communication),
            new EmployeeSkill(EmployeeSkillType.Marketing),
            new EmployeeSkill(EmployeeSkillType.Hardware),
            new EmployeeSkill(EmployeeSkillType.Programming)
        };

        public EmployeeData(string customizationData, int employeeQualityLevelBetween0To100)
        {
            _id = Guid.NewGuid().ToString();
            _name = EmployeeNameGenerator.instance.GenerateRandomName();
            _surname = EmployeeNameGenerator.instance.GenerateRandomSurname();
            _age = Random.Range(18, 40);
            _customizationData = customizationData;
            _qualityLevelBetween0To100 = employeeQualityLevelBetween0To100;

            RandomizeSkillLevels(employeeQualityLevelBetween0To100);
            DetermineRole();
            AssignAndBoostPrimarySkills();
            
            foreach (var skill in _skills.OrderByDescending(s => s.Level))
            {
                Debug.Log($"[{_role.ToString()}] {skill.Type.ToString()} {skill.IsPrimary} {skill.Level}");
            }
        }

        public string GetId() => _id;
        public int GetAge() => _age;
        public string GetName() => _name;
        public string GetSurname() => _surname;
        public EmployeeRoleType GetRole() => _role;
        public string GetCustomizationData() => _customizationData;
        public int GetOverallQualityLevel() => _qualityLevelBetween0To100;

        public async UniTask<Sprite> GetSpriteAsync()
        {
            var texture2D = await PhotoTaker.instance.TakePhoto(
                CharacterManager.instance.dummyCharacter.gameObject,
                from: PhotoDirection.Front,
                width: 200f,
                height: 200f,
                distance: 0.8f,
                cameraOffset: Vector3.up * 0.8f,
                lookAtOffset: Vector3.up * 0.8f
            );
            var sprite = PhotoConverter.instance.Texture2DToSprite(texture2D);

            return sprite;
        }

        private void RandomizeSkillLevels(int qualityLevelBetween0To100)
        {
            foreach (var skill in _skills)
            {
                var randomLevel = Mathf.Clamp(qualityLevelBetween0To100 + Random.Range(-20, 0), 0, 100);
                skill.Level = randomLevel;
            }
        }

        private void AssignAndBoostPrimarySkills()
        {
            var skillMappings = new Dictionary<EmployeeRoleType, EmployeeSkillType>
            {
                {
                    EmployeeRoleType.Trader,
                    
                    EmployeeSkillType.Trading | 
                    EmployeeSkillType.Analysis
                },
                {
                    EmployeeRoleType.TechnicalAnalyst,
                    
                    EmployeeSkillType.RiskManagement | 
                    EmployeeSkillType.Analysis
                },
                {
                    EmployeeRoleType.AlgorithmicTrader,
                    
                    EmployeeSkillType.Trading | 
                    EmployeeSkillType.Analysis |
                    EmployeeSkillType.Programming
                },
                {
                    EmployeeRoleType.Recruiter,
                    
                    EmployeeSkillType.Communication | 
                    EmployeeSkillType.Analysis
                },
                {
                    EmployeeRoleType.HumanResourcesSpecialist,
                    
                    EmployeeSkillType.Communication | 
                    EmployeeSkillType.LegalKnowledge
                },
                {
                    EmployeeRoleType.FinancialAdvisor,
                    
                    EmployeeSkillType.Trading | 
                    EmployeeSkillType.Communication
                },
            };

            foreach (var skill in _skills)
            {
                if (!skillMappings[_role].HasFlag(skill.Type))
                {
                    continue;
                }
                
                var boostedLevel = Mathf.Clamp(skill.Level + Random.Range(0, 20), 0, 100);
                skill.IsPrimary = true;
                skill.Level = boostedLevel;
            }
        }
        
        private void DetermineRole()
        {
            var roleValues = (EmployeeRoleType[])Enum.GetValues(typeof(EmployeeRoleType));
            _role = roleValues[Random.Range(0, roleValues.Length)];
        }
    }
}
