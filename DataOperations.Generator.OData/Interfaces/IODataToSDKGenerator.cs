namespace Generator
{
    public interface IODataToSDKGenerator
    {   
        Task GenerateAsync(string input, string location, bool generateSamples);
    }
}