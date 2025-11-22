using Framework1.conexion_dba;
using Framework1.principal;
using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows.Input;


using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Windows;




namespace Framework1.usuarios
{
    public partial class alumnos_edit : System.Windows.Window
    {
        public alumnos_edit()
        {
            InitializeComponent();
            Globales.Sqlite_Conex.Open();
            Cargar_Semestre_Alumno();
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
                System.Data.DataTable tabla_alumnos = new System.Data.DataTable("ALUMNOS");
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




        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txt_expediente.Text == "")
                {
                    MessageBox.Show("No existe información de ID: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (txt_nombre.Text == "")
                {
                    MessageBox.Show("No existe información en Nombre: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (txt_correo.Text == "")
                {
                    MessageBox.Show("No existe información en clave: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (cb_semestre.Text == "" || cb_semestre.Text == "Semestre")
                {
                    MessageBox.Show("No existe información en Semestre: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (txt_edad.Text == "")
                {
                    MessageBox.Show("No existe información en edad: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (cb_genero.Text == "" || cb_genero.Text == "-- Seleccione Genero --")
                {
                    MessageBox.Show("No existe información en tipo: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBoxResult result;
                    result = MessageBox.Show("¿Esta seguro que deseas agregar al alumno con expediente: " + txt_expediente + ", nombre: " + txt_nombre.Text + "?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes)
                    {
                        Agregar_Usuario_Nuevo();
                        Mostrar_Datos_Grid();
                        txt_expediente.Clear();
                        txt_nombre.Clear();
                        txt_correo.Clear();
                        cb_semestre.Text = "Semestre";
                        txt_edad.Clear();
                        cb_genero.Text = "-- Seleccione el tipo de usuario --";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 404 Agregar Usuario: " + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }














        private void Agregar_Usuario_Nuevo()
        {
            try
            {
                string sql_insert = "INSERT INTO ALUMNOS (Expediente, Nombre, Semestre, Correo, Edad, Genero)" +
                    "VALUES(@Expediente, @Nombre, @Semestre, @Correo, @Edad, @Genero)";
                SQLiteCommand cmd_insert = new SQLiteCommand(sql_insert, Globales.Sqlite_Conex);
                cmd_insert.Parameters.AddWithValue("@Expediente", txt_expediente.Text);
                cmd_insert.Parameters.AddWithValue("@Nombre", txt_nombre.Text);
                cmd_insert.Parameters.AddWithValue("@Semestre", cb_semestre.Text);
                cmd_insert.Parameters.AddWithValue("@Correo", txt_correo.Text);
                cmd_insert.Parameters.AddWithValue("@Edad", txt_edad.Text);

                cmd_insert.Parameters.AddWithValue("@Genero", cb_genero.Text);

                cmd_insert.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 404 Agregar Usuario: " + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }












        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_alumnos.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Elija al menos un usuario: ", "Error 404", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBoxResult result;
                    result = MessageBox.Show("¿Esta seguro que desea eliminar al alumno: " + Globales.Nombre_Alumno + " con Expediente: " + Globales.Expediente_Alumno + "?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes)
                    {
                        string sql = "DELETE FROM Alumnos WHERE Expediente=" + Globales.Expediente_Alumno;
                        SQLiteCommand Cmd_delete = new SQLiteCommand(sql, Globales.Sqlite_Conex);
                        Cmd_delete.ExecuteNonQuery();
                        Mostrar_Datos_Grid();
                        MessageBox.Show("Usuario eliminado correctamente", "Eliminar Usuario", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 404 Eliminar Usuario: " + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }







        private void CrearLista_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cb_semestre.Text == "Semestre" || cb_semestre.Text == "" || cb_semestre.Text == "Todos los semestres")
                {
                    MessageBox.Show("No se puede realizar una lista, elige un semestre válido", "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBoxResult result;
                    result = MessageBox.Show("Se creará la lista del semestre selccionado. ¿Deseas continuar? ",
                                             "Crear Lista",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Lista_Excel(); //Se manda a llamar el método para crear la lista
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar crear la lista" + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void Lista_Excel()
        {
            Excel.Application excel = null;
            Workbook exLibro = null;
            SQLiteDataReader Reader_Alumnos = null;

            try
            {
                // Construir rutas correctas
                string rutaSemestre = Globales.Ruta_Aplicacion + "/Recursos/Semestres/" + cb_semestre.Text + "-Semestre";
                string rutaListas = rutaSemestre + "/Listas";

                // Crear directorios si no existen
                if (!Directory.Exists(rutaSemestre))
                    Directory.CreateDirectory(rutaSemestre);

                if (!Directory.Exists(rutaListas))
                    Directory.CreateDirectory(rutaListas);

                string archivoLista = Path.Combine(rutaListas, "Lista de " + cb_semestre.Text + ".xlsx");

                // Verificar si el archivo existe y eliminarlo
                if (File.Exists(archivoLista))
                {
                    File.Delete(archivoLista);
                }

                // Verificar que la plantilla existe
                string rutaPlantilla = Globales.Ruta_Aplicacion + "/Recursos/Plantillas/Listas.xlsx";
                if (!File.Exists(rutaPlantilla))
                {
                    MessageBox.Show("No se encuentra la plantilla de listas: " + rutaPlantilla,
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Crear instancia de Excel
                excel = new Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false; // Importante: deshabilitar alertas

                // Abrir la plantilla
                exLibro = excel.Workbooks.Open(rutaPlantilla);

                // Consulta SQL para alumnos del semestre
                string Sql_Alumnos = "SELECT * FROM ALUMNOS WHERE SEMESTRE = '" + cb_semestre.Text + "' ORDER BY Nombre";
                SQLiteCommand Cmd_Alumnos = new SQLiteCommand(Sql_Alumnos, Globales.Sqlite_Conex);
                Reader_Alumnos = Cmd_Alumnos.ExecuteReader();

                int j = 0; // Contador para filas

                // Llenar datos en Excel
                while (Reader_Alumnos.Read())
                {
                    try
                    {
                        // Hoja "Lista"
                        exLibro.Worksheets["Lista"].Cells[j + 6, 2] = Convert.ToInt64(Reader_Alumnos["Expediente"].ToString());
                        exLibro.Worksheets["Lista"].Cells[j + 6, 3] = Reader_Alumnos["Nombre"].ToString();
                        exLibro.Worksheets["Lista"].Cells[j + 6, 1] = j + 1; // Numeración

                        // Hoja "Calificaciones"
                        exLibro.Worksheets["Calificaciones"].Cells[j + 8, 2] = Convert.ToInt64(Reader_Alumnos["Expediente"].ToString());
                        exLibro.Worksheets["Calificaciones"].Cells[j + 8, 3] = Reader_Alumnos["Nombre"].ToString();
                        exLibro.Worksheets["Calificaciones"].Cells[j + 8, 1] = j + 1; // Numeración

                        j++; // Incrementar contador
                    }
                    catch (Exception exFila)
                    {
                        Console.WriteLine("Error procesando alumno: " + exFila.Message);
                        continue; // Continuar con el siguiente alumno
                    }
                }

                Reader_Alumnos.Close();

                // Guardar como nuevo archivo
                exLibro.SaveAs(archivoLista, Excel.XlFileFormat.xlOpenXMLWorkbook);

                // Cerrar y liberar recursos
                exLibro.Close(false);
                excel.Quit();

                // Liberar objetos COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(exLibro);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                exLibro = null;
                excel = null;

                // Forzar garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();

                MessageBox.Show($"Lista creada exitosamente para el semestre {cb_semestre.Text}\n\nArchivo: {archivoLista}",
                               "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Abrir la carpeta donde se guardó la lista
                Process.Start("explorer.exe", rutaListas);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la lista de Excel: " + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Limpieza segura de recursos
                try
                {
                    if (Reader_Alumnos != null && !Reader_Alumnos.IsClosed)
                        Reader_Alumnos.Close();
                }
                catch { }

                try
                {
                    if (exLibro != null)
                    {
                        exLibro.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(exLibro);
                    }
                }
                catch { }

                try
                {
                    if (excel != null)
                    {
                        excel.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                    }
                }
                catch { }
            }
        }


        private void Eliminar_PDF_S()
        {
            try
            {
                string rutaFichas = Globales.Ruta_Aplicacion + "/Recursos/Semestres/" + cb_semestre.Text + "-Semestre/Fichas";

                Console.WriteLine("Intentando eliminar PDFs de: " + rutaFichas);

                // Verificar si la carpeta existe
                if (Directory.Exists(rutaFichas))
                {
                    string[] files = Directory.GetFiles(rutaFichas, "*.pdf");

                    int eliminados = 0;
                    foreach (string file in files)
                    {
                        try
                        {
                            File.Delete(file);
                            eliminados++;
                            Console.WriteLine("Eliminado: " + file);
                        }
                        catch (Exception exFile)
                        {
                            Console.WriteLine("Error eliminando " + file + ": " + exFile.Message);
                        }
                    }

                    Console.WriteLine($"Se eliminaron {eliminados} archivos PDF");
                }
                else
                {
                    Console.WriteLine("La carpeta no existe: " + rutaFichas);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Eliminar_PDF_S: " + ex.Message);
                // No mostrar MessageBox aquí para no interrumpir el flujo
            }
        }


        private void Crear_Fichas()
        {
            Excel.Application excel = null;
            Workbook exLibro = null;
            SQLiteDataReader Reader_Alumnos = null;

            try
            {
                // Construir rutas específicas para tu estructura
                string rutaSemestre = Globales.Ruta_Aplicacion + "/Recursos/Semestres/" + cb_semestre.Text + "-Semestre";
                string rutaFichas = rutaSemestre + "/Fichas";

                // Mostrar información de depuración
                Console.WriteLine("Ruta semestre: " + rutaSemestre);
                Console.WriteLine("Ruta fichas: " + rutaFichas);

                // Crear directorios si no existen
                if (!Directory.Exists(rutaSemestre))
                {
                    Directory.CreateDirectory(rutaSemestre);
                    Console.WriteLine("Carpeta de semestre creada: " + rutaSemestre);
                }

                if (!Directory.Exists(rutaFichas))
                {
                    Directory.CreateDirectory(rutaFichas);
                    Console.WriteLine("Carpeta de fichas creada: " + rutaFichas);
                }

                // Verificar que la plantilla existe
                string rutaPlantilla = Globales.Ruta_Aplicacion + "/Recursos/Plantillas/Fichas.xlsx";
                if (!File.Exists(rutaPlantilla))
                {
                    MessageBox.Show("No se encuentra la plantilla de fichas: " + rutaPlantilla,
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Eliminar_PDF_S(); // Eliminar PDFs existentes

                // Crea una instancia de Excel
                excel = new Excel.Application();
                excel.Visible = false; // Ocultar Excel
                excel.DisplayAlerts = false;

                // Abrir la plantilla
                exLibro = excel.Workbooks.Open(rutaPlantilla);

                // Consulta SQL para alumnos del semestre
                string Sql_Alumnos = "SELECT * FROM ALUMNOS WHERE SEMESTRE = '" + cb_semestre.Text + "' ORDER BY Nombre";
                SQLiteCommand Cmd_Alumnos = new SQLiteCommand(Sql_Alumnos, Globales.Sqlite_Conex);
                Reader_Alumnos = Cmd_Alumnos.ExecuteReader();

                int contador = 0;

                // Procesar cada alumno
                while (Reader_Alumnos.Read())
                {
                    try
                    {
                        // Llenar datos en la plantilla
                        exLibro.Worksheets["FichaAlumno"].Cells[4, 4] = Convert.ToInt64(Reader_Alumnos["Expediente"].ToString());
                        exLibro.Worksheets["FichaAlumno"].Cells[5, 4] = Reader_Alumnos["Nombre"].ToString();
                        exLibro.Worksheets["FichaAlumno"].Cells[6, 4] = Reader_Alumnos["Semestre"].ToString();
                        exLibro.Worksheets["FichaAlumno"].Cells[7, 4] = Reader_Alumnos["Correo"].ToString();
                        exLibro.Worksheets["FichaAlumno"].Cells[9, 4] = Convert.ToInt32(Reader_Alumnos["Edad"].ToString());
                        exLibro.Worksheets["FichaAlumno"].Cells[11, 4] = Reader_Alumnos["Genero"].ToString();

                        // Campos opcionales
                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[8, 4] = Reader_Alumnos["FechaNacimiento"].ToString();
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[8, 4] = "No especificado";
                        }

                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[13, 4] = Reader_Alumnos["Direccion"].ToString();
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[13, 4] = "No especificado";
                        }

                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[14, 4] = Reader_Alumnos["TelefonoAlumno"].ToString();
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[14, 4] = "No especificado";
                        }

                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[17, 4] = Reader_Alumnos["NumeroContacto"].ToString();
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[17, 4] = "No especificado";
                        }

                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[18, 4] = Reader_Alumnos["LugarNacimiento"].ToString();
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[18, 4] = "No especificado";
                        }

                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[19, 4] = Reader_Alumnos["Tutor"].ToString();
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[19, 4] = "No especificado";
                        }

                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[20, 4] = Reader_Alumnos["Tutora"].ToString();
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[20, 4] = "No especificado";
                        }

                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[22, 4] = Convert.ToInt32(Reader_Alumnos["TotalNA"]?.ToString() ?? "0");
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[22, 4] = 0;
                        }

                        try
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[24, 4] = Reader_Alumnos["Alergias"].ToString();
                        }
                        catch
                        {
                            exLibro.Worksheets["FichaAlumno"].Cells[24, 4] = "No especificado";
                        }

                        // Nombre del archivo PDF (más corto para evitar problemas de ruta)
                        string nombreArchivo = $"Ficha_{Reader_Alumnos["Expediente"]}_{Reader_Alumnos["Nombre"].ToString().Replace(" ", "_")}.pdf";
                        string rutaCompleta = Path.Combine(rutaFichas, nombreArchivo);

                        // Exportar a PDF
                        exLibro.ExportAsFixedFormat(
                            XlFixedFormatType.xlTypePDF,
                            rutaCompleta,
                            XlFixedFormatQuality.xlQualityStandard,
                            IncludeDocProperties: true,
                            IgnorePrintAreas: false,
                            OpenAfterPublish: false
                        );

                        contador++;
                        Console.WriteLine("Ficha creada: " + nombreArchivo);
                    }
                    catch (Exception exAlumno)
                    {
                        Console.WriteLine("Error con alumno " + Reader_Alumnos["Expediente"] + ": " + exAlumno.Message);
                    }
                }

                Reader_Alumnos.Close();

                // Cerrar el libro sin guardar cambios (ya que exportamos PDFs)
                exLibro.Close(false);
                excel.Quit();

                // Liberar objetos COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(exLibro);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                exLibro = null;
                excel = null;

                // Forzar garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Mostrar resultados
                if (contador > 0)
                {
                    MessageBox.Show($"Se crearon {contador} fichas exitosamente en:\n{rutaFichas}",
                                   "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Abrir la carpeta de fichas
                    Process.Start("explorer.exe", rutaFichas);
                }
                else
                {
                    MessageBox.Show("No se crearon fichas. Verifique que haya alumnos en el semestre seleccionado.",
                                   "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear las fichas: " + ex.Message +
                               "\n\nDetalles: " + ex.StackTrace,
                               "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Limpieza segura de recursos
                try
                {
                    if (Reader_Alumnos != null && !Reader_Alumnos.IsClosed)
                        Reader_Alumnos.Close();
                }
                catch { }

                try
                {
                    if (exLibro != null)
                    {
                        exLibro.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(exLibro);
                    }
                }
                catch { }

                try
                {
                    if (excel != null)
                    {
                        excel.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                    }
                }
                catch { }
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







        private void CrearFicha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cb_semestre.Text == "Semestre" || cb_semestre.Text == "" || cb_semestre.Text == "Todos los semestres")
                {
                    MessageBox.Show("No se puede realizar una lista, elige un semestre válido", "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBoxResult result;
                    result = MessageBox.Show("Se crearán las fichas individuales de cada alumno del semestre selccionado. ¿Deseas continuar? ",
                                             "Crear Lista",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Crear_Fichas(); //Se manda a llamar el método para crear la ficha
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar crear la ficha" + ex.Message, "Error 404", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void Cargar_Semestre_Alumno()
        {
            try
            {
                cb_semestre.Items.Add("Todos los semestres");

                string Sql_Semestres = "SELECT DISTINCT SEMESTRE FROM ALUMNOS ORDER BY SEMESTRE";
                SQLiteCommand Cmd_Semestres = new SQLiteCommand(Sql_Semestres, Globales.Sqlite_Conex);
                SQLiteDataReader Lector_Semestres = Cmd_Semestres.ExecuteReader();

                while (Lector_Semestres.Read())
                {
                    cb_semestre.Items.Add(Lector_Semestres["SEMESTRE"].ToString());
                }
                Lector_Semestres.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar semestres: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            if (grid_alumnos.SelectedItems.Count != 1)
            {
                MessageBox.Show("Seleccione un alumno para actualizar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                // Aquí puedes implementar tu ventana de actualización
                Editar_Click(sender, e); // O llamar a tu propia ventana de actualización
            }
        }





        private void Reiniciar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            cb_semestre.Text = "-- Seleccione Semestre --";
            Mostrar_Datos_Grid();
        }

        private void LimpiarCampos()
        {
            txt_expediente.Clear();
            txt_nombre.Clear();
            txt_correo.Clear();
            txt_edad.Clear();
            cb_genero.Text = "-- Seleccione Genero --";
        }


        private void cb_semestre_DropDownClosed(object sender, EventArgs e)
        {
            Mostrar_Datos_Grid_Filtrado();
        }

        private void Mostrar_Datos_Grid_Filtrado()
        {
            try
            {
                if (cb_semestre.Text == "Todos los semestres" || string.IsNullOrEmpty(cb_semestre.Text))
                {
                    Mostrar_Datos_Grid();
                }
                else
                {
                    string Consulta_Sql = "SELECT * FROM ALUMNOS WHERE SEMESTRE = @SEMESTRE ORDER BY NOMBRE";
                    SQLiteCommand Cmd_Alumnos = new SQLiteCommand(Consulta_Sql, Globales.Sqlite_Conex);
                    Cmd_Alumnos.Parameters.AddWithValue("@SEMESTRE", cb_semestre.Text);
                    Cmd_Alumnos.ExecuteNonQuery();

                    SQLiteDataAdapter adaptador = new SQLiteDataAdapter(Cmd_Alumnos);
                    System.Data.DataTable tabla_alumnos = new System.Data.DataTable("ALUMNOS");
                    adaptador.Fill(tabla_alumnos);

                    grid_alumnos.ItemsSource = tabla_alumnos.DefaultView;
                    adaptador.Update(tabla_alumnos);
                }

                // Actualizar contador
                // Label_Total.Content = "Total: " + grid_alumnos.Items.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar alumnos: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }



        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            bool haySeleccion = grid_alumnos.SelectedItem != null;
            if (FindName("ctxEditar") is System.Windows.Controls.MenuItem m1) m1.IsEnabled = haySeleccion;
            if (FindName("ctxEliminar") is System.Windows.Controls.MenuItem m2) m2.IsEnabled = haySeleccion;
        }

        private void Boton_Regresar_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.Show();
            this.Close();
        }







    }
}
