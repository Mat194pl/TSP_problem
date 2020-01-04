using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Lib
{
    public class City
    {
        public double X;
        public double Y;
        public int ID { get; }

        private static int id = 0;

        public static void ResetId()
        {
            id = 0;
        }

        public static int GetNewId()
        {
            int newId = id;
            id++;
            return newId;
        }

        public City (double X, double Y)
        {
            this.X = X;
            this.Y = Y;
            this.ID = GetNewId();
        }

        public City()
        {
            Random rng = new Random();
            X = rng.NextDouble() * 100.0;
            Y = rng.NextDouble() * 100.0;
            ID = GetNewId();
        }

        public double DistanceToOtherCity(City other)
        {
            return Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));
        }

        public override string ToString()
        {
            return string.Format("{0}: {1:0.00} {2:0.00}", ID, X, Y);
        }
    }
}
