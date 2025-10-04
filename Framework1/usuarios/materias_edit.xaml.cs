using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using Framework1.conexion_dba;

namespace Framework1.usuarios
{
    public partial class materias_edit : Window
    {
        public materias_edit()
        {
            InitializeComponent();
            Globales.Sqlite_Conex.Open();
            Mostrar_Datos_Grid();
        }

        public void Mostrar_Datos_Grid()
        {
            try
            {
                string Consulta_Sql = "SELECT * FROM MATERIAS ORDER BY ID";
                SQLiteCommand Cmd_Materias = new SQLiteCommand(Consulta_Sql, Globales.Sqlite_Conex);
                Cmd_Materias.ExecuteNonQuery();

                SQLiteDataAdapter adaptador = new SQLiteDataAdapter(Cmd_Materias);
                DataTable tabla_materias = new DataTable("MATERIAS");
                adaptador.Fill(tabla_materias);

                grid_materias.ItemsSource = tabla_materias.DefaultView;
                adaptador.Update(tabla_materias);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar materias: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Globales.Sqlite_Conex.Close();
        }

        private void grid_materias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid Grid = (DataGrid)sender;
                DataRowView fila = Grid.SelectedItem as DataRowView;

                if (fila != null)
                {
                    Globales.ID_Materia = Convert.ToInt32(fila["ID"].ToString());
                    Globales.Nombre_Materia = fila["NOMBRE"].ToString();
                    Globales.Semestre_Materia = Convert.ToInt32(fila["SEMESTRE"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar materia: " + ex.Message);
            }
        }
    }
}
