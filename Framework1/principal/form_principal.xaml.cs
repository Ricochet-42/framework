using Framework1.usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Framework1.principal
{
    /// <summary>
    /// Lógica de interacción para form_principal.xaml
    /// </summary>
    public partial class form_principal : Window
    {
        public form_principal()
        {
            InitializeComponent();
        }

        private void boton_home_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void boton_home_Click(object sender, RoutedEventArgs e)
        {
            usuarios_edit user = new usuarios_edit();
            user.ShowDialog();
        }
    }
}
