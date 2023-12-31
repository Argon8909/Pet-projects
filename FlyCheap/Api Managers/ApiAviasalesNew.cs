﻿using System.Text;
using FlyCheap.Db_Context;
using FlyCheap.Models;
using FlyCheap.Models.JsonModels;
using Newtonsoft.Json;
using FlyCheap.Utility_Components;
using Microsoft.EntityFrameworkCore;

namespace FlyCheap.Api_Managers;
/*
public class ApiAviaSalesNew

{
    
    private string apiToken = "bbaadb1a591224a0562c138a0a040e4e";

    // Параметры запроса
    string currency = "RUB"; // Валюта цен (по умолчанию - RUB)

    //string origin = "AER"; // Код IATA города или аэропорта отправления
    //string destination = "SVO"; // Код IATA города или аэропорта назначения
    //string departureDate = "2023-09"; // Дата отправления (YYYY-MM или YYYY-MM-DD)
    //string returnDate = "2023-10"; // Дата возврата (не указывайте для билетов в один конец)
    string directFlight = "false"; // Билеты без пересадок (true или false, по умолчанию: false)
    string market = "ru"; // Рынок источника данных (по умолчанию ru)
    int limit = 30; // Количество записей на странице (по умолчанию 30, макс. 1000)
    int page = 1; // Номер страницы (для пропуска результатов)
    string sorting = "price"; // Сортировка цен (по цене или по популярности маршрута, по умолчанию: по цене)

    string
        unique = "false"; // Возвращает только уникальные маршруты (если указан только origin, true или false, по умолчанию: false)

    public string Test()
    {
        HumanReadableConverter humanReadableConverter = new HumanReadableConverter();
        StringBuilder sb = new StringBuilder();

        var airports =
            humanReadableConverter.GetHumanReadableAirways(FlightSearchRequestCreating(new DateTime(2023, 12, 20),
                "Москва", "Tokyo"));

        foreach (var flightData in airports.data)
        {
            sb.Append("origin_airport: " + flightData.origin_airport + "\n");
            sb.Append("destination_airport: " + flightData.destination_airport + "\n");
            sb.Append("departure_at: " + flightData.departure_at + "\n");
            sb.Append("airline: " + flightData.airline + "\n");
            sb.Append("----------------------------------------------" + "\n");
        }

        return sb.ToString();
    }

    public AirwaysJson FlightSearchRequestCreating1(DateTime departureDate, string departureLocation,
        string destinationLocation)
    {
        var flightData = new ObjectForRequestFlight( {
            departureDate = Convert.ToString($"{departureDate.Year}-{departureDate.Month:D2}-{departureDate.Day:D2}")
        };
        
        var departure = FindCities(departureLocation);
        var origin = FindCities(departureLocation);
        
        if (departure == null && origin == null)
        {
            var departureAirPort = FindAnAirport(departureLocation);
            var destinationAirPort = FindAnAirport(destinationLocation);
            flightData.destination = destinationAirPort.Code;
            flightData.origin = departureAirPort.Code;
            content =  HttpRequest(flightData);
        }
        
        try
        {
            string content = HttpRequest(httpRequest);
            if (!string.IsNullOrEmpty(content))
            {
                AirwaysJson airwaysResult = JsonConvert.DeserializeObject<AirwaysJson>(content);
                if (airwaysResult.success)
                {
                    airwaysJson.data.AddRange(airwaysResult.data);
                    airwaysJson.success = true;
                    airwaysJson.currency = airwaysResult.currency;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Произошла ошибка при парсинге JSON: " + e.Message);
        }
        

        return default;
    }

    // FlightSearchRequestCreating(new DateTime(2023, 12,20), "Москва", "Tokyo"); Пример вызова
    public AirwaysJson FlightSearchRequestCreating(DateTime departureDate, string departureCity, string destinationCity)
    {
        var departureAirPorts = FindAnAirports(departureCity);
        var destinationAirPorts = FindAnAirports(destinationCity);

        var flightData = new ObjectForRequestFlight()
        {
            departureAirPorts = departureAirPorts,
            destinationAirPorts = destinationAirPorts,
            returnDate = null,
            departureDate = $"{departureDate.Year}-{departureDate.Month:D2}-{departureDate.Day:D2}",
        };

        //Console.WriteLine("departureDate " + departureDate);
        //Console.WriteL--ine("Дата ===> " + $"{departureDate.Year}-{departureDate.Month: D2}-{departureDate.Day: D2}");

        return RequestFlight(flightData);
    }

    private AirwaysJson RequestFlight(ObjectForRequestFlight flightData)
    {
        AirwaysJson airwaysJson = new AirwaysJson
        {
            currency = "",
            success = false,
            data = new List<FlightData>()
        };

        var httpRequest = new ObjectForHttpRequest();

        foreach (var airportStart in flightData.departureAirPorts)
        {
            httpRequest.origin = airportStart.Code;
            foreach (var airportFinal in flightData.destinationAirPorts)
            {
                httpRequest.destination = airportFinal.Code;
                httpRequest.departureDate = flightData.departureDate;
                httpRequest.returnDate = flightData.returnDate;
                Thread.Sleep(100);

                try
                {
                    string content = HttpRequest(httpRequest);
                    if (!string.IsNullOrEmpty(content))
                    {
                        AirwaysJson airwaysResult = JsonConvert.DeserializeObject<AirwaysJson>(content);
                        if (airwaysResult.success)
                        {
                            airwaysJson.data.AddRange(airwaysResult.data);
                            airwaysJson.success = true;
                            airwaysJson.currency = airwaysResult.currency;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Произошла ошибка при парсинге JSON: " + e.Message);
                }
            }
        }

        return airwaysJson;
    }

    private string? HttpRequest(ObjectForHttpRequest httpRequest)
    {
        string returnDateConstructor =
            string.IsNullOrEmpty(httpRequest.returnDate) ? "" : $"return_at={httpRequest.returnDate}&";

        string url = $"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?" +
                     $"origin={httpRequest.origin}&destination={httpRequest.destination}&departure_at={httpRequest.departureDate}" +
                     $"&{returnDateConstructor}unique={unique}&sorting={sorting}&direct={directFlight}" +
                     $"&cy={currency}&limit={limit}&page={page}&one_way=true&token={apiToken}";

        //Console.WriteLine(url);
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = client.GetAsync(url).Result;
                string content = "";

                if (response.IsSuccessStatusCode)
                {
                    content = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine("content ===> " + content);
                    return content;
                }
                else
                {
                    Console.WriteLine("Ошибка запроса");
                    content = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine("content ===> " + content);
                    //throw new InvalidOperationException("Ошибка запроса!");
                    return content;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                return null;
            }
        }

        return null;
    }

    private Cities? FindCities(string cityOrAirport = "", string iataType = "airport")
    {
        using (AirDbContext dbContext = new())
        {
            var city = dbContext.Cityes
                .AsNoTracking()
                .FirstOrDefault(x => x
                        .name.Contains(cityOrAirport.Trim()) || x
                        .name_translations.Contains(cityOrAirport.Trim())
                );

            if (city != null)
            {
                return city;
            }

            Console.WriteLine("Поиск города не дал результатов!");
            return default;
        }
    }

    private Airport? FindAnAirport(string cityOrAirport = "", string iataType = "airport")
    {
        using (AirDbContext dbContext = new())
        {
            var airport = dbContext.Airports
                .AsNoTracking()
                .FirstOrDefault(x =>
                    (x.name.Contains(cityOrAirport.Trim()) || x.NameTranslationsEn.Contains(cityOrAirport.Trim())) &&
                    x.IataType == iataType &&
                    x.Flightable == true);

            if (airport != null)
            {
                return airport;
            }

            Console.WriteLine("Поиск аэропорта не дал результата!");
            return default;
        }
    }

   
}
*/

/*
 var airportsInCity = dbContext.Airports
                .AsNoTracking()
                .Where(x => x
                    .name.Contains(cityOrAirport.Trim()) || x
                    .NameTranslationsEn.Contains(city.Trim()) || x
                    .Code.Contains();

*/
