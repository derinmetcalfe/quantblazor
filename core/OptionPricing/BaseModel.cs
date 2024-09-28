namespace QuantBlazor.OptionPricing;

public enum OptionType
{
    CallOption,
    PutOption
}

public enum OptionPricingModelType
{
    BlackScholes,
    MonteCarlo,
    BinomialTree
}

public interface IOptionPricingModel
{
    /// <summary>
    ///     Calculates the option price for the specified option type.
    /// </summary>
    /// <param name="optionType">The type of the option (Call or Put).</param>
    /// <returns>The calculated option price.</returns>
    double CalculateOptionPrice(OptionType optionType);
}

public abstract class BaseModel : IOptionPricingModel
{
    /// <summary>
    ///     Calculates the option price for the specified option type.
    /// </summary>
    /// <param name="optionType">The type of the option (Call or Put).</param>
    /// <returns>The calculated option price.</returns>
    public double CalculateOptionPrice(OptionType optionType)
    {
        switch (optionType)
        {
            case OptionType.CallOption:
                return CalculateCallOptionPrice();
            case OptionType.PutOption:
                return CalculatePutOptionPrice();
            default:
                return -1; // or throw an exception
        }
    }

    /// <summary>
    ///     Calculates the option price for a call option.
    ///     Must be implemented by derived classes.
    /// </summary>
    /// <returns>The calculated call option price.</returns>
    protected abstract double CalculateCallOptionPrice();

    /// <summary>
    ///     Calculates the option price for a put option.
    ///     Must be implemented by derived classes.
    /// </summary>
    /// <returns>The calculated put option price.</returns>
    protected abstract double CalculatePutOptionPrice();
}