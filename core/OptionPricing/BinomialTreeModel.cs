namespace QuantBlazor.OptionPricing;

public class BinomialTreeModel : BaseModel
{
    public BinomialTreeModel(
        double underlyingSpotPrice,
        double strikePrice,
        int daysToMaturity,
        double riskFreeRate,
        double sigma,
        int numberOfTimeSteps)
    {
        S = underlyingSpotPrice;
        K = strikePrice;
        T = daysToMaturity / 365.0;
        r = riskFreeRate;
        Sigma = sigma;
        NumberOfTimeSteps = numberOfTimeSteps;
    }

    // Properties
    private double S { get; } // Underlying spot price
    private double K { get; } // Strike price
    private double T { get; } // Time to maturity in years
    private double r { get; } // Risk-free rate
    private double Sigma { get; } // Volatility (sigma)
    private int NumberOfTimeSteps { get; } // Number of time steps

    /// <summary>
    ///     Calculates the call option price.
    /// </summary>
    /// <returns>The calculated call option price.</returns>
    protected override double CalculateCallOptionPrice()
    {
        return CalculateOptionPriceForType(OptionType.CallOption);
    }

    /// <summary>
    ///     Calculates the put option price.
    /// </summary>
    /// <returns>The calculated put option price.</returns>
    protected override double CalculatePutOptionPrice()
    {
        return CalculateOptionPriceForType(OptionType.PutOption);
    }

    /// <summary>
    ///     Common method to calculate the option price for either call or put.
    /// </summary>
    /// <param name="optionType">The type of the option (Call or Put).</param>
    /// <returns>The calculated option price.</returns>
    private double CalculateOptionPriceForType(OptionType optionType)
    {
        var dT = T / NumberOfTimeSteps; // Delta t
        var u = Math.Exp(Sigma * Math.Sqrt(dT)); // Up factor
        var d = 1.0 / u; // Down factor
        var a = Math.Exp(r * dT); // Risk-free compounded return
        var p = (a - d) / (u - d); // Risk-neutral up probability
        var q = 1.0 - p; // Risk-neutral down probability

        // Initialise arrays for option prices at each node
        var currentLayer = new double[NumberOfTimeSteps + 1];
        var nextLayer = new double[NumberOfTimeSteps + 1];

        // Calculate option prices at maturity
        for (var j = 0; j <= NumberOfTimeSteps; j++)
        {
            var S_T = S * Math.Pow(u, j) * Math.Pow(d, NumberOfTimeSteps - j);
            currentLayer[j] = optionType == OptionType.CallOption
                ? Math.Max(S_T - K, 0.0) // Call option payoff
                : Math.Max(K - S_T, 0.0); // Put option payoff
        }

        // Backward induction for option price
        for (var i = NumberOfTimeSteps - 1; i >= 0; i--)
        {
            for (var j = 0; j <= i; j++)
                nextLayer[j] = Math.Exp(-r * dT) * (p * currentLayer[j + 1] + q * currentLayer[j]);

            // Swap layers for next iteration
            (currentLayer, nextLayer) = (nextLayer, currentLayer);
        }

        // Option price at the root node
        return currentLayer[0];
    }
}