using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TaskManager.Commands;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.ViewModels;

public class TaskViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private WorkTask _task;
    private List<Person> _people = new();
    private Person? _selectedPerson;
    private string _tagsText = "";

    public TaskViewModel(ITaskService taskService, WorkTask? task = null)
    {
        _taskService = taskService;
        _task = task ?? new WorkTask { Status = Status.Draft, Priority = Priority.Medium };

        if (task != null)
        {
            _selectedPerson = task.AssignedTo;
            _tagsText = string.Join(", ", task.Tags);
        }

        SaveCommand = new AsyncRelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);

        _ = LoadPeople();
    }

    #region Properties

    public WorkTask Task
    {
        get => _task;
        set => SetProperty(ref _task, value, nameof(Task));
    }

    public string Title
    {
        get => _task.Title;
        set
        {
            _task.Title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public string Description
    {
        get => _task.Description;
        set
        {
            _task.Description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public Status Status
    {
        get => _task.Status;
        set
        {
            _task.Status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public Priority Priority
    {
        get => _task.Priority;
        set
        {
            _task.Priority = value;
            OnPropertyChanged(nameof(Priority));
        }
    }

    public DateTime? DueDate
    {
        get => _task.DueDate;
        set
        {
            _task.DueDate = value;
            OnPropertyChanged(nameof(DueDate));
        }
    }

    public int? EstimatedHours
    {
        get => _task.EstimatedHours;
        set
        {
            _task.EstimatedHours = value;
            OnPropertyChanged(nameof(EstimatedHours));
        }
    }

    public Person? SelectedPerson
    {
        get => _selectedPerson;
        set
        {
            SetProperty(ref _selectedPerson, value, nameof(SelectedPerson));
            _task.AssignedTo = value;
        }
    }

    public string TagsText
    {
        get => _tagsText;
        set
        {
            SetProperty(ref _tagsText, value, nameof(TagsText));
            _task.Tags = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(t => t.Trim())
                           .Where(t => !string.IsNullOrEmpty(t))
                           .ToList();
        }
    }

    public List<Person> People
    {
        get => _people;
        set => SetProperty(ref _people, value, nameof(People));
    }

    public Array StatusOptions => Enum.GetValues(typeof(Status));
    public Array PriorityOptions => Enum.GetValues(typeof(Priority));

    private bool? _dialogResult;
    public bool? DialogResult
    {
        get => _dialogResult;
        set => SetProperty(ref _dialogResult, value, nameof(DialogResult));
    }
    
    public bool IsEditMode => _task.Id != Guid.Empty;

    // Add Person functionality
    private bool _isAddingNewPerson;
    public bool IsAddingNewPerson
    {
        get => _isAddingNewPerson;
        set => SetProperty(ref _isAddingNewPerson, value, nameof(IsAddingNewPerson));
    }

    private string _newPersonName = "";
    public string NewPersonName
    {
        get => _newPersonName;
        set => SetProperty(ref _newPersonName, value, nameof(NewPersonName));
    }

    #endregion

    #region Commands

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    #endregion

    #region Methods

    private async Task LoadPeople()
    {
        var result = await _taskService.GetAllPeopleAsync();
        if (result.IsSuccess)
        {
            People = result.Data!;
        }
    }

    public void ShowAddPersonForm()
    {
        IsAddingNewPerson = true;
        NewPersonName = "";
    }

    public void AddNewPerson()
    {
        if (string.IsNullOrWhiteSpace(NewPersonName))
        {
            MessageBox.Show("Please enter a person name.", "Validation", 
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Create new person
        var newPerson = new Person 
        { 
            Id = Guid.NewGuid(), 
            Name = NewPersonName.Trim() 
        };
        
        // Add to list
        _people.Add(newPerson);
        OnPropertyChanged(nameof(People));
        
        // Select the new person
        SelectedPerson = newPerson;
        
        // Hide form
        IsAddingNewPerson = false;
        NewPersonName = "";
    }

    public void CancelAddPerson()
    {
        IsAddingNewPerson = false;
        NewPersonName = "";
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(Title);
    }

    private void Cancel()
    {
        DialogResult = false;
    }

    private async Task Save()
    {
        try
        {
            if (IsEditMode)
            {
                var result = await _taskService.UpdateTaskAsync(_task);
                if (result.IsSuccess)
                {
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show($"Failed to update task:\n{string.Join("\n", result.Errors)}",
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                var result = await _taskService.CreateTaskAsync(_task);
                if (result.IsSuccess)
                {
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show($"Failed to create task:\n{string.Join("\n", result.Errors)}",
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    #endregion
}