namespace TvShowScrubber.Constants
{
    public static class SettingConstants
    {
        public static string PreloadCast { get; } = "PreloadCast";

        public static string HttpClientBaseAddress { get; } = "HttpClientBaseAddress";

        public static string HttpClientTimeOutInSeconds { get; } = "HttpClientTimeOutInSeconds";

        public static string ConcurrentThresholdShowsOnly { get; } = "ConcurrentThresholdShowsOnly";

        public static string ConcurrentRequestsThresholdWithCast { get; } = "ConcurrentRequestsThresholdWithCast";
    }
}
