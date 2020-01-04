using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Lib
{
    public class CitiesMap
    {
        private List<City> Cities;
        private Dictionary<Tuple<int, int>, double> CityToCityDistanceTable;
        
        public CitiesMap(int numberOfCities)
        {
            Cities = new List<City>();
            CityToCityDistanceTable = new Dictionary<Tuple<int, int>, double>();

            for (int i = 0; i < numberOfCities; i++)
            {
                Cities.Add(new City());
            }

            for (int i = 0; i < numberOfCities; i++)
            {
                for (int j = 0; j < numberOfCities; j++)
                {
                    if (i != j)
                    {
                        CityToCityDistanceTable.Add(
                            new Tuple<int, int>(i, j),
                            Cities[i].DistanceToOtherCity(Cities[j]));
                    }
                }
            }
        }

        public double GetDistanceBetweenCities(City first, City second)
        {
            return CityToCityDistanceTable[new Tuple<int, int>(first.ID, second.ID)];
        }

        public double GetDistanceBetweenCities(int first, int second)
        {
            return CityToCityDistanceTable[new Tuple<int, int>(first, second)];
        }

        public void PrintCities()
        {
            foreach(City city in Cities)
            {
                Console.WriteLine(city.ToString());
            }
        }

        public void PrintDistances()
        {
            foreach (Tuple<int, int> pair in CityToCityDistanceTable.Keys)
            {
                Console.WriteLine(string.Format("{0} => {1}: {2:0.00}", pair.Item1, pair.Item2, CityToCityDistanceTable[pair]));
            }
        }
    }
}
