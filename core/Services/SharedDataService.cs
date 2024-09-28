using QuantBlazor.DataModels;
using QuantBlazor.OptionPricing;

namespace QuantBlazor.Services;

public class SharedDataService
{
    public string SharedResponse { get; set; }
    public double SharedCalculatedPrice { get; set; }
    public MonteCarloModel SharedMonteCarloModel { get; set; }

    public OptionType SharedOptionType { get; set; }

    public List<HistoricalDataModel> SharedPriceData { get; set; }
}