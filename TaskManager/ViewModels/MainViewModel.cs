using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TaskManager.Commands;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private ObservableCollection<WorkTask> _tasks;
    private WorkTask? _selectedTask;
    private string _searchText = "";
    private Status? _selectedStatus;
    private Priority? _selectedPriority;
    private string _statusMessage = "";

    public MainViewModel(ITaskService taskService)
    {
        _taskService = taskService;
        _tasks = new ObservableCollection<WorkTask>();

        LoadTasksCommand = new RelayCommand(async () => await LoadTasks());
        AddTaskCommand = new RelayCommand(AddTask);
        EditTaskCommand = new RelayCommand(EditTask, () => SelectedTask != null);
        DeleteTaskCommand = new RelayCommand(async () => await DeleteTask(), () => SelectedTask != null);
        SearchCommand = new RelayCommand(async () => await SearchTasks());
        ClearFiltersCommand = new RelayCommand(async () => await ClearFilters());
        ExportToCsvCommand = new RelayCommand(ExportToCsv, () => Tasks.Any());
        ExportToExcelCommand = new RelayCommand(ExportToExcel, () => Tasks.Any());

        Task.Run(LoadTasks);
    }

    #region Properties

    public ObservableCollection<WorkTask> Tasks
    {
        get => _tasks;
        set => SetProperty(ref _tasks, value, nameof(Tasks));
    }

    public WorkTask? SelectedTask
    {
        get => _selectedTask;
        set => SetProperty(ref _selectedTask, value, nameof(SelectedTask));
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value, nameof(SearchText));
            OnPropertyChanged(nameof(HasActiveFilters));
        }
    }

    public Status? SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            SetProperty(ref _selectedStatus, value, nameof(SelectedStatus));
            OnPropertyChanged(nameof(HasActiveFilters));

            if (string.IsNullOrEmpty(SearchText))
            {
                _ = FilterTasks();
            }
        }
    }

    public Priority? SelectedPriority
    {
        get => _selectedPriority;
        set
        {
            SetProperty(ref _selectedPriority, value, nameof(SelectedPriority));
            OnPropertyChanged(nameof(HasActiveFilters));

            if (string.IsNullOrEmpty(SearchText))
            {
                _ = FilterTasks();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value, nameof(StatusMessage));
    }

    public bool HasActiveFilters =>
        SelectedStatus != null ||
        SelectedPriority != null ||
        !string.IsNullOrEmpty(SearchText);

    // For ComboBoxes
    public Array StatusOptions => Enum.GetValues(typeof(Status));
    public Array PriorityOptions => Enum.GetValues(typeof(Priority));

    #endregion

    #region Commands

    public ICommand LoadTasksCommand { get; }
    public ICommand AddTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand ExportToCsvCommand { get; }
    public ICommand ExportToExcelCommand { get; }

    #endregion

    #region Private Methods

    private async Task LoadTasks()
    {
        try
        {
            var result = await _taskService.GetAllTasksAsync();

            if (result.IsSuccess)
            {
                Tasks.Clear();
                foreach (var task in result.Data!)
                {
                    Tasks.Add(task);
                }
                StatusMessage = result.Message;
            }
            else
            {
                StatusMessage = "Failed to load tasks: " + result.Message;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Error: " + ex.Message;
        }
    }

    private async void AddTask()
    {
        try
        {
            var viewModel = new TaskViewModel(_taskService);
            var dialog = new TaskManager.Views.TaskDialog(viewModel);

            var result = dialog.ShowDialog();

            if (result == true)
            {
                await LoadTasks();
                StatusMessage = "Task added successfully!";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening add dialog: {ex.Message}";
        }
    }

    private async void EditTask()
    {
        if (SelectedTask == null) return;

        try
        {
            var viewModel = new TaskViewModel(_taskService, SelectedTask);
            var dialog = new TaskManager.Views.TaskDialog(viewModel);

            var result = dialog.ShowDialog();

            if (result == true) 
            {
                await LoadTasks(); 
                StatusMessage = "Task updated successfully! ✏️";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening edit dialog: {ex.Message}";
        }
    }

    private async Task DeleteTask()
    {
        if (SelectedTask == null) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{SelectedTask.Title}'?",
            "Delete Task",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            var deleteResult = await _taskService.DeleteTaskAsync(SelectedTask.Id);

            if (deleteResult.IsSuccess)
            {
                Tasks.Remove(SelectedTask);
                StatusMessage = deleteResult.Message;
                SelectedTask = null;
            }
            else
            {
                MessageBox.Show("Failed to delete task: " + deleteResult.Message);
            }
        }
    }

    private async Task SearchTasks()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            // If search is empty, apply current filters
            await FilterTasks();
            return;
        }

        if (SelectedStatus != null || SelectedPriority != null)
        {
            _selectedStatus = null;
            _selectedPriority = null;
            OnPropertyChanged(nameof(SelectedStatus));
            OnPropertyChanged(nameof(SelectedPriority));
            OnPropertyChanged(nameof(HasActiveFilters));
        }

        var result = await _taskService.SearchTasksAsync(SearchText);

        if (result.IsSuccess)
        {
            Tasks.Clear();
            foreach (var task in result.Data!)
            {
                Tasks.Add(task);
            }
            StatusMessage = result.Message;
        }
        else
        {
            StatusMessage = "Search failed: " + result.Message;
        }
    }

    private async Task FilterTasks()
    {
        var result = await _taskService.FilterTasksAsync(SelectedStatus, SelectedPriority);

        if (result.IsSuccess)
        {
            Tasks.Clear();
            foreach (var task in result.Data!)
            {
                Tasks.Add(task);
            }
            StatusMessage = $"Filtered: {result.Data!.Count} tasks found";
        }
    }

    private async Task ClearFilters()
    {
        _selectedStatus = null;
        _selectedPriority = null;
        _searchText = "";

        OnPropertyChanged(nameof(SelectedStatus));
        OnPropertyChanged(nameof(SelectedPriority));
        OnPropertyChanged(nameof(SearchText));
        OnPropertyChanged(nameof(HasActiveFilters));

        // Reload all tasks
        await LoadTasks();
        StatusMessage = "Filters cleared, showing all tasks";
    }

    private void ExportToCsv()
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            DefaultExt = ".csv",
            FileName = $"Tasks_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                var lines = new List<string>();
                
                // Header
                lines.Add("Title,Status,Priority,Assigned To,Due Date,Created Date,Estimated Hours,Description,Tags");
                
                // Data rows
                foreach (var task in Tasks)
                {
                    var assignedTo = task.AssignedTo?.Name ?? "";
                    var dueDate = task.DueDate?.ToString("yyyy-MM-dd") ?? "";
                    var tags = task.Tags != null ? string.Join(";", task.Tags) : "";
                    var description = task.Description?.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", "") ?? "";
                    
                    lines.Add($"\"{task.Title}\",{task.Status},{task.Priority},\"{assignedTo}\",{dueDate},{task.CreatedDateTime:yyyy-MM-dd},{task.EstimatedHours},\"{description}\",\"{tags}\"");
                }
                
                File.WriteAllLines(saveFileDialog.FileName, lines);
                StatusMessage = $"Exported {Tasks.Count} tasks to CSV";
                MessageBox.Show($"Tasks exported successfully to:\n{saveFileDialog.FileName}", 
                    "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = "Export failed";
                MessageBox.Show($"Failed to export: {ex.Message}", "Export Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExportToExcel()
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv",
            DefaultExt = ".xlsx",
            FileName = $"Tasks_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                var lines = new List<string>();
                
                // Header
                lines.Add("Title\tStatus\tPriority\tAssigned To\tDue Date\tCreated Date\tEstimated Hours\tDescription\tTags");
                
                // Data rows
                foreach (var task in Tasks)
                {
                    var assignedTo = task.AssignedTo?.Name ?? "";
                    var dueDate = task.DueDate?.ToString("yyyy-MM-dd") ?? "";
                    var tags = task.Tags != null ? string.Join("; ", task.Tags) : "";
                    var description = task.Description?.Replace("\t", " ").Replace("\n", " ").Replace("\r", "") ?? "";
                    
                    lines.Add($"{task.Title}\t{task.Status}\t{task.Priority}\t{assignedTo}\t{dueDate}\t{task.CreatedDateTime:yyyy-MM-dd}\t{task.EstimatedHours}\t{description}\t{tags}");
                }
                
                File.WriteAllLines(saveFileDialog.FileName, lines);
                StatusMessage = $"Exported {Tasks.Count} tasks to Excel";
                MessageBox.Show($"Tasks exported successfully to:\n{saveFileDialog.FileName}\n\nNote: This is a tab-delimited file. For native Excel format, EPPlus library would be needed.", 
                    "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = "Export failed";
                MessageBox.Show($"Failed to export: {ex.Message}", "Export Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    #endregion
}