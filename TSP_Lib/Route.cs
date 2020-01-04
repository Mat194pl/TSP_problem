using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Lib
{
    public class Route
    {
        List<int> route;

        public Route(int nmbOfCities)
        {
            route = new List<int>();
            Random rng = new Random();

            while (route.Count < nmbOfCities)
            {
                int nextCity = rng.Next(0, nmbOfCities);
                if (!route.Contains(nextCity))
                {
                    route.Add(nextCity);
                }
            } 
        }

        // based on https://towardsdatascience.com/evolution-of-a-salesman-a-complete-genetic-algorithm-tutorial-for-python-6fe5d2b3ca35
        // 
        public Route(Route firstParent, Route secondParent)
        {
            Random rng = new Random();
            int geneA = rng.Next(0, firstParent.route.Count);
            int geneB = rng.Next(0, secondParent.route.Count);

            int startGene = Math.Min(geneA, geneB);
            int endGene = Math.Max(geneA, geneB);

            List<int> firstParentSubset = new List<int>();
            route = new List<int>();

            for (int i = startGene; i < endGene; i++)
            {
                firstParentSubset.Add(firstParent.route[i]);
            }

            for (int i = 0; i < startGene; i++)
            {
                int idx = 0;
                while (idx < secondParent.route.Count)
                {
                    if (!firstParentSubset.Contains(secondParent.route[idx]) && !route.Contains(secondParent.route[idx]))
                    {
                        route.Add(secondParent.route[idx]);
                        break;
                    }
                    idx++;
                }
            }

            route.AddRange(firstParentSubset);

            for (int i = 0; i < secondParent.route.Count - endGene; i++)
            {
                int idx = 0;
                while (idx < secondParent.route.Count)
                {
                    if (!firstParentSubset.Contains(secondParent.route[idx]) && !route.Contains(secondParent.route[idx]))
                    {
                        route.Add(secondParent.route[idx]);
                        break;
                    }
                    idx++;
                }
            }
        }

        public void Mutate(int nmbOfSwaps)
        {
            Random rng = new Random();   
            for (int i = 0; i < nmbOfSwaps; i++)
            {
                int firstGeneIdx = rng.Next(0, route.Count);
                int secondGeneIdx = rng.Next(0, route.Count);

                int temp = route[firstGeneIdx];
                route[firstGeneIdx] = route[secondGeneIdx];
                route[secondGeneIdx] = temp;
            }
        }

        public double GetRouteDistance(CitiesMap citiesMap)
        {
            double dst = 0.0f;

            for (int i = 0; i < route.Count - 1; i++)
            {
                dst += citiesMap.GetDistanceBetweenCities(route[i], route[i + 1]);
            }

            dst += citiesMap.GetDistanceBetweenCities(route[route.Count - 1], route[0]);
            return dst;
        }
    }
}
