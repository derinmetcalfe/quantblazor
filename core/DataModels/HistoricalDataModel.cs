namespace QuantBlazor.DataModels;

public class HistoricalDataModel
{
    public DateTime Date { get; set; }
    public decimal Close { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal PreviousClose { get; set; }
    public decimal FiftyTwoWeekHigh { get; set; }
    public decimal FiftyTwoWeekLow { get; set; }
    public decimal DayHigh { get; set; }
    public decimal DayLow { get; set; }
    public long Volume { get; set; }
    public string Currency { get; set; }
    public string ExchangeName { get; set; }
    public string FullExchangeName { get; set; }
    public string LongName { get; set; }
    public string Symbol { get; set; }
}