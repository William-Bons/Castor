using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Castor.gui.force
{
    public class ForcedViewModel : INotifyPropertyChanged
    {
        private Forced? _selectedItem;
        private Movebook _movebook;

        public ObservableCollection<ForcedNode> TreeItems { get; } = new();

        public Forced? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEdit));
                LoadFormData();
            }
        }

        public bool CanEdit => SelectedItem != null;

        // --- Поля формы с поддержкой INotifyPropertyChanged ---
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

        private long _patientId;
        public long PatientId
        {
            get => _patientId;
            set { _patientId = value; OnPropertyChanged(); }
        }

        private long _visitId;
        public long VisitId
        {
            get => _visitId;
            set { _visitId = value; OnPropertyChanged(); }
        }

        private int? _type;
        public int? Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        private string? _courtName;
        public string? CourtName
        {
            get => _courtName;
            set { _courtName = value; OnPropertyChanged(); }
        }

        private long? _moveBookId;
        public long? MoveBookId
        {
            get => _moveBookId;
            set { _moveBookId = value; OnPropertyChanged(); }
        }
        // -----------------------------------------------------

        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CreateCommand { get; }

        public ForcedViewModel(Movebook movebook)
        {
            _movebook = movebook;
            SaveCommand = new RelayCommandAsync(Save, () => CanEdit);
            RefreshCommand = new RelayCommandAsync(LoadTree);
            CreateCommand = new RelayCommand(Create);
            Task.Run(LoadTree);
        }

        private void Create()  
        {
            if(TreeItems.Count == 0)
            {
                // создание ROOT постановления
                SelectedItem = new Forced()
                {
                    Movebookid = _movebook.Id,
                    Patientid = _movebook.Patientid ?? 0,
                    Visitid = _movebook.Visitid ?? 0,
                    RootId = 0 // RootId = 0 у первичного постановления
                };
                var node = new ForcedNode(SelectedItem);
                BuildTree(SelectedItem, node);
                TreeItems.Add(node);
            }
            else
            {
                // создание вторичного постановления
            }

            
        }

        public async Task LoadTree()
        {
            using CastorContext _context = new CastorContext();
            TreeItems.Clear();
            var roots = _context.Forced
                .Where(f => f.Movebookid == _movebook.Id) // корневые
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
                BuildTree(child, childNode); // рекурсия, если есть вложенность глубже
            }
        }

        private void LoadFormData()
        {
            if (SelectedItem == null)
            {
                ClearForm();
                return;
            }

            Number = SelectedItem.Number;
            Start = SelectedItem.Start;
            End = SelectedItem.End;
            PatientId = SelectedItem.Patientid;
            VisitId = SelectedItem.Visitid;
            Type = SelectedItem.Type;
            CourtName = SelectedItem.Courtname;
            MoveBookId = SelectedItem.Movebookid;
        }

        private void ClearForm()
        {
            Number = null;
            Start = DateOnly.FromDateTime(DateTime.Today);
            End = null;
            PatientId = 0;
            VisitId = 0;
            Type = null;
            CourtName = null;
            MoveBookId = null;
        }

        private async Task Save()
        {
            if (SelectedItem == null || !CanEdit) return;

            SelectedItem.Number = Number;
            SelectedItem.Start = Start;
            SelectedItem.End = End;
            SelectedItem.Patientid = PatientId;
            SelectedItem.Visitid = VisitId;
            SelectedItem.Type = Type;
            SelectedItem.Courtname = CourtName;
            SelectedItem.Movebookid = MoveBookId;

            using CastorContext _context = new CastorContext();
            _context.Update(SelectedItem);
            await _context.SaveChangesAsync();
            // опционально: обновить дерево или показать уведомление
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ForcedNode
    {
        public Forced Data { get; }
        public ObservableCollection<ForcedNode> Children { get; } = new();

        public ForcedNode(Forced data) => Data = data;

        public override string ToString() =>
            Data.Number ?? $"Постановление #{Data.Id} ({Data.Start})";
    }

}
