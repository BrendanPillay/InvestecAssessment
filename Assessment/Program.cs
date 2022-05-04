using Assessment.Models;
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
            var data = await GetPeople();

            var buddies = FilterBuddies(data);

            var groupedBuddies = buddies.GroupBy(x => x.BuddyNumber)
                                        .Select(y => new
                                        {
                                            Name = string.Join(", ", y.Select(x => x.Name))
                                        })
                                        .ToList();

            groupedBuddies.ForEach(x => Console.WriteLine($"\n Buddies: {x.Name} \n"));

        }

        private static List<Buddies> FilterBuddies(RawData filterData)
        {
            List<Buddies> buddies = new List<Buddies>();

            for (int i = 0; i < filterData.results.Count; i++)
            {
                var bud = filterData.results.Where(x => x.films.SequenceEqual(filterData.results[i].films))
                                           .Select(y => new Buddies
                                           {
                                               BuddyNumber = i,
                                               Name = y.name,
                                               Films = y.films
                                           }).ToList();

                var buddyNotAdded = !buddies.Any(x => bud.Any(y => y.Name == x.Name));

                if (buddyNotAdded)
                {
                    buddies.AddRange(bud);
                }
            }
            return buddies;

        }

        private static async Task<RawData> GetPeople()
        {
            var listRawData = new RawData() { results = new List<FilmDetails>() };

            var path = "https://swapi.dev/api/people";

            var client = new RestClient(path);
            var request = new RestRequest();

            var response = await client.ExecuteAsync(request);
            var pagedRequest = JsonSerializer.Deserialize<RawData>(response.Content);

            listRawData.results.AddRange(pagedRequest.results);

            if (listRawData != null)
            {
                while (!string.IsNullOrWhiteSpace(pagedRequest.next))
                {
                    var pagedClient = new RestClient(pagedRequest.next);
                    response = await pagedClient.ExecuteAsync(request);

                    pagedRequest = JsonSerializer.Deserialize<RawData>(response.Content);
                    listRawData.results.AddRange(pagedRequest.results);
                }
            }

            return listRawData;
        }
    }
}
