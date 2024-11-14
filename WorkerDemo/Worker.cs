using System.Net.Http.Json;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly int _fetchInterval;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = new HttpClient();

        // Read configuration values from appsettings.json
        _apiUrl = configuration["ApiSettings:Url"];
        _fetchInterval = int.Parse(configuration["ApiSettings:FetchIntervalSeconds"]);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Fetching data from API at: {time}", DateTimeOffset.Now);

                // Fetch data from the API
                var data = await _httpClient.GetFromJsonAsync<ApiData>(_apiUrl, stoppingToken);

                // Process the data (in this case, just log it)
                _logger.LogInformation("Data fetched: {data}", data);

                // Wait for the specified interval before fetching data again
                await Task.Delay(_fetchInterval * 1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker stopping.");
        await base.StopAsync(stoppingToken);
    }
}

// Define a class to represent the JSON data structure
public class ApiData
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public bool Completed { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, Completed: {Completed}";
    }
}
