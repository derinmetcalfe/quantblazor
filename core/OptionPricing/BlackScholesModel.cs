using MathNet.Numerics.Distributions;

namespace QuantBlazor.OptionPricing;

public class BlackScholesModel : BaseModel
{
    private readonly double d1; // Precomputed d1
    private readonly double d2; // Precomputed d2

    public BlackScholesModel(
        double underlyingSpotPrice,
        double strikePrice,
        int daysToMaturity,
        double riskFreeRate,
        double sigma)
    {
        S = underlyingSpotPrice;
        K = strikePrice;
        T = daysToMaturity / 365.0;
        r = riskFreeRate;
        Sigma = sigma;

        if (S <= 0) throw new ArgumentException("Underlying spot price must be positive.", nameof(underlyingSpotPrice));
        if (K <= 0) throw new ArgumentException("Strike price must be positive.", nameof(strikePrice));
        if (T <= 0) throw new ArgumentException("Time to maturity must be positive.", nameof(daysToMaturity));
        if (Sigma <= 0) throw new ArgumentException("Volatility must be positive.", nameof(sigma));

        // Precompute d1 and d2
        var sigmaSqrtT = Sigma * Math.Sqrt(T);
        d1 = (Math.Log(S / K) + (r + 0.5 * Sigma * Sigma) * T) / sigmaSqrtT;
        d2 = d1 - sigmaSqrtT;
    }

    private double S { get; } // Underlying spot price
    private double K { get; } // Strike price
    private double T { get; } // Time to maturity (in years)
    private double r { get; } // Risk-free rate
    private double Sigma { get; } // Volatility

    /// <summary>
    ///     Calculates the call option price using the Black-Scholes formula.
    /// </summary>
    /// <returns>The calculated call option price.</returns>
    protected override double CalculateCallOptionPrice()
    {
        return S * Normal.CDF(0.0, 1.0, d1) - K * Math.Exp(-r * T) * Normal.CDF(0.0, 1.0, d2);
    }

    /// <summary>
    ///     Calculates the put option price using the Black-Scholes formula.
    /// </summary>
    /// <returns>The calculated put option price.</returns>
    protected override double CalculatePutOptionPrice()
    {
        return K * Math.Exp(-r * T) * Normal.CDF(0.0, 1.0, -d2) - S * Normal.CDF(0.0, 1.0, -d1);
    }
}