using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework1.conexion_dba
{
    internal class conex_db
    {
    }

    public class Globales
    {
        // Conexión SQLite
        public static SQLiteConnection Sqlite_Conex = new SQLiteConnection(@"Data Source=Recursos/Database/Base_Frameworks.db;Version=3;");
        public static string Ruta_Aplicacion = Directory.GetCurrentDirectory();

        // =========================
        // Variables globales USUARIOS
        // =========================
        public static int ID_Usuario;
        public static string Nombre_Usuario;
        public static string Clave_Usuario;
        public static string Correo_Usuario;
        public static string Tipo_Usuario;

        // =========================
        // Variables globales MATERIAS
        // =========================
        public static int ID_Materia;
        public static string Nombre_Materia;
        public static int Semestre_Materia;

        // =========================
        // Variables globales MAESTROS
        // =========================
        public static int ID_Maestro;
        public static string Nombre_Maestro;
        public static string Correo_Maestro;
        public static string Facultad_Maestro;
        public static string Tipo_Maestro;

        // =========================
        // Variables globales ALUMNOS
        // =========================
        public static int Expediente_Alumno;
        public static string Nombre_Alumno;
        public static int Semestre_Alumno;
        public static string Correo_Alumno;
        public static int Edad_Alumno;
        public static string Genero_Alumno;
    }
}
