using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CryptoCoinSaver
{
    class Program
    {
        private static String ALPHA_VANTAGE_API_KEY = "XXGM6QUJN5T5J9W9"; //ключ
        static void Main(string[] args)
        {
            var currencyList = new string[] { "BTC", "ETH", "XRP", "BCH", "XLM", "LTC", "ADA", "BNB", "LINK"};
            var resultDictionary = new Dictionary<string, ApiResponse>();
            DownloadCryptoCurrencyInfo(currencyList, resultDictionary);
            WriteToCSV(currencyList, resultDictionary);
            //WriteToConsole(currencyList, resultDictionary);
        }

        private static void DownloadCryptoCurrencyInfo(string[] currencyList, Dictionary<string, ApiResponse> resultDictionary)
        {

            foreach (var currency in currencyList)
            {
                Console.WriteLine($"Запрос отправлен: {currency}");
                WebRequest request = WebRequest.Create($"https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_DAILY&symbol={currency}&market=USD&apikey={ALPHA_VANTAGE_API_KEY}");
                WebResponse response = request.GetResponse();

                Console.WriteLine($"Ответ получен: {currency}");

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

                Console.WriteLine($"Десериализация завершена: {currency}");
            }
        }

        public static void WriteToConsole(string[] currencyList, Dictionary<string, ApiResponse> resultDictionary)
        {
            foreach (var currency in currencyList)
            {
                var dateForCycle = new DateTime(2020, 12, 14);
                while (dateForCycle < DateTime.Now.AddDays(-1))
                {
                    var formattedDateForCycle = dateForCycle.ToString("yyyy-MM-dd");
                    Console.WriteLine(currency);
                    Console.WriteLine(dateForCycle);
                    if (resultDictionary[currency]?.mainData == null || !resultDictionary[currency].mainData.ContainsKey(formattedDateForCycle))
                    {
                        Console.Write("null");
                    }
                    else
                    {
                        Console.WriteLine(resultDictionary[currency].mainData[formattedDateForCycle]);
                    }
                    dateForCycle = dateForCycle.AddDays(1);
                    Console.WriteLine();
                }
            }
        }
        public static void WriteToCSV(string[] currencyList, Dictionary<string, ApiResponse> resultDictionary)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string writePath = @$"{baseDirectory}\CryptoCoinHigh_BCH_XLM_LTC.csv";

            var dateForCycle = DateTime.Now.AddDays(-1);
            var firstDay = DateTime.Now.AddDays(-999);
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("Date,BCH,ETH,XRP,BCH,XLM,LTC,ADA,BNB,LINK");
                    while (dateForCycle > firstDay)
                    {
                        var formattedDateForCycle = dateForCycle.ToString("yyyy-MM-dd");
                        sw.Write(formattedDateForCycle);

                        foreach (var currency in currencyList)
                        {
                            sw.Write(",");
                            if (resultDictionary[currency]?.mainData == null || !resultDictionary[currency].mainData.ContainsKey(formattedDateForCycle))
                            {
                                sw.Write("null");
                            }
                            else
                            {
                                var stringToWrite = resultDictionary[currency].mainData[formattedDateForCycle].high.ToString();
                                sw.Write(stringToWrite.Replace(",", "."));
                            }
                        }
                        sw.WriteLine();
                        dateForCycle = dateForCycle.AddDays(-1);
                    }
                }
                Console.WriteLine("done");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }

    public class ApiResponse
    {
        [JsonProperty("Meta Data")]
        public Dictionary<string, dynamic> metaData { get; set; }

        [JsonProperty("Time Series (Digital Currency Daily)")]
        public Dictionary<string, DigitalCurrencyDaily> mainData { get; set; } //{ date: {open, high,low, close, volume}}
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
