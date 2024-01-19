namespace SunkCost.HH.Modules.GridSystem
{
    public class GridCoordinate
    {
        public readonly int X;
        public readonly int Z;

        public GridCoordinate(int x, int z)
        {
            X = x;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (GridCoordinate)obj;
            return X == other.X && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return (X, Z).GetHashCode();
        }

        public static bool operator ==(GridCoordinate left, GridCoordinate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GridCoordinate left, GridCoordinate right)
        {
            return !Equals(left, right);
        }
    }
}