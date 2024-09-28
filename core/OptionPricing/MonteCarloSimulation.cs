namespace QuantBlazor.OptionPricing;

public class MonteCarloModel : BaseModel
{
    public MonteCarloModel(
        double underlyingSpotPrice,
        double strikePrice,
        int daysToMaturity,
        double riskFreeRate,
        double sigma,
        int numberOfSimulations)
    {
        S0 = underlyingSpotPrice;
        K = strikePrice;
        T = daysToMaturity / 365.0;
        r = riskFreeRate;
        Sigma = sigma;

        N = numberOfSimulations;
        NumOfSteps = daysToMaturity;
        Dt = T / NumOfSteps;

        Drift = (r - 0.5 * Sigma * Sigma) * Dt;
        VolatilityTerm = Sigma * Math.Sqrt(Dt);

        SimulationResultsS = new double[NumOfSteps, N];
    }

    public double S0 { get; } // Initial underlying spot price
    public double K { get; } // Strike price
    public double T { get; } // Time to maturity (in years)
    public double r { get; } // Risk-free rate
    public double Sigma { get; } // Volatility

    public int N { get; } // Number of simulations
    public int NumOfSteps { get; } // Number of time steps
    public double Dt { get; }
    public double Drift { get; }
    public double VolatilityTerm { get; }

    public double[,] SimulationResultsS { get; } // Results matrix

    /// <summary>
    ///     Simulates stock prices using a Geometric Brownian Motion model across multiple simulations in parallel.
    /// </summary>
    public void SimulatePrices()
    {
        // Initialise the first row of the simulation with the initial price
        for (var i = 0; i < N; i++) SimulationResultsS[0, i] = S0;

        Parallel.For(0, N, i =>
        {
            // Each thread gets its own Random instance
            var localRandom = new Random(Guid.NewGuid().GetHashCode());

            var localSimulation = new double[NumOfSteps];
            localSimulation[0] = S0;

            for (var t = 1; t < NumOfSteps; t++)
            {
                // Generate a standard normal random number
                var Z = NextGaussian(localRandom);
                // Update the price using drift and volatility
                localSimulation[t] = localSimulation[t - 1] * Math.Exp(Drift + VolatilityTerm * Z);
            }

            for (var t = 0; t < NumOfSteps; t++) SimulationResultsS[t, i] = localSimulation[t];
        });
    }

    /// <summary>
    ///     Calculates the price of a European call option using Monte Carlo simulation.
    /// </summary>
    /// <returns>
    ///     The discounted expected payoff of the call option, which is the estimated price of the option.
    /// </returns>
    protected override double CalculateCallOptionPrice()
    {
        SimulatePrices(); // Ensure the simulation is run before calculating the option price

        var payoffSum = 0.0;

        // Compute the payoff for a call option at maturity (NumOfSteps - 1)
        for (var i = 0; i < N; i++)
        {
            var payoff = Math.Max(SimulationResultsS[NumOfSteps - 1, i] - K, 0.0);
            payoffSum += payoff;
        }

        // Average the payoffs and discount to present value
        return Math.Exp(-r * T) * (payoffSum / N);
    }

    /// <summary>
    ///     Calculates the price of a European put option using Monte Carlo simulation.
    /// </summary>
    /// <returns>
    ///     The discounted expected payoff of the put option, which is the estimated price of the option.
    /// </returns>
    protected override double CalculatePutOptionPrice()
    {
        SimulatePrices();

        var payoffSum = 0.0;

        for (var i = 0; i < N; i++)
        {
            var payoff = Math.Max(K - SimulationResultsS[NumOfSteps - 1, i], 0.0);
            payoffSum += payoff;
        }

        return Math.Exp(-r * T) * (payoffSum / N);
    }

    /// <summary>
    ///     Generates a random number following a standard normal (Gaussian) distribution
    ///     using the Box-Muller transform.
    /// </summary>
    /// <param name="rand">An instance of the Random class used to generate uniform random numbers.</param>
    /// <returns>
    ///     A double representing a random value from a standard normal distribution (mean 0, variance 1).
    /// </returns>
    private double NextGaussian(Random rand)
    {
        var u1 = 1.0 - rand.NextDouble(); // Uniform(0,1] random doubles
        var u2 = 1.0 - rand.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}