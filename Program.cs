using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace CryptoCoinSaver
{
    class Program
    {
        private static String CRYPTOCOMPARE_API_KEY = "4f9000235719c664092cacb8f4ceeec8da270d0227a11f36da3062212255f0f2";
        private static String NOMICS_API_KEY = "e16fd774b68d3d460d63240eef1ba0b8";
        static void Main(string[] args)
        {
            DownloadCryptoCurrencyInfo();
        }

        private static void DownloadCryptoCurrencyInfo()
        {
            var currencyList = new string[] { "BTC", "ETH", "USDT", "XRP", "BСH", "LINK", "BNB", "AUD" };
            var resultDictionary = new Dictionary<string, ExchangeRatesHistory[]>();

            foreach (var currency in currencyList)
            {
                WebRequest request = WebRequest.Create($"https://api.nomics.com/v1/exchange-rates/history?key={NOMICS_API_KEY}&currency={currency}&start=2020-11-01T00:00:00Z&end=2020-11-08T00:00:00Z");

                WebResponse response = request.GetResponse();

                ExchangeRatesHistory[] responseModel;

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var data = reader.ReadToEnd();
                        Console.WriteLine(currency);
                        Console.WriteLine(data);
                        responseModel = JsonSerializer.Deserialize<ExchangeRatesHistory[]>(data); //десереализация из стоки в модель
                    }
                }

                resultDictionary.Add(currency, responseModel);

                response.Close();
            }
        }
    }

    public class ExchangeRatesHistory
    {
        public DateTime Timestamp { get; set; }
        public string Rate { get; set; }
    }

    //public class MyClass
    //{
    //    public double USD { get; set; }
    //    public double EUR { get; set; }
    //}
}
