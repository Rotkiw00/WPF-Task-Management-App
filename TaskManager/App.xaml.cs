using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Data;

namespace TaskManager;
public partial class App : Application
{
    private readonly ServiceProvider? _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TaskManager",
            "tasks.db"
        );

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        services.AddDbContext<TaskDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<ITaskRepository, TaskRepository>();

        services.AddScoped<ITaskService, TaskService>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using (var scope = _serviceProvider!.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

            context.Database.EnsureCreated();

            DataSeeder.SeedData(context);
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
