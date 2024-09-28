using System.Text.Json;
using System.Text.RegularExpressions;
using QuantBlazor.DataModels;
using Newtonsoft.Json.Linq;

namespace QuantBlazor.Util;

public static class Util
{
    /// <summary>
    /// Extracts the adjusted close prices from a Yahoo Finance API JSON response.
    /// </summary>
    /// <param name="jsonResponse">The JSON response string from Yahoo Finance.</param>
    /// <returns>
    /// An array of doubles representing the adjusted close prices from the provided JSON response.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown if the adjusted close prices are not found in the response.
    /// </exception>
    public static double[] ExtractAdjustedClosePrices(string jsonResponse)
    {
        var json = JObject.Parse(jsonResponse);
        var adjClosePrices = json["chart"]?["result"]?[0]?["indicators"]?["adjclose"]?[0]?["adjclose"]
            ?.ToObject<double[]>();
        
        if (adjClosePrices == null)
            throw new Exception("Adjusted close prices not found in the response.");
        
        return adjClosePrices;
    }

    /// <summary>
    /// Calculates the number of days remaining until the option's maturity or exercise date.
    /// </summary>
    /// <param name="exerciseDate">The exercise or maturity date of the option.</param>
    /// <returns>
    /// An integer representing the number of days from today to the exercise date.
    /// </returns>
    public static int GetDaysToMaturity(DateTime exerciseDate)
    {
        var today = DateTime.Now.Date;
        var timeToMaturity = exerciseDate - today;
        return timeToMaturity.Days;
    }

    /// <summary>
    /// Retrieves the last price from an array of prices.
    /// </summary>
    /// <param name="prices">An array of doubles representing the price data.</param>
    /// <returns>
    /// A double representing the last price in the array. 
    /// Returns 0 if the array is null or empty.
    /// </returns>
    public static double GetLastPrice(double[] prices)
    {
        if (prices != null && prices.Length > 0) return prices[^1];
        return 0;
    }

    /// <summary>
    /// Converts a given DateTime to a Unix timestamp (the number of seconds since January 1, 1970, UTC).
    /// </summary>
    /// <param name="date">The DateTime to convert to a Unix timestamp.</param>
    /// <returns>
    /// A long representing the Unix timestamp corresponding to the given DateTime.
    /// </returns>
    public static long ConvertToUnixTimestamp(DateTime date)
    {
        return ((DateTimeOffset)date).ToUnixTimeSeconds();
    }

    /// <summary>
    /// Formats a given string by adding a space before each uppercase letter that follows a lowercase letter.
    /// This is useful for converting camelCase or PascalCase strings into a more human-readable format.
    /// </summary>
    /// <param name="text">The input string to format.</param>
    /// <returns>
    /// A string with spaces inserted before each uppercase letter that follows a lowercase letter.
    /// </returns>
    public static string FormatModelName(string text)
    {
        return Regex.Replace(
            text,
            "(?<=[a-z])([A-Z])",
            " $1",
            RegexOptions.Compiled);
    }

    /// <summary>
    /// Extracts key stock information from a Yahoo Finance JSON response, such as symbol, price, volume, 
    /// and 52-week high/low data.
    /// </summary>
    /// <param name="jsonData">The JSON response string containing the stock data.</param>
    /// <returns>
    /// A <see cref="HistoricalDataModel"/> object populated with key stock data, including the symbol, 
    /// current price, previous close, 52-week range, daily range, volume, currency, and exchange information.
    /// </returns>
    /// <exception cref="Exception">
    /// Catches and logs any exceptions that occur while extracting data from the JSON response.
    /// </exception>
    public static HistoricalDataModel ExtractKeyResponseData(string jsonData)
    {
        var historicalDataModel = new HistoricalDataModel();
        try
        {
            // Parse the JSON document
            using (var document = JsonDocument.Parse(jsonData))
            {
                var root = document.RootElement;

                // Cache the 'chart' and 'result' properties to avoid redundant GetProperty calls
                var chart = root.GetProperty("chart");
                var result = chart.GetProperty("result")[0];
                var meta = result.GetProperty("meta");

                // Assign key data to variables using cached properties
                historicalDataModel.Symbol = meta.GetProperty("symbol").GetString();
                historicalDataModel.LongName = meta.GetProperty("longName").GetString();
                historicalDataModel.CurrentPrice = meta.GetProperty("regularMarketPrice").GetDecimal();
                historicalDataModel.PreviousClose = meta.GetProperty("chartPreviousClose").GetDecimal();
                historicalDataModel.FiftyTwoWeekHigh = meta.GetProperty("fiftyTwoWeekHigh").GetDecimal();
                historicalDataModel.FiftyTwoWeekLow = meta.GetProperty("fiftyTwoWeekLow").GetDecimal();
                historicalDataModel.DayHigh = meta.GetProperty("regularMarketDayHigh").GetDecimal();
                historicalDataModel.DayLow = meta.GetProperty("regularMarketDayLow").GetDecimal();
                historicalDataModel.Volume = meta.GetProperty("regularMarketVolume").GetInt64();
                historicalDataModel.Currency = meta.GetProperty("currency").GetString();
                historicalDataModel.ExchangeName = meta.GetProperty("exchangeName").GetString();
                historicalDataModel.FullExchangeName = meta.GetProperty("fullExchangeName").GetString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting data from JSON: {ex.Message}");
        }

        return historicalDataModel;
    }

    /// <summary>
    /// Extracts advanced historical data (timestamps and adjusted close prices) from a Yahoo Finance JSON response.
    /// </summary>
    /// <param name="jsonData">The JSON response string containing the historical data.</param>
    /// <returns>
    /// A list of <see cref="HistoricalDataModel"/> objects, where each object contains the date and the adjusted close price for that date.
    /// </returns>
    /// <exception cref="Exception">
    /// Catches and logs any exceptions that occur while parsing the JSON data.
    /// </exception>
    public static List<HistoricalDataModel> ExtractHistoricalDataAdvanced(string jsonData)
    {
        var historicalData = new List<HistoricalDataModel>();

        try
        {
            // Parse the root JSON document
            using (var document = JsonDocument.Parse(jsonData))
            {
                var root = document.RootElement;

                // Cache intermediate properties to avoid redundant GetProperty calls
                var chart = root.GetProperty("chart");
                var result = chart.GetProperty("result")[0];
                var indicators = result.GetProperty("indicators");

                // Access the 'timestamp' and 'adjclose' arrays
                var timestampsElement = result.GetProperty("timestamp");
                var adjCloseElement = indicators.GetProperty("adjclose")[0].GetProperty("adjclose");

                var dataLength = timestampsElement.GetArrayLength();

                // Initialize the list with the known capacity
                historicalData = new List<HistoricalDataModel>(dataLength);

                for (var i = 0; i < dataLength; i++)
                {
                    var timestamp = timestampsElement[i].GetInt64();
                    var adjClosePrice = adjCloseElement[i].GetDecimal();

                    // Convert timestamp to DateTime
                    var date = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

                    historicalData.Add(new HistoricalDataModel
                    {
                        Date = date,
                        Close = Math.Round(adjClosePrice, 2)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }

        return historicalData;
    }
}