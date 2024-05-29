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
        Task<long> numeratorTask = Task.Run(() => CalculateNumerator(N, K));
        Task<long> denominatorTask = Task.Run(() => CalculateDenominator(K));

        await Task.WhenAll(numeratorTask, denominatorTask);

        return numeratorTask.Result / denominatorTask.Result;
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
