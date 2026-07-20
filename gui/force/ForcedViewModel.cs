using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Castor.gui.force
{
    public class ForcedViewModel : INotifyPropertyChanged
    {
        private Forced? _selectedItem;
        private Movebook? _movebook;

        // Данные пациента (только для отображения вверху формы)
        public string? PatientInfo => $"Пациент: #{_movebook?.Fio ?? string.Empty} | Визит: #{SelectedItem?.Visitid ?? 0}";
        // Список документов (плоский список вместо дерева)
        public ObservableCollection<Forced> ForcedList { get; private set; } = new();
        public Forced? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(IsNew));
            }
        }
        public bool CanEdit => SelectedItem != null;
        public Visibility IsNew => (SelectedItem?.Id ?? 0) == 0 ? Visibility.Visible : Visibility.Collapsed;
        public ICommand? RefreshCommand { get; }
        public ICommand? AddCommand { get; }
        public ICommand? SetBaseCommand { get; }
        public ICommand? DeleteCommand { get; }
        public ICommand? SaveCommand { get; }

        public ForcedViewModel(Movebook mb)
        {
            if (mb == null) return;

            _movebook = mb;
            RefreshCommand = new RelayCommand(LoadTree);
            AddCommand = new RelayCommand(AddItem);
            SetBaseCommand = new RelayCommand(SetBaseFlag);
            DeleteCommand = new RelayCommand(DeleteItem);
            SaveCommand = new RelayCommand(Save);

            LoadTree();
        }

        private void DeleteItem()
        {
            if (SelectedItem == null) return;

            if (MessageBox.Show("Удалить постановление?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                // Если запись существует в БД — удалить
                if (SelectedItem.Id > 0)
                {
                    using CastorContext context = new CastorContext();
                    context.Forced.Remove(SelectedItem);
                    context.SaveChanges();
                }

                // Удаляем из локальных коллекций
                if (_movebook?.Forceds != null)
                {
                    _movebook.Forceds = _movebook.Forceds.Where(f => f != SelectedItem).ToList();
                }

                if (ForcedList.Contains(SelectedItem))
                    ForcedList.Remove(SelectedItem);

                // Установим следующий выбранный элемент
                SelectedItem = ForcedList.FirstOrDefault();
                OnPropertyChanged(nameof(ForcedList));
                OnPropertyChanged(nameof(SelectedItem));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void Save()
        {
            if (ForcedList.Count > 0)
            {
                using CastorContext context = new CastorContext();
                foreach (var item in ForcedList)
                {
                    if (item.Id > 0) context.Update(item);
                    else context.Add(item);
                }
                context.SaveChanges();
            }
        }

        private void SetBaseFlag()
        {
            int calcMonth = SelectedItem?.Start.Month ?? 1;
            ReportSettingsWindow rsw = new ReportSettingsWindow();
            rsw.SelectedRange = calcMonth;
            if (rsw.ShowDialog()==true)
            {
                if (ForcedList.Count > 0 && SelectedItem != null)
                {
                    // Если нужно установить отметку месяца для всех документов — применяем к каждому
                    foreach (var f in ForcedList)
                    {
                        f.MonthFlag = rsw.SelectedRange;
                    }
                    LoadTree();
                    OnPropertyChanged(nameof(SelectedItem));
                    OnPropertyChanged(nameof(ForcedList));
                }
            }
        }

        private void AddItem()
        {
            if(_movebook!=null)
            {
                // добавление нового постановления в список
                // Если есть предыдущие постановления — копируем часть полей из последнего
                var previous = _movebook.Forceds?.OrderByDescending(f => f.Start).FirstOrDefault();

                SelectedItem = new Forced()
                {
                    Movebookid = _movebook.Id,
                    Patientid = _movebook.Patientid ?? 0,
                    Visitid = _movebook.Visitid ?? 0,
                    MonthFlag = 1
                };

                if (previous != null)
                {
                    // Корневой Id: если у предыдущего есть RootId — используем его, иначе используем Id предыдущего
                    SelectedItem.RootId = previous.RootId ?? previous.Id;
                    // Начало нового постановления — конец предыдущего если указан
                    SelectedItem.Start = previous.End ?? DateOnly.FromDateTime(DateTime.Now);
                    SelectedItem.Courtname = previous.Courtname;
                    SelectedItem.Section = previous.Section;
                    SelectedItem.Type = previous.Type;
                }
                else
                {
                    SelectedItem.Start = DateOnly.FromDateTime(DateTime.Now);
                }

                // добавляем во временный список Movebook чтобы LoadTree мог его увидеть
                if (_movebook.Forceds == null) _movebook.Forceds = new List<Forced>();
                _movebook.Forceds.Add(SelectedItem);

                // Обновляем представление и выбор
                LoadTree();
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        /// <summary>
        /// Загружает Forceds из Movebook или добавляет первого нового
        /// </summary>
        public void LoadTree()
        {
            ForcedList.Clear();
            if (_movebook != null)
            {
                // Загружаем все постановления, относящиеся к текущему Movebook, упорядочиваем по дате начала
                var list = _movebook.Forceds?.OrderBy(f => f.Start).ToList() ?? new List<Forced>();

                // Если нет ни одного — создаём стартовое запись
                if (list.Count == 0)
                {
                    list.Add(new Forced()
                    {
                        Movebookid = _movebook.Id,
                        Patientid = _movebook.Patientid ?? 0,
                        Visitid = _movebook.Visitid ?? 0,
                        Start = _movebook.Datein ?? DateOnly.FromDateTime(DateTime.Now),
                        MonthFlag = 1
                    });
                }

                foreach (var f in list)
                    ForcedList.Add(f);
            }
            // Восстановим/установим выбранный элемент: если ранее был выбран элемент с Id>0 - найдём его по Id,
            // иначе выберем только что созданный (Id == 0) или первый в списке.
            long? prevId = _selectedItem?.Id;
            if (prevId != null && prevId > 0)
            {
                SelectedItem = ForcedList.FirstOrDefault(f => f.Id == prevId) ?? ForcedList.FirstOrDefault();
            }
            else
            {
                SelectedItem = ForcedList.FirstOrDefault(f => f.Id == 0) ?? ForcedList.FirstOrDefault();
            }

            OnPropertyChanged(nameof(PatientInfo));
            OnPropertyChanged(nameof(ForcedList));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
