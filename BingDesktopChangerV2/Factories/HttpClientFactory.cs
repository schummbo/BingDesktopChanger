namespace BingDesktopChangerV2.Factories
{
    public static class HttpClientFactory
    {
        public static HttpClient CreateHttpClient()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            
            return new HttpClient(clientHandler);
        }
    }
}
