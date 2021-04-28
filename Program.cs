using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CryptoCoinSaver
{
    class Program
    {
        private static String METALS_API_KEY = "9mnbcp3pwmybgyjtk9n656bpkh9du3t7mmkmmmvzl6l45w911ly656yiyzio";
        private static String ALPHA_VANTAGE_API_KEY = "XXGM6QUJN5T5J9W9"; //ключ
        static void Main(string[] args)
        {
            var currencyList = new string[] { "BTC"/*, "ETH", "XRP", "XLM", "LTC", "ADA", "BNB"*/ };
            var resultDictionary = new Dictionary<string, ApiResponse>();
            //var goldResultDictionary = new Dictionary<string, MetalsApiResponse>();
            //DownloadGoldInfo(goldResultDictionary);
            //WriteGoldToCSV(goldResultDictionary);
            DownloadCryptoCurrencyInfo(currencyList, resultDictionary);
            //WriteToCSV(currencyList, resultDictionary);
            //WriteToConsole(currencyList, resultDictionary);
            WriteCurrencyToCSV("BTC", resultDictionary["BTC"]);
        }
        private static void DownloadGoldInfo(Dictionary<string, MetalsApiResponse> resultDictionary)
        {
            var endDate = DateTime.Now.AddDays(-1).ToString("yyyy'-'MM'-'dd");
            Console.WriteLine($"Запрос отправлен: GOLD");
            WebRequest request = WebRequest.Create($"https://metals-api.com/api/timeseries?access_key={METALS_API_KEY}&start_date=2018-08-02&end_date={endDate}&base=XAU&symbols=USD");
            WebResponse response = request.GetResponse();

            Console.WriteLine($"Ответ получен: GOLD");

            MetalsApiResponse responseModel;

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var data = reader.ReadToEnd();
                    responseModel = JsonConvert.DeserializeObject<MetalsApiResponse>(data);
                }
            }

            resultDictionary.Add("GOLD", responseModel);

            response.Close();

            Console.WriteLine($"Десериализация завершена: GOLD");

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
        public static void WriteGoldToCSV(Dictionary<string, MetalsApiResponse> resultDictionary)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string writePath = @$"{baseDirectory}\GoldPriceUSD.csv";

            var dateForCycle = DateTime.Now.AddDays(-1);
            var firstDay = new DateTime(2018, 8, 2);
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("Date,GOLD");
                    while (dateForCycle > firstDay)
                    {
                        var formattedDateForCycle = dateForCycle.ToString("yyyy-MM-dd");
                        sw.Write(formattedDateForCycle);


                        sw.Write(",");
                        if (resultDictionary["GOLD"]?.rates == null || !resultDictionary["GOLD"].rates.ContainsKey(formattedDateForCycle))
                        {
                            sw.Write("null");
                        }
                        else
                        {
                            var stringToWrite = resultDictionary["GOLD"].rates[formattedDateForCycle].USD.ToString();
                            sw.Write(stringToWrite.Replace(",", "."));
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
        public static void WriteToCSV(string[] currencyList, Dictionary<string, ApiResponse> resultDictionary)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string writePath = @$"{baseDirectory}\CryptoCoinHigh.csv";

            var dateForCycle = DateTime.Now.AddDays(-1);
            var firstDay = DateTime.Now.AddDays(-999);
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine($"Date,{string.Join(",", currencyList)}");
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
        public static void WriteCurrencyToCSV(string currency, ApiResponse apiResponse)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string writePath = @$"{baseDirectory}\{currency}.csv";

            var lastDay = DateTime.Now.AddDays(-1);
            var firstDay = DateTime.Now.AddDays(-999);
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine($"Date,Open,High,Low,Close,Volume");
                    while (lastDay > firstDay)
                    {
                        var formattedDateForCycle = firstDay.ToString("yyyy-MM-dd");
                        sw.Write(formattedDateForCycle);

                        sw.Write(",");
                        if (apiResponse?.mainData == null || !apiResponse.mainData.ContainsKey(formattedDateForCycle))
                        {
                            sw.Write("null");
                        }
                        else
                        {
                            var stringToWrite = apiResponse.mainData[formattedDateForCycle].open.ToString();
                            sw.Write(stringToWrite.Replace(",", "."));
                            sw.Write(",");
                            stringToWrite = apiResponse.mainData[formattedDateForCycle].high.ToString();
                            sw.Write(stringToWrite.Replace(",", "."));
                            sw.Write(",");
                            stringToWrite = apiResponse.mainData[formattedDateForCycle].low.ToString();
                            sw.Write(stringToWrite.Replace(",", "."));
                            sw.Write(",");
                            stringToWrite = apiResponse.mainData[formattedDateForCycle].close.ToString();
                            sw.Write(stringToWrite.Replace(",", "."));
                            sw.Write(",");
                            stringToWrite = apiResponse.mainData[formattedDateForCycle].volume.ToString();
                            sw.Write(stringToWrite.Replace(",", "."));
                        }
                        sw.WriteLine();
                        firstDay = firstDay.AddDays(1);
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

    public class MetalsApiResponse
    {
        [JsonProperty("rates")]
        public Dictionary<string, MetalsDaily> rates { get; set; }
    }

    public class MetalsDaily
    {
        [JsonProperty("USD")]
        public double USD { get; set; }
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
