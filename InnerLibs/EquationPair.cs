using System;
using System.Reflection;

namespace InnerLibs
{
    /// <summary>
    /// Representa um Par X,Y para operaçoes matemáticas
    /// </summary>
    public class EquationPair<T> where T : struct
    {
        public EquationPair(T? X, T? Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public bool IsComplete => X.HasValue && Y.HasValue;
        public bool IsNotComplete => !IsComplete;
        public bool MissX => !X.HasValue;
        public bool MissY => !Y.HasValue;
        public T? X { get; set; }
        public T? Y { get; set; }

        public static explicit operator EquationPair<T>(Tuple<T?, T?> Equation) => new EquationPair<T>(Equation.Item1, Equation.Item2);

        public static explicit operator Tuple<T?, T?>(EquationPair<T> Equation) => new Tuple<T?, T?>(Equation.X, Equation.Y);

        public PropertyInfo GetMissing()
        {
            if (MissX)
            {
                return this.GetProperty("X");
            }

            if (MissY)
            {
                return this.GetProperty("Y");
            }

            return null;
        }

        public void SetMissing(T value)
        {
            if (GetMissing() != null)
            {
                GetMissing().SetValue(this, value);
            }
        }

        public T?[] ToArray() => new[] { X, Y };
    }
}