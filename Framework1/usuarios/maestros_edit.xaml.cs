using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using Framework1.conexion_dba;

namespace Framework1.usuarios
{
    public partial class maestros_edit : Window
    {
        public maestros_edit()
        {
            InitializeComponent();
            Globales.Sqlite_Conex.Open();
            Mostrar_Datos_Grid();
        }

        public void Mostrar_Datos_Grid()
        {
            try
            {
                string Consulta_Sql = "SELECT * FROM MAESTROS ORDER BY ID";
                SQLiteCommand Cmd_Maestros = new SQLiteCommand(Consulta_Sql, Globales.Sqlite_Conex);
                Cmd_Maestros.ExecuteNonQuery();

                SQLiteDataAdapter adaptador = new SQLiteDataAdapter(Cmd_Maestros);
                DataTable tabla_maestros = new DataTable("MAESTROS");
                adaptador.Fill(tabla_maestros);

                grid_maestros.ItemsSource = tabla_maestros.DefaultView;
                adaptador.Update(tabla_maestros);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar maestros: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Globales.Sqlite_Conex.Close();
        }

        private void grid_maestros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid Grid = (DataGrid)sender;
                DataRowView fila = Grid.SelectedItem as DataRowView;

                if (fila != null)
                {
                    Globales.ID_Maestro = Convert.ToInt32(fila["ID"].ToString());
                    Globales.Nombre_Maestro = fila["NOMBRE"].ToString();
                    Globales.Correo_Maestro = fila["CORREO"].ToString();
                    Globales.Facultad_Maestro = fila["FACULTAD"].ToString();
                    Globales.Tipo_Maestro = fila["TIPO"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar maestro: " + ex.Message);
            }
        }
    }
}
