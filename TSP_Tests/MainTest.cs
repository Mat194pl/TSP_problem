using NUnit.Framework;
using TSP_Lib;

namespace TSP_Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GenerateCitiesMap()
        {
            CitiesMap citiesMap = new CitiesMap(5);
            citiesMap.PrintCities();
            citiesMap.PrintDistances();
        }


    }
}