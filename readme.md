## Introduction

This application is designed to assist investors, traders, and financial analysts in pricing European options using
three fundamental models. Users are able to fine-tune and configure these models using a variety of input parameters. The application accommodates both **call** and **put** option calculations.

## Features

- **Multiple Pricing Models**: Choose from Black-Scholes, Binomial Tree, or Monte Carlo Simulation to price options
- **Real-Time Data Retrieval**: User inputs are used to build the API request string that pulls data directly from the relevant endpoint, minimalising overhead
- **Interactive Visualisations**:
    - Historical adjusted closing price of the selected stock
    - Simulated price paths from the Monte Carlo Simulation
- **User-Friendly Interface**: Intuitive input fields and sliders for setting parameters
- **Optimised Performance**: Leveraging computational optimisations and libraries for rapid calculations

## Models Implemented

### Black-Scholes Model

A mathematical model for pricing European options, accounting for factors like volatility, interest rate, and time to
maturity.

### Binomial Tree Model

A discrete-time model for option pricing that constructs a binomial tree of possible underlying asset prices.

### Monte Carlo Simulation

A statistical method that uses random sampling to simulate possible price paths of the underlying asset to estimate the
option price.

## Optimisations

The primary motivation for converting the Python-based QuantST application to a C# and .NET solution was to achieve optimal model performance, and enhance the interactiveness of visualisations and charting.  

These optimisations include:

1. **Backward Induction and Memory Swap (BinomialTreeModel)**
   Using backward induction and swapping arrays minimises memory usage. Only the current and previous layers are kept in memory, reducing memory requirements to two arrays, enhancing performance during option pricing calculations.

2. **Parallelism in Monte Carlo Simulation (MonteCarloModel)**
   Parallelised the Monte Carlo simulations allows for concurrent execution, improving performance on multi-core processors. Each simulation runs in parallel threads, speeding up the generation of stock price paths for large numbers of simulations.

3. **Efficient JSON Parsing and Property Caching (Util)**  
   Caching JSON properties like `chart` and `result` minimises the number of calls to `GetProperty()`, improving performance when extracting data from large JSON responses.

4. **Memory Efficiency in Monte Carlo Model (MonteCarloModel)**  
   Reducing the scope of large arrays by using local variables within threads avoids memory contention. Each thread gets its own copy of the array, improving memory efficiency and minimising synchronisation costs.

5. **Box-Muller Transform for Random Gaussian Generation (MonteCarloModel)**  
   Using the Box-Muller transform to generate Gaussian-distributed random numbers is an efficient method compared to more complex alternatives, enhancing the performance of the Monte Carlo simulations.

<br>

<hr>

**Disclaimer:** This application is for educational and informational purposes only and should not be considered financial advice. Always consult with a qualified financial professional before making investment decisions.