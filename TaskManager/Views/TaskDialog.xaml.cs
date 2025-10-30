using System.ComponentModel;
using System.Windows;
using TaskManager.ViewModels;

namespace TaskManager.Views;

public partial class TaskDialog : Window
{
    private TaskViewModel _viewModel;

    public TaskDialog(TaskViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        Title = viewModel.IsEditMode ? "Edit Task" : "Add New Task";

        // ✅ KLUCZOWE - nasłuchuj zmian DialogResult
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // ✅ Gdy ViewModel ustawi DialogResult, zamknij okno
        if (e.PropertyName == nameof(TaskViewModel.DialogResult))
        {
            if (_viewModel.DialogResult.HasValue)
            {
                // ✅ To zamyka okno WPF!
                this.DialogResult = _viewModel.DialogResult.Value;
            }
        }
    }

    private void AddPersonButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.ShowAddPersonForm();
    }

    private void SavePersonButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.AddNewPerson();
    }

    private void CancelPersonButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.CancelAddPerson();
    }
}