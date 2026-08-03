using CityAuthority.Data;

namespace CityAuthority.Emergency
{
    // Runtime unit-commitment tracking for one department (03 §13). "Unit" here
    // is whatever the department's resource counts represent (engine crew, patrol
    // unit, etc.) — the slice's departments each define exactly one deployable resource.
    public sealed class DepartmentCoverageState
    {
        private readonly DepartmentDefinition department;
        private int committedUnitCount;

        public DepartmentCoverageState(DepartmentDefinition department)
        {
            this.department = department;
        }

        public DepartmentDefinition Department => department;

        public int TotalUnitCount
        {
            get
            {
                var total = 0;
                foreach (var resource in department.Resources)
                {
                    total += resource.Count;
                }
                return total;
            }
        }

        public int UncommittedUnitCount
        {
            get
            {
                var uncommitted = TotalUnitCount - committedUnitCount;
                return uncommitted > 0 ? uncommitted : 0;
            }
        }

        public void CommitUnit()
        {
            if (committedUnitCount < TotalUnitCount)
            {
                committedUnitCount++;
            }
        }

        public void ReleaseUnit()
        {
            if (committedUnitCount > 0)
            {
                committedUnitCount--;
            }
        }
    }
}
