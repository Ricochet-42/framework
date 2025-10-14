using Framework1.conexion_dba;
using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Framework1.usuarios
{
    public partial class alumnos_edit : Window
    {
        public alumnos_edit()
        {
            InitializeComponent();
            Globales.Sqlite_Conex.Open();
            Mostrar_Datos_Grid();
        }

        public void Mostrar_Datos_Grid()
        {
            try
            {
                string Consulta_Sql = "SELECT * FROM ALUMNOS ORDER BY EXPEDIENTE";
                SQLiteCommand Cmd_Alumnos = new SQLiteCommand(Consulta_Sql, Globales.Sqlite_Conex);
                Cmd_Alumnos.ExecuteNonQuery();

                SQLiteDataAdapter adaptador = new SQLiteDataAdapter(Cmd_Alumnos);
                DataTable tabla_alumnos = new DataTable("ALUMNOS");
                adaptador.Fill(tabla_alumnos);

                grid_alumnos.ItemsSource = tabla_alumnos.DefaultView;
                adaptador.Update(tabla_alumnos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar alumnos: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Globales.Sqlite_Conex.Close();
        }

        private void Row_RightClick_Select(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = true;
                grid_alumnos.SelectedItem = row.Item;
                grid_alumnos.CurrentItem = row.Item;
                grid_alumnos.Focus();
            }
        }
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            bool hay = grid_alumnos.SelectedItem != null;
            if (FindName("ctxEditar") is MenuItem m1) m1.IsEnabled = hay;
            if (FindName("ctxEliminar") is MenuItem m2) m2.IsEnabled = hay;
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fila = grid_alumnos.SelectedItem as DataRowView;
                if (fila == null)
                {
                    MessageBox.Show("Seleccione un alumno para editar.", "Aviso",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int expediente = Convert.ToInt32(fila["EXPEDIENTE"]);
                string nombre = fila["NOMBRE"]?.ToString() ?? "";
                int semestre = Convert.ToInt32(fila["SEMESTRE"]);
                string correo = fila["CORREO"]?.ToString() ?? "";
                int edad = Convert.ToInt32(fila["EDAD"]);
                string genero = fila["GENERO"]?.ToString() ?? "";

                var dlg = new EditAlumnoDialog(Globales.Sqlite_Conex,
                                               expediente, nombre, semestre, correo, edad, genero)
                { Owner = this };

                if (dlg.ShowDialog() == true)
                    Mostrar_Datos_Grid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir editor: " + ex.Message, "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void grid_alumnos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid Grid = (DataGrid)sender;
                DataRowView fila = Grid.SelectedItem as DataRowView;

                if (fila != null)
                {
                    Globales.Expediente_Alumno = Convert.ToInt32(fila["EXPEDIENTE"].ToString());
                    Globales.Nombre_Alumno = fila["NOMBRE"].ToString();
                    Globales.Semestre_Alumno = Convert.ToInt32(fila["SEMESTRE"].ToString());
                    Globales.Correo_Alumno = fila["CORREO"].ToString();
                    Globales.Edad_Alumno = Convert.ToInt32(fila["EDAD"].ToString());
                    Globales.Genero_Alumno = fila["GENERO"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar alumno: " + ex.Message);
            }
        }
    }
}
