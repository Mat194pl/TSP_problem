using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Lib
{
    public class NewPopulationParameters
    {
        private int eliteSize;

        public int EliteSize
        {
            get
            {
                return eliteSize;
            }
            set
            {
                eliteSize = value > 100 ? 100 : value;
            }
        }

        public int MutationRate;
        public int MutationSize;
    }
    
    public class Population
    {
        public CitiesMap citiesMap;
        public int SizeOfPopulation;
        private double bestRouteDistance;
        private int bestRouteDistanceIdx;
        public double BestRouteDistance
        {
            get
            {
                return bestRouteDistance;
            }
        }

        List<Route> routes;
        public Population(int nmbOfCities, int sizeOfPopulation, CitiesMap citiesMap)
        {
            routes = new List<Route>();

            for (int i = 0; i < sizeOfPopulation; i++)
            {
                routes.Add(new Route(nmbOfCities));
            }
            
            this.citiesMap = citiesMap;  
            SizeOfPopulation = sizeOfPopulation;
            FindBestRoute();
        }

        private void FindBestRoute()
        {
            for (int i = 0; i < SizeOfPopulation; i++)
            {
                if (i == 0)
                {
                    bestRouteDistance = routes[0].GetRouteDistance(citiesMap);
                    bestRouteDistanceIdx = 0;
                }
                else
                {
                    if (routes[i].GetRouteDistance(citiesMap) < bestRouteDistance)
                    {
                        bestRouteDistance = routes[i].GetRouteDistance(citiesMap);
                        bestRouteDistanceIdx = i;
                    }
                }
            }
        }

        public Population(Population oldPopulation, NewPopulationParameters newPopulationParameters)
        {
            citiesMap = oldPopulation.citiesMap;
            SizeOfPopulation = oldPopulation.SizeOfPopulation;

            int sizeOfPopulationToPreserve = SizeOfPopulation / 2;
            if (sizeOfPopulationToPreserve % 2 != 0)
            {
                sizeOfPopulationToPreserve++;
            }
            int sizeOfElite = ((newPopulationParameters.EliteSize * sizeOfPopulationToPreserve)) / 100;
            int sizeOfNonElite = sizeOfPopulationToPreserve - sizeOfElite;
            List<Route> sortedOldPopulation = oldPopulation.routes.OrderBy(x => x.GetRouteDistance(citiesMap)).ToList();
            List<Route> newPopulation = sortedOldPopulation.Take(sizeOfElite).ToList();
            sortedOldPopulation.RemoveRange(0, sizeOfElite);

            Random random = new Random();
            while (newPopulation.Count < sizeOfPopulationToPreserve)
            {
                int idx = 0;// random.Next(0, sortedOldPopulation.Count);
                newPopulation.Add(sortedOldPopulation[idx]);
                sortedOldPopulation.RemoveAt(idx);
            }
            
            while (newPopulation.Count < SizeOfPopulation)
            {
                int firstParentIdx = 0;
                newPopulation.Add(new Route(newPopulation[firstParentIdx * 2], newPopulation[firstParentIdx * 2 + 1]));
                firstParentIdx++;
            }

            int nmbOfMutationSwaps = newPopulationParameters.MutationRate;

            foreach(Route route in newPopulation)
            {
                if (random.Next(0, 100) < newPopulationParameters.MutationRate)
                {
                    route.Mutate(nmbOfMutationSwaps);
                }
            }

            routes = newPopulation;
            FindBestRoute();
        }

        public IEnumerable<double> GetDistanceRank()
        {
            List<Route> sortedRoutes = routes.OrderBy(x => x.GetRouteDistance(citiesMap)).ToList();
            List<double> distances = new List<double>();
            for (int i = 0; i < sortedRoutes.Count; i++)
            {
                distances.Add(sortedRoutes[i].GetRouteDistance(citiesMap));
            }

            return distances;
        }
    }
}
