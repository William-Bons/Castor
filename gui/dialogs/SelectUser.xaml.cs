using Castor.database;
using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using RefreshEventHandler = Castor.gui.common.RefreshEventHandler;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для SelectUser.xaml
    /// </summary>
    public partial class SelectUser : Window, INotifyPropertyChanged, IDialog, IRefresh
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event RefreshEventHandler RefreshNotify;

        public SelectUser()
        {
            try
            {
                MainWindow.Wait(true);

                // список разрешенных отделений, устанавливается в настройках приложения
                List<long> allowed = new List<long>();
                foreach (var d in Settings.Default.AllowedDepartments.Split(';'))
                {
                    long alldep = 0;
                    if(long.TryParse(d, out alldep))
                        allowed.Add(alldep);
                }

                // получение списка отделений и врачей отделений из учреждения, код учреждения указан в параметре RootDepartmentId
                using (MedisContext medis = new MedisContext())
                {
                    Departments = medis.dep
                        .Where(dep => dep.rootid == Settings.Default.RootDepartmentId) // ref Department Дружносельская ПБ 1482
                        .Where(d => allowed.Contains(d.keyid))
                        //.Include(dep => dep.Docdeps)
                        .ToList();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Departments)));
                }

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
            finally
            {
                MainWindow.Wait();
            }
            
            // Стандартная инициализация диалога
            InitializeComponent();
            DataContext = this;
        }

        public ICollection<dep> Departments { get; set; } = new List<dep>();
        public string DbFilename { get; set; } = Settings.Default.sqliteConnection;
        public dep? SelectedDepartment { get; set;  }


        private void SaveDialogButton(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save SelectedDepartment Name and Doctors into Settings.Default
                Settings.Default.LastSelectedDepId = SelectedDepartment.keyid;
                Settings.Default.LastSelectedDepName = SelectedDepartment.text;
                
                DbFilename = $"{Settings.Default.dbPrefix}\\dep{SelectedDepartment.keyid}.db";
                Settings.Default.sqliteConnection = DbFilename;
                
                //Settings.Default.DepartmentUsers = string.Join(';', [.. SelectedDepartment.Docdeps.Select(d => d.text)]);
                Settings.Default.Save();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDepartment)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DbFilename)));

                // проверка существования файла БД и его создание в случае отсутствия
                using (CastorContext context = new CastorContext())
                {
                    context.Database.EnsureCreated();
                }
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
            //using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            //{
            //    dialog.Description = "Select a folder for your project";
            //    dialog.ShowNewFolderButton = true;
            //    dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        string folderPath = dialog.SelectedPath;
            //        Settings.Default.dbPrefix = folderPath;
            //        Settings.Default.Save();

            //        SelectedDepartment = SelectedDepartment;
            //    }
            //}
        }
    }
}
