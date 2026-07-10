using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
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
        private string? _patientInfo;
        public string? PatientInfo
        {
            get => _patientInfo;
            set { _patientInfo = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ForcedNode> TreeItems { get; } = new();

        public Forced? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(IsNew));
                LoadFormData();
            }
        }

        public bool CanEdit => SelectedItem != null;

        // Поля формы — имена строго соответствуют полям класса Forced
        private string? _number;
        public string? Number
        {
            get => _number;
            set { _number = value; OnPropertyChanged(); }
        }

        private DateOnly _start = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly Start
        {
            get => _start;
            set { _start = value; OnPropertyChanged(); }
        }

        private DateOnly? _end;
        public DateOnly? End
        {
            get => _end;
            set { _end = value; OnPropertyChanged(); }
        }

        private int? _type;
        public int? Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        // Список значений для ComboBox (100–199)
        public List<int> TypeOptions { get; } = new()
        {
            100, // амб
            101, // общ
            102, // спец
            103, // стин
            104, // снято
            199  // в переходе
        };

        private string? _section;
        public string? Section
        {
            get => _section;
            set { _section = value; OnPropertyChanged(); }
        }

        private string? _courtName;
        public string? Courtname
        {
            get => _courtName;
            set { _courtName = value; OnPropertyChanged(); }
        }

        private int[]? _month;
        public int[]? Month
        {
            get => _month;
            set { _month = value; OnPropertyChanged(); }
        }

        public Visibility IsNew => (SelectedItem?.Id ?? 0) == 0 ? Visibility.Visible : Visibility.Collapsed;

        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }

        public ForcedViewModel(Movebook mb)
        {
            _movebook = mb;
            SaveCommand = new RelayCommandAsync(async () => await SaveAsync(), () => CanEdit);
            RefreshCommand = new RelayCommand(LoadTree);
            AddCommand = new RelayCommand(AddItem);
            LoadTree();

            // Если список пуст - добавление новой записи
            if(TreeItems.Count==0)
            {
                AddItem();
            }
        }

        private void AddItem()
        {
            if(TreeItems.Count==0 && _movebook!=null)
            {
                // добавление первичного 
                SelectedItem = new Forced()
                {
                    Movebookid = _movebook.Id,
                    Patientid = _movebook.Patientid ?? 0,
                    Visitid = _movebook.Visitid ?? 0,
                    Start = _movebook.Datein ?? DateOnly.FromDateTime(DateTime.Now)
                };
            }
            else if(_movebook!=null)
            {
                // добавление последующих
                ForcedNode? root = TreeItems
                    .Where(t => t.Data.RootId == null)
                    .ToList().First();
                SelectedItem = new Forced()
                {
                    Movebookid = _movebook.Id,
                    Patientid = _movebook.Patientid ?? 0,
                    Visitid = _movebook.Visitid ?? 0,
                    RootId = root?.Data.Id ?? 0,
                    Start = root?.Data.End ?? DateOnly.FromDateTime(DateTime.Now)
                };
            }
            LoadFormData();
        }

        public void LoadTree()
        {
            using CastorContext _context = new CastorContext();
            TreeItems.Clear();
            var roots = _context.Forced
                .Where(f => f.Movebookid==_movebook.Id && f.RootId==null)
                .Include(f => f.AllForces)
                .ToList();

            foreach (var root in roots)
            {
                var node = new ForcedNode(root);
                BuildTree(root, node);
                TreeItems.Add(node);
            }
        }

        private void BuildTree(Forced parent, ForcedNode parentNode)
        {
            if (parent.AllForces == null) return;
            foreach (var child in parent.AllForces)
            {
                var childNode = new ForcedNode(child);
                parentNode.Children.Add(childNode);
                BuildTree(child, childNode);
            }
        }

        private void LoadFormData()
        {
            if (SelectedItem == null)
            {
                ClearForm();
                PatientInfo = null;
                return;
            }

            var item = SelectedItem;

            // Формируем строку с данными пациента (подставь свои поля из Patient/Visit, если они есть в контексте)
            // Здесь пока просто заготовка на основе ID из Forced
            PatientInfo = $"Пациент: #{_movebook?.Fio ?? string.Empty} | Визит: #{item.Visitid}";

            Number = item.Number;
            Start = item.Start;
            End = item.End;
            Type = item.Type;
            Section = item.Section;
            Courtname = item.Courtname;
            Month = item.Month;
        }

        private void ClearForm()
        {
            Number = null;
            Start = DateOnly.FromDateTime(DateTime.Today);
            End = null;
            Type = null;
            Section = null;
            Courtname = null;
            Month = null;
        }

        private async Task SaveAsync()
        {
            if (SelectedItem == null) return;

            var item = SelectedItem;
            item.Number = Number;
            item.Start = Start;
            item.End = End;
            item.Type = Type;
            item.Section = Section;
            item.Courtname = Courtname;

            using CastorContext _context = new CastorContext();
            _context.Update(SelectedItem);
            await _context.SaveChangesAsync();
            LoadTree(); // обновить дерево после сохранения
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ForcedNode
    {
        public Forced Data { get; }
        public ObservableCollection<ForcedNode> Children { get; } = new();

        public string DisplayText => BuildDisplayText(Data);

        public ForcedNode(Forced forced)
        {
            Data = forced;
        }

        private static string BuildDisplayText(Forced item)
        {
            // Формируем читаемую строку для узла дерева
            var parts = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(item.Number))
                parts.Append($"№ {item.Number}");

            if (parts.Length > 0)
                parts.Append(" | ");

            parts.Append($"Тип: {item.Type}");

            if (item.Start != default)
                parts.Append($" | {item.Start.ToString("dd.MM.yyyy")}");

            return parts.ToString();
        }
    }
}
