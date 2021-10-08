using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{
    /// <summary>
    /// Representa um Par X,Y para operaçoes matemáticas
    /// </summary>
    public class EquationPair
    {
        public EquationPair()
        {
        }

        public EquationPair(decimal? X, decimal? Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public decimal? X { get; set; }
        public decimal? Y { get; set; }

        public decimal?[] ToArray()
        {
            return new[] { X, Y };
        }

        public bool IsNotComplete
        {
            get
            {
                return !IsComplete;
            }
        }

        public bool IsComplete
        {
            get
            {
                return X.HasValue && Y.HasValue;
            }
        }

        public object MissX
        {
            get
            {
                return !X.HasValue;
            }
        }

        public object MissY
        {
            get
            {
                return !Y.HasValue;
            }
        }

        public PropertyInfo GetMissing()
        {
            if (Conversions.ToBoolean(MissX))
            {
                return this.GetProperty("X");
            }

            if (Conversions.ToBoolean(MissY))
            {
                return this.GetProperty("Y");
            }

            return null;
        }

        public void SetMissing(decimal value)
        {
            if (GetMissing() != null)
            {
                GetMissing().SetValue(this, value);
            }
        }
    }
}