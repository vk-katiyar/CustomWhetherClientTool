using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace CustomWeatherClientTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter the Name of the City.....");

                string cityName = Console.ReadLine();  // Reading City Name from User

                List<City> cities = LoadJson();        // Loading Json for city with latitude and longitude
                City city = null;

                if (cities.Any())
                {
                    city = cities.FirstOrDefault(x => x.city == cityName);  // Getting city details 
                }

                if (city != null)
                {
                    GetCityWeatherDetails(city);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static void GetCityWeatherDetails(City city)
        {
            HttpClient client = new HttpClient();
            try
            {
                var latitude = city.lat;
                var longitude = city.lng;

                string URL = "https://api.open-meteo.com/v1/forecast";
                string urlParameters = "?latitude=" + latitude + "&longitude=" + longitude + "&current_weather=true";

                client.BaseAddress = new Uri(URL);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    Response data = JsonConvert.DeserializeObject<Response>(result);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Weather Details of {0} are", city.city);
                    Console.WriteLine("Temperature --> {0}", data?.current_weather?.temperature);
                    Console.WriteLine("WindSpeed --> {0}", data?.current_weather?.windspeed);
                    Console.WriteLine("WindDirection --> {0}", data?.current_weather?.winddirection);
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }

        public static List<City> LoadJson()
        {
            List<City> cities = null;
            try
            {
                string filePath = @"C:\Users\v.katiyar\source\repos\CustomWeatherClientTool\CustomWeatherClientTool\City.json";

                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    cities = JsonConvert.DeserializeObject<List<City>>(json);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return cities;
        }
    }
}
