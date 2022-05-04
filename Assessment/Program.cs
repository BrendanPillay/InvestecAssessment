using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assessment
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var path = "https://swapi.dev/api/people";

            var data = await ExtractData(path);

            var buddies = FilterBuddies(data.Content);


            buddies.ForEach(x => Console.WriteLine($"Name: {x.name}"));
        }

        private static List<Buddies> FilterBuddies(string data)
        {
           var listBuddies = new List<Buddies>();

            var filterData = JsonSerializer.Deserialize<RawData>(data);


            //var result = filterData.results.GroupBy(x=> x.)

            //for (int i = 0; i < filterData.results.Count; i++)
            //{
            //    var buddy = filterData.results[i];
            //}

            var result = filterData.results.Where(x => filterData.results.Where(y => y != x)
                                            .Any(z => z.films.SequenceEqual(x.films)))
                                            .Select(data => new Buddies
                                            {
                                                name = data.name,
                                                films = data.films
                                            }).ToList();

            //var result2 = filterData.results
            //                            .Where(y=> y.films
            //                            .Any(filterData.results.Select(x=> x.films))



            //var result = filterData.results.Any(y => y.films)

            return result;

        }

        private static async Task<RestResponse> ExtractData(string path)
        {
            var client = new RestClient(path);
            var request = new RestRequest();

            var response = await client.ExecuteAsync(request);

            //var mappedResponse = JsonSerializer.Deserialize<List<Buddies>>(response.Content);

            return response;
        }
    }

    public class RawData
    {
        public List<Buddies> results { get; set; }
    }

    public class Buddies
    {
        public string name{ get; set; }
        public List<string> films { get; set; }
    }
}
