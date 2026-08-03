using System;
using System.Linq;
using CityAuthority.Data;
using NUnit.Framework;
using UnityEditor;

namespace CityAuthority.Tests.Editor
{
    public class SliceConfigTests
    {
        private const string SliceConfigAssetPath = "Assets/Data/Slice/SliceConfig_Default.asset";

        [Test]
        public void SliceConfig_HasExpectedShape()
        {
            var config = AssetDatabase.LoadAssetAtPath<SliceConfig>(SliceConfigAssetPath);

            Assert.IsNotNull(config, $"Expected a SliceConfig asset at {SliceConfigAssetPath}");
            Assert.IsNotNull(config.Region, "SliceConfig.Region must be assigned");
            Assert.IsNotNull(config.Mayor, "SliceConfig.Mayor must be assigned");

            Assert.AreEqual(5, config.Departments.Count, "Slice must define exactly 5 departments");
            CollectionAssert.AreEquivalent(
                Enum.GetValues(typeof(DepartmentType)),
                config.Departments.Select(d => d.DepartmentType).ToArray(),
                "Slice departments must cover every DepartmentType exactly once");

            foreach (var department in config.Departments)
            {
                Assert.GreaterOrEqual(department.StartingFacilityCount, 1,
                    $"{department.DepartmentType} must have at least one facility");
                Assert.IsTrue(department.Resources.Any(r => r.Count >= 1),
                    $"{department.DepartmentType} must define at least one staffed resource");
                Assert.IsFalse(string.IsNullOrWhiteSpace(department.RoleDescription),
                    $"{department.DepartmentType} must have a role description");
                Assert.AreEqual(OperatingPolicyLevel.Standard, department.OperatingPolicy,
                    $"{department.DepartmentType} must use the slice's fixed Standard policy");
            }

            CollectionAssert.AreEquivalent(
                new[] { ZoningCategory.SingleFamily, ZoningCategory.MixedUse },
                config.Region.AvailableZoningCategories,
                "Slice region must allow exactly Single-family and Mixed-use zoning");
            Assert.AreEqual(1f, config.Region.BaseLandValueIndex, "Slice region's base land value index must be 1.0");
        }
    }
}
