using System.Text;
using DateDumperApp;
using DateDumperApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

Console.WriteLine("Press Enter to start");
Console.ReadLine();

var serviceProvider = new ServiceCollection()
    .AddTransient<ICsvService, CsvService>()
    .BuildServiceProvider();

var csvService = serviceProvider.GetService<ICsvService>();

var values = ReadConfigValues();
var filePaths = Directory.GetFiles(values.DirPath);

var userList = new List<User>();
foreach (var path in filePaths)
{
    using StreamReader reader = new(path);
    var json = await reader.ReadToEndAsync();
    var users = JsonConvert.DeserializeObject<List<User>>(json);

    var finalUserList = userList.Union(ExtractUser(users)).ToList();
    DumpDataToCsv(finalUserList, values.CsvPath, csvService);

    Console.WriteLine("Done!");
    Console.ReadLine();
}

static List<User> ExtractUser(List<User> users)
{
    var userList = new List<User>();
    foreach (var user in users)
    {
        Console.WriteLine($"User {user} added to list");
        userList.Add(user);
    }

    return userList;
}

//Not recommended to pass the service in the method but doing it because of the time constraint
static void DumpDataToCsv(List<User> users, string csvPath, ICsvService csvService)
{
    var sb = new StringBuilder();
    foreach (var user in users)
    {
        var id = user.Id;
        var name = user.Name;
        var createdOn = user.CreatedOn;
        var createdBy = user.CreatedBy;

        var newLine = $"{id},{name},{createdBy},{createdBy}";
        sb.Append(newLine);
        Console.WriteLine($"User {user} will be added to the csv file");
    }

    csvService.Add(csvPath, sb.ToString());
}

static Values ReadConfigValues()
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false).Build();

    return config.GetSection("Values").Get<Values>();
}