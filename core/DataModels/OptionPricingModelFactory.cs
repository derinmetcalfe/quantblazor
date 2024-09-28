using QuantBlazor.OptionPricing;

namespace QuantBlazor.DataModels;

public static class OptionPricingModelFactory
{
    public static IOptionPricingModel CreateModel(OptionPricingModelType modelType,
        OptionPricingParametersModel parameters)
    {
        switch (modelType)
        {
            case OptionPricingModelType.BlackScholes:
                return new BlackScholesModel(
                    parameters.SpotPrice,
                    parameters.StrikePrice,
                    parameters.DaysToMaturity,
                    parameters.RiskFreeRate,
                    parameters.SigmaDecimal
                );
            case OptionPricingModelType.MonteCarlo:
                return new MonteCarloModel(
                    parameters.SpotPrice,
                    parameters.StrikePrice,
                    parameters.DaysToMaturity,
                    parameters.RiskFreeRate,
                    parameters.SigmaDecimal,
                    parameters.NumOfSimulations
                );
            case OptionPricingModelType.BinomialTree:
                return new BinomialTreeModel(
                    parameters.SpotPrice,
                    parameters.StrikePrice,
                    parameters.DaysToMaturity,
                    parameters.RiskFreeRate,
                    parameters.SigmaDecimal,
                    parameters.NumOfTimeSteps
                );
            default:
                throw new ArgumentException("Invalid model type selected.", nameof(modelType));
        }
    }
}