using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CryptoCoinSaver
{
    class Program
    {
        private static String ALPHA_VANTAGE_API_KEY = "XXGM6QUJN5T5J9W9";
        static void Main(string[] args)
        {
            DownloadCryptoCurrencyInfo();
        }

        private static void DownloadCryptoCurrencyInfo()
        {
            var currencyList = new string[] { "BTC", "ETH", "XRP","BCH", "LTC" };
            var resultDictionary = new Dictionary<string, ApiResponse>();

            foreach (var currency in currencyList)
            {
                Console.WriteLine($"Запрос отправлен: {currency}");
                WebRequest request = WebRequest.Create($"https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_DAILY&symbol={currency}&market=USD&apikey={ALPHA_VANTAGE_API_KEY}");
                WebResponse response = request.GetResponse();

                ApiResponse responseModel;

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var data = reader.ReadToEnd();
                        //Console.WriteLine(currency);
                        //Console.WriteLine(data.Substring(0, 100));
                        //десереализация из стоки в модель
                        responseModel = JsonConvert.DeserializeObject<ApiResponse>(data);
                    }
                }

                resultDictionary.Add(currency, responseModel);

                response.Close();

                Console.WriteLine($"Ответ получен: {currency}");
            }

            foreach (var currency in currencyList)
            {
                Console.WriteLine(currency);
                Console.WriteLine("2020-11-10");
                Console.WriteLine(resultDictionary[currency].mainData["2020-11-10"]);
                Console.WriteLine();
            }
                
        }
    }

    public class ApiResponse
    {
        [JsonProperty("Meta Data")]
        public Dictionary<string, dynamic> metaData { get; set; }

        [JsonProperty("Time Series (Digital Currency Daily)")]
        public Dictionary<string, DigitalCurrencyDaily> mainData { get; set; }
    }

    public class DigitalCurrencyDaily
    {
        [JsonProperty("1b. open (USD)")]
        public double open { get; set; }
        [JsonProperty("2b. high (USD)")]
        public double high { get; set; }
        [JsonProperty("3b. low (USD)")]
        public double low { get; set; }
        [JsonProperty("4b. close (USD)")]
        public double close { get; set; }
        [JsonProperty("5. volume")]
        public double volume { get; set; }

        public override string ToString()
        {
            return $"Open: {open}\nHigh: {high}\nLow: {low}\nClose: {close}\nVolume: {volume}";
        }
    }
}
