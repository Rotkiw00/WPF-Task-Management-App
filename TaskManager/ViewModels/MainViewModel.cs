using System.Collections.ObjectModel;
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
        ClearFiltersCommand = new RelayCommand(async () => await ClearFilters()); // ✨ NOWE

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
            OnPropertyChanged(nameof(HasActiveFilters)); // ✨ Update indicator
        }
    }

    public Status? SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            SetProperty(ref _selectedStatus, value, nameof(SelectedStatus));
            OnPropertyChanged(nameof(HasActiveFilters)); // ✨ Update indicator

            // Only auto-filter if user is not searching
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
            OnPropertyChanged(nameof(HasActiveFilters)); // ✨ Update indicator

            // Only auto-filter if user is not searching
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

    // ✨ NOWE - Visual indicator for active filters
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
    public ICommand ClearFiltersCommand { get; } // ✨ NOWE

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

        // Clear combo filters when searching by text ✨ KEY FEATURE
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

        // Notify UI about changes
        OnPropertyChanged(nameof(SelectedStatus));
        OnPropertyChanged(nameof(SelectedPriority));
        OnPropertyChanged(nameof(SearchText));
        OnPropertyChanged(nameof(HasActiveFilters));

        // Reload all tasks
        await LoadTasks();
        StatusMessage = "Filters cleared, showing all tasks";
    }

    #endregion
}