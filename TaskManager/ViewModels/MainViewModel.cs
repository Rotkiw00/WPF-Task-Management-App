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

        _ = LoadTasks();
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
        set => SetProperty(ref _searchText, value, nameof(SearchText));
    }

    public Status? SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            SetProperty(ref _selectedStatus, value, nameof(SelectedStatus));
            _ = FilterTasks(); // Auto-filter when status changes
        }
    }

    public Priority? SelectedPriority
    {
        get => _selectedPriority;
        set
        {
            SetProperty(ref _selectedPriority, value, nameof(SelectedPriority));
            _ = FilterTasks(); // Auto-filter when priority changes
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value, nameof(StatusMessage));
    }

    // For ComboBoxes
    public static Array StatusOptions => Enum.GetValues(typeof(Status));
    public static Array PriorityOptions => Enum.GetValues(typeof(Priority));

    #endregion

    #region Commands

    public ICommand LoadTasksCommand { get; }
    public ICommand AddTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand SearchCommand { get; }

    #endregion

    #region Useful Methods

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
                // TODO: Proper error handling
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Error: " + ex.Message;
            // TODO: Add logging
        }
    }

    private void AddTask()
    {
        // TODO: Open AddTaskDialog/Window
        MessageBox.Show("Add Task - TODO in Phase 6");
    }

    private void EditTask()
    {
        if (SelectedTask == null) return;

        // TODO: Open EditTaskDialog/Window with SelectedTask
        MessageBox.Show($"Edit Task: {SelectedTask.Title} - TODO in Phase 6");
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
            await LoadTasks();
            return;
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
        }
    }

    #endregion
}