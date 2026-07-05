using Castor.database.tables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Castor.gui.commities;

public class CommityViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private string _movebookIdInput = string.Empty;
    private string _visitIdInput = string.Empty;
    private string _patientIdInput = string.Empty;
    private int? _selectedType;
    private DateTime? _startDate;
    private DateTime? _closedDate;

    private readonly Dictionary<string, List<string>> _errors = new();

    // Строковые свойства для удобной валидации числового ввода
    public string MovebookIdInput
    {
        get => _movebookIdInput;
        set { _movebookIdInput = value; ValidateId(value); OnPropertyChanged(); }
    }

    public string VisitIdInput
    {
        get => _visitIdInput;
        set { _visitIdInput = value; ValidateId(value); OnPropertyChanged(); }
    }

    public string PatientIdInput
    {
        get => _patientIdInput;
        set { _patientIdInput = value; ValidateId(value); OnPropertyChanged(); }
    }

    public int? SelectedType
    {
        get => _selectedType;
        set { _selectedType = value; ValidateType(value); OnPropertyChanged(); }
    }

    public DateTime? StartDate
    {
        get => _startDate;
        set { _startDate = value; ValidateDates(); OnPropertyChanged(); }
    }

    public DateTime? ClosedDate
    {
        get => _closedDate;
        set { _closedDate = value; ValidateDates(); OnPropertyChanged(); }
    }

    public Dictionary<int?, string> TypeOptions { get; } = new()
    {
        { 100, "100 - ЭЛН (15 дн.)" },
        { 101, "101 - НГ 1 мес (30 дн.)" },
        { 102, "102 - НГ 6 мес (183 дн.)" },
        { 199, "199 - Общий ВК" }
    };

    public RelayCommand SaveCommand { get; }

    public CommityViewModel()
    {
        // Передаем CanExecute предикат, проверяющий отсутствие ошибок
        SaveCommand = new RelayCommand(SaveData, () => !HasErrors && IsFormFilled());
        
        // Первичная валидация пустых полей при запуске
        ValidateId(MovebookIdInput, nameof(MovebookIdInput));
        ValidateId(VisitIdInput, nameof(VisitIdInput));
        ValidateId(PatientIdInput, nameof(PatientIdInput));
        ValidateType(SelectedType);
    }

    #region Логика Валидации

    private void ValidateId(string value, [CallerMemberName] string propertyName = "")
    {
        ClearErrors(propertyName);
        if (string.IsNullOrWhiteSpace(value))
        {
            AddError(propertyName, "Поле обязательно");
        }
        else if (!long.TryParse(value, out long result) || result <= 0)
        {
            AddError(propertyName, "Введите число > 0");
        }
        SaveCommand.RaiseCanExecuteChanged();
    }

    private void ValidateType(int? value)
    {
        ClearErrors(nameof(SelectedType));
        if (!value.HasValue)
        {
            AddError(nameof(SelectedType), "Выберите вид ВК");
        }
        SaveCommand.RaiseCanExecuteChanged();
    }

    private void ValidateDates()
    {
        ClearErrors(nameof(StartDate));
        ClearErrors(nameof(ClosedDate));

        if (StartDate.HasValue && ClosedDate.HasValue && ClosedDate < StartDate)
        {
            AddError(nameof(ClosedDate), "Не может быть раньше начала");
        }
        SaveCommand.RaiseCanExecuteChanged();
    }

    private bool IsFormFilled()
    {
        return !string.IsNullOrWhiteSpace(MovebookIdInput) &&
               !string.IsNullOrWhiteSpace(VisitIdInput) &&
               !string.IsNullOrWhiteSpace(PatientIdInput) &&
               SelectedType.HasValue;
    }

    #endregion

    #region Реализация INotifyDataErrorInfo

    public bool HasErrors => _errors.Any();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            return Enumerable.Empty<string>();
        return _errors[propertyName];
    }

    private void AddError(string propertyName, string error)
    {
        if (!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();

        if (!_errors[propertyName].Contains(error))
        {
            _errors[propertyName].Add(error);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    private void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
        {
            _errors.Remove(propertyName);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    #endregion

    private void SaveData()
    {
        var commity = new Commity
        {
            MovebookId = long.Parse(MovebookIdInput),
            Visitid = long.Parse(VisitIdInput),
            Patientid = long.Parse(PatientIdInput),
            Type = SelectedType,
            Start = StartDate.HasValue ? DateOnly.FromDateTime(StartDate.Value) : null,
            Closed = ClosedDate.HasValue ? DateOnly.FromDateTime(ClosedDate.Value) : null
        };

        string info = $"Объект Commity успешно валидирован и создан!\nID Пациента: {commity.Patientid}";
        if (commity.Start.HasValue)
        {
            info += $"\nСледующая итерация ВК: {commity.CalculateNextIteration(commity.Start.Value)}";
        }

        MessageBox.Show(info, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

#region Модифицированный RelayCommand для поддержки CanExecute
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
    public void Execute(object? parameter) => _execute();
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;
}
#endregion
