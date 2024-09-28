using QuantBlazor.OptionPricing;

namespace QuantBlazor.DataModels;

public class OptionPricingParametersModel
{
    public double SpotPrice { get; set; }
    public double StrikePrice { get; set; }
    public int DaysToMaturity { get; set; }
    public double RiskFreeRate { get; set; }
    public double SigmaDecimal { get; set; }
    public int NumOfSimulations { get; set; }
    public int NumOfTimeSteps { get; set; }
    public OptionType OptionType { get; set; }
}