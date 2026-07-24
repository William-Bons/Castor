using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using RefreshEventHandler = Castor.gui.common.RefreshEventHandler;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для SelectUser.xaml
    /// </summary>
    public partial class SelectUser : Window, INotifyPropertyChanged, IDialog, IRefresh
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event RefreshEventHandler? RefreshNotify;

        public SelectUser()
        {
            try
            {
                LoadAllowedDepartmentsList();

                // если отделение уже ранее выбрано то выбор его в списке отдлений
                long depid = Settings.Default.LastSelectedDepId;
                if (depid > 0)
                {
                    SelectedDepartment = Departments?.Where(dep => dep.keyid == depid)?.First();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDepartment)));
                }

            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }

            // Стандартная инициализация диалога
            InitializeComponent();
            DataContext = this;
        }
        public ICollection<dep> Departments { get; set; } = new List<dep>();
        public string DbFilename { get; set; } = Settings.Default.sqliteConnection;
        public dep? SelectedDepartment { get; set; }

        private void LoadAllowedDepartmentsList()
        {
            // список разрешенных отделений, устанавливается в настройках приложения
            List<long> allowed = new List<long>();
            foreach (var d in Settings.Default.AllowedDepartments.Split(';'))
            {
                long alldep = 0;
                if (long.TryParse(d, out alldep))
                    allowed.Add(alldep);
            }

            // получение списка отделений и врачей отделений из учреждения, код учреждения указан в параметре RootDepartmentId
            using (MedisContext medis = new MedisContext())
            {
                Departments = medis.dep
                    .Where(dep => dep.rootid == Settings.Default.RootDepartmentId) // ref Department Дружносельская ПБ 1482
                    .Where(d => allowed.Contains(d.keyid))                         // фильтр допустимых отделений (из Settings)
                                                                                   //.Include(dep => dep.Docdeps)
                    .OrderBy(dep => dep.keyid)
                    .ToList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Departments)));
            }
        }


        private void SaveDialogButton(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save SelectedDepartment Name and Doctors into Settings.Default
                Settings.Default.LastSelectedDepId = SelectedDepartment.keyid;
                Settings.Default.LastSelectedDepName = SelectedDepartment.text;
                Settings.Default.sqliteConnection = DbFilename;
                //Settings.Default.DepartmentUsers = string.Join(';', [.. SelectedDepartment.Docdeps.Select(d => d.text)]);
                Settings.Default.Save();

                Close();
                RefreshNotify?.Invoke("Castor.gui.movebook.Theband", "Castor.gui.movebook.Thebook");
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }

        public void Refresh()
        {
        }

        private void SelectDbDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog();
            dialog.Title = "Select a folder";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dialog.ShowDialog() == true)
            {
                string folderPath = dialog.FolderName;
                Settings.Default.dbPrefix = folderPath;
                Settings.Default.Save();
                DbFilename = $"{Settings.Default.dbPrefix}\\dep{SelectedDepartment?.keyid}.db";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DbFilename)));
            }
        }

        private void F1KeyPressed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            using MedisContext context = new MedisContext();
            List<dep> deps = context.dep
                .Where(d => d.rootid == Settings.Default.RootDepartmentId)
                .Include(d => d.Root)
                .ToList();
            new SelectObjectFromEnumerable("Отделения", deps, PlacementMode.MousePoint, "keyid", "text")
                .Selected += (o) =>
                {
                    if (o is dep _selDepartment)
                    {
                        List<string> allowedSettings = Settings.Default.AllowedDepartments.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                        allowedSettings.Add(_selDepartment.keyid.ToString());
                        Settings.Default.AllowedDepartments = string.Join(";", allowedSettings);
                        Settings.Default.Save();
                        LoadAllowedDepartmentsList();
                    }
                };
        }

        private void DepartmentSelectedInCombo(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DbFilename = $"{Settings.Default.dbPrefix}\\dep{SelectedDepartment?.keyid}.db";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DbFilename)));
        }
    }
}
