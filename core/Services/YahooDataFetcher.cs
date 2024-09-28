namespace QuantBlazor.Services;

public class YahooDataFetcher
{
    private string resultMessage;

    /// <summary>
    ///     Fetches historical stock data for a given ticker from Yahoo Finance within a specified date range.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., AAPL for Apple Inc.).</param>
    /// <param name="startDate">The start date for the historical data.</param>
    /// <param name="endDate">The end date for the historical data.</param>
    /// <param name="interval">The interval for the data (e.g., "1d" for daily, "1h" for hourly).</param>
    /// <param name="includePrePost">A boolean indicating whether to include pre-market and post-market data.</param>
    /// <returns>
    ///     The JSON response from Yahoo Finance with the requested ticker data.
    /// </returns>
    public async Task<string> FetchTickerData(string ticker, DateTime startDate, DateTime endDate, string interval,
        bool includePrePost)
    {
        var starter = new DateTime(2023, 1, 1);
        var ender = new DateTime(2023, 12, 1);
        var period1 = Util.Util.ConvertToUnixTimestamp(starter);
        var period2 = Util.Util.ConvertToUnixTimestamp(ender);

        var url = $"https://query1.finance.yahoo.com/v8/finance/chart/{ticker}?" +
                  $"period1={period1}&period2={period2}&" +
                  $"interval={interval}&" +
                  $"includePrePost={includePrePost.ToString().ToLower()}";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                resultMessage = jsonResponse;

                return resultMessage;
            }
            catch (Exception ex)
            {
                resultMessage = $"An error occurred: {ex.Message}";
                return resultMessage;
            }
        }
    }
}