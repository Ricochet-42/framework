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
                using (SQLiteCommand Cmd_Maestros = new SQLiteCommand(Consulta_Sql, Globales.Sqlite_Conex))
                using (SQLiteDataAdapter adaptador = new SQLiteDataAdapter(Cmd_Maestros))
                {
                    DataTable tabla_maestros = new DataTable("MAESTROS");
                    adaptador.Fill(tabla_maestros);
                    grid_maestros.ItemsSource = tabla_maestros.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar maestros: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            Mostrar_Datos_Grid();
        }

        //private void grid_usuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        DataGrid Grid = (DataGrid)sender;
        //        DataRowView fila = Grid.SelectedItem as DataRowView;

        //        if (fila != null)
        //        {
        //            Globales.ID_Maestro = Convert.ToInt32(fila["ID"].ToString());
        //            Globales.Nombre_Maestro = fila["NOMBRE"].ToString();
        //            Globales.Correo_Maestro = fila["CORREO"].ToString();
        //            Globales.Facultad_Maestro = fila["Facultad"].ToString();
        //            Globales.Tipo_Maestro = fila["TIPO"].ToString();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error 404 " + ex.Message, "Error 404", MessageBoxButton.OK);
        //    }
        //}

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_maestros.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione un usuario para editar.", "Aviso",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Asegúrate de que Globales.* ya estén cargados por SelectionChanged
                var dlg = new Framework1.usuarios.EditMaestroDialog(
                    Globales.Sqlite_Conex,
                    Globales.ID_Maestro,
                    Globales.Nombre_Maestro,
                    Globales.Correo_Maestro,

                    Globales.Facultad_Maestro,
                    Globales.Tipo_Maestro
                )
                {
                    Owner = this
                };

                bool? ok = dlg.ShowDialog();
                if (ok == true)
                {
                    // Refrescar grid
                    Mostrar_Datos_Grid();

                    // Opcional: limpiar controles de edición del panel izquierdo

                    MessageBox.Show("Usuario actualizado correctamente.", "Listo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir editor: " + ex.Message, "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Globales.Sqlite_Conex.Close();
        }

        //private void grid_maestros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        DataGrid dg = (DataGrid)sender;
        //        DataRowView fila = dg.SelectedItem as DataRowView;

        //        if (fila != null)
        //        {
        //            Globales.ID_Maestro = Convert.ToInt32(fila["ID"].ToString());
        //            Globales.Nombre_Maestro = fila["NOMBRE"].ToString();
        //            Globales.Correo_Maestro = fila["CORREO"].ToString();
        //            Globales.Facultad_Maestro = fila["FACULTAD"].ToString();
        //            Globales.Tipo_Maestro = fila["TIPO"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al seleccionar maestro: " + ex.Message);
        //    }
        //}



        private void grid_maestros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var dg = (DataGrid)sender;
                var fila = dg.SelectedItem as DataRowView;
                if (fila != null)
                {
                    Globales.ID_Maestro = Convert.ToInt32(fila["ID"]);
                    Globales.Nombre_Maestro = fila["NOMBRE"]?.ToString();
                    Globales.Correo_Maestro = fila["CORREO"]?.ToString();
                    Globales.Facultad_Maestro = fila["FACULTAD"]?.ToString(); // <-- NO "Facultad"
                    Globales.Tipo_Maestro = fila["TIPO"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar maestro: " + ex.Message);
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            bool haySeleccion = grid_maestros.SelectedItem != null;
            if (FindName("ctxEditar") is MenuItem m1) m1.IsEnabled = haySeleccion;
            if (FindName("ctxEliminar") is MenuItem m2) m2.IsEnabled = haySeleccion;
        }

        //private void Editar_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (grid_maestros.SelectedItem == null)
        //        {
        //            MessageBox.Show("Seleccione un maestro para editar.", "Aviso",
        //                MessageBoxButton.OK, MessageBoxImage.Information);
        //            return;
        //        }

        //        var dlg = new EditMaestroDialog(
        //            Globales.Sqlite_Conex,
        //            Globales.ID_Maestro,
        //            Globales.Nombre_Maestro,
        //            Globales.Correo_Maestro,
        //            Globales.Facultad_Maestro,
        //            Globales.Tipo_Maestro
        //        )
        //        {
        //            Owner = this
        //        };

        //        bool? ok = dlg.ShowDialog();
        //        if (ok == true)
        //        {
        //            Mostrar_Datos_Grid();
        //            MessageBox.Show("Maestro actualizado correctamente.", "Listo",
        //                MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al abrir editor: " + ex.Message, "Error",
        //            MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
        // using necesarios arriba del archivo:
        // using System.Windows.Controls;   // DataGridRow
        // using System.Windows.Input;      // MouseButtonEventArgs

        private void Row_RightClick_Select(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = true;
                grid_maestros.SelectedItem = row.Item;
                grid_maestros.CurrentItem = row.Item;
                grid_maestros.Focus();
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_maestros.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione un maestro.", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var r = MessageBox.Show($"¿Desea eliminar al maestro: {Globales.Nombre_Maestro} (ID: {Globales.ID_Maestro})?",
                                        "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (r != MessageBoxResult.Yes) return;

                const string sql = "DELETE FROM MAESTROS WHERE ID=@id";
                using (var cmd = new SQLiteCommand(sql, Globales.Sqlite_Conex))
                {
                    cmd.Parameters.AddWithValue("@id", Globales.ID_Maestro);
                    cmd.ExecuteNonQuery();
                }

                Mostrar_Datos_Grid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar maestro: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
