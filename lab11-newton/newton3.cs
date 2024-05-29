using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        int N = 10;
        int K = 5;

        long result = await CalculateBinomialCoefficientAsync(N, K);
        Console.WriteLine($"Symbol Newtona ({N}, {K}) = {result}");
    }

    static async Task<long> CalculateBinomialCoefficientAsync(int N, int K)
    {
        long numerator = await Task.Run(() => CalculateNumerator(N, K));
        long denominator = await Task.Run(() => CalculateDenominator(K));

        return numerator / denominator;
    }

    static long CalculateNumerator(int N, int K)
    {
        long result = 1;
        for (int i = 0; i < K; i++)
        {
            result *= N - i;
        }
        return result;
    }

    static long CalculateDenominator(int K)
    {
        long result = 1;
        for (int i = 1; i <= K; i++)
        {
            result *= i;
        }
        return result;
    }
}
