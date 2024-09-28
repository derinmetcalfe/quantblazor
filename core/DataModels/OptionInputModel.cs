using System.Globalization;
using QuantBlazor.OptionPricing;

namespace QuantBlazor.DataModels;

public class OptionInputModel
{
    public OptionPricingModelType SelectedModel { get; set; }
    public string Ticker { get; set; } = "AAPL"; // Default value
    public double StrikePrice { get; set; } = 110;
    public double RiskFreeRate { get; set; } = 0.2; // Risk-free rate in %
    public double Sigma { get; set; } = 20; // Volatility in %

    public DateTime ExerciseDate { get; set; } =
        DateTime.ParseExact("2025/09/04", "yyyy/MM/dd", CultureInfo.InvariantCulture);

    public DateTime Period1 { get; set; } = DateTime.Now.AddMonths(-1); // Default to one month ago
    public DateTime Period2 { get; set; } = DateTime.Now; // Default to today
    public int NumOfSimulations { get; set; } = 100;
    public int NumOfPriceSimulations { get; set; } = 100;
    public int NumOfTimeSteps { get; set; } = 15000;
    public string SelectedInterval { get; set; } = "1wk";

    public string[] Intervals { get; set; } =
        { "1m", "2m", "5m", "15m", "30m", "60m", "90m", "1d", "5d", "1wk", "1mo", "3mo" };

    public OptionType OptionType { get; set; } = OptionType.CallOption;

    public bool Visualise { get; set; } = false;
}