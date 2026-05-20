using CustomGameDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomGameDB.controles
{
    public partial class ConfiguracionUsuario : UserControl
    {
        List<Amistade> amigosSinAceptar = new List<Amistade>();
        public ConfiguracionUsuario()
        {
            InitializeComponent();
            CargarReviews();



        }




        public void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            TxtNombre.Text = ((App)Application.Current).usuarioLogeado.Username;
            TxtCorreo.Text = ((App)Application.Current).usuarioLogeado.Email;
            TxtFechaNac.Text = ((App)Application.Current).usuarioLogeado.Anyonacimiento.Value.ToString("dd / MM / yyyy");
            using (var db = new NeondbContext())
            {
                int idLogeado = ((App)Application.Current).usuarioLogeado.Idusuario;
                var solicitudesFiltradas = db.Amistades
                    .Include(a => a.IdUsuario1Navigation)
                    .ToList()
                    .Where(a => a.Estado == "Pendiente" && a.IdUsuario2 == idLogeado)
                    .Select(a => new
                    {
                        IdAmistad = a.IdUsuario1,
                        NombreRemitente = a.IdUsuario1Navigation != null ? a.IdUsuario1Navigation.Username : "Usuario Desconocido",
                        EstadoSolicitud = a.Estado
                    })
                    .ToList();
                dgsolicitudesSinAceptar.ItemsSource = solicitudesFiltradas;
            }

        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {

            string textoBusqueda = InputBusqueda.Text.Trim();
            if (string.IsNullOrEmpty(textoBusqueda))
            {
                MessageBox.Show("Por favor, escribe un nombre de usuario para buscar.", "Campo vacío", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int idLogeado = ((App)Application.Current).usuarioLogeado.Idusuario;

            using (var db = new NeondbContext())
            {
                var usuariosEnMemoria = db.Usuarios.ToList();
                Usuario userAmistad = usuariosEnMemoria
                    .FirstOrDefault(u => u.Username.Equals(textoBusqueda, StringComparison.OrdinalIgnoreCase));

                if (userAmistad != null)
                {
                    if (userAmistad.Idusuario == idLogeado)
                    {
                        MessageBox.Show("No puedes enviarte una solicitud de amistad a ti mismo.", "Operación no válida", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    bool yaExisteRelacion = db.Amistades.Any(a =>
                        (a.IdUsuario1 == idLogeado && a.IdUsuario2 == userAmistad.Idusuario) ||
                        (a.IdUsuario1 == userAmistad.Idusuario && a.IdUsuario2 == idLogeado));

                    if (yaExisteRelacion)
                    {
                        MessageBox.Show($"Ya existe una solicitud o una relación de amistad en curso con {userAmistad.Username}.", "Solicitud duplicada", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    MessageBox.Show("Enviando solicitud de amistad a " + userAmistad.Username, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    db.Amistades.Add(new Amistade
                    {
                        IdUsuario1 = idLogeado,
                        IdUsuario2 = userAmistad.Idusuario,
                        Estado = "Pendiente"
                    });

                    db.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Usuario no encontrado. Verifica si el nombre está bien escrito.", "Sin resultados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }






        private void BtnAgregarAmigo_Click(object sender, RoutedEventArgs e)
        {
            int idLogeado = ((App)Application.Current).usuarioLogeado.Idusuario;
            if (dgsolicitudesSinAceptar.SelectedItem != null)
            {
                dynamic filaSeleccionada = dgsolicitudesSinAceptar.SelectedItem;

                int idAmistadAEditar = filaSeleccionada.IdAmistad;

                using (var db = new NeondbContext())
                {
                    Amistade amistad = db.Amistades.Find(idAmistadAEditar, idLogeado);

                    if (amistad != null)
                    {
                        amistad.Estado = "Aceptada";
                        db.SaveChanges();
                        MessageBox.Show("Solicitud de amistad aceptada", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        var solicitudesActualizadas = db.Amistades
                            .Include(a => a.IdUsuario1Navigation)
                            .ToList()
                            .Where(a => a.Estado == "Pendiente" && a.IdUsuario2 == idLogeado)
                            .Select(a => new
                            {
                                IdAmistad = a.IdUsuario2,
                                NombreRemitente = a.IdUsuario1Navigation != null ? a.IdUsuario1Navigation.Username : "Usuario Desconocido",
                                EstadoSolicitud = a.Estado
                            })
                            .ToList();

                        dgsolicitudesSinAceptar.ItemsSource = solicitudesActualizadas;
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el registro de la solicitud en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una solicitud de la lista para aceptarla.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        ObservableCollection<CargarReview> reviewsObservable = new ObservableCollection<CargarReview>();
        ObservableCollection<CargarReview> reviewsObservableFavoritas = new ObservableCollection<CargarReview>();
        public void CargarReviews()
        {
            int idUsuarioActual = ((App)Application.Current).usuarioLogeado.Idusuario;
            using (var db = new NeondbContext())
            {
                List<CargarReview> reviewsCargadas = new List<CargarReview>();
                reviewsCargadas = db.ReviewsUsuarios
                    .Where(r => r.IdUsuario == idUsuarioActual)
        .Select(r => new CargarReview
        {
            NombreUsuario = r.IdUsuarioNavigation.Username ?? "Usuario desconocido",
            NombreJuego = r.IdGameNavigation.Name ?? "Juego Desconocido",
            HorasJugadas = r.HorasJugadas ?? 0,
            NotaPersonal = r.NotaPersonal,
            EstadoJuego = r.Estadojuego ?? "No especificado",
            ReviewTexto = string.IsNullOrEmpty(r.ReviewTexto) ? "El usuario no ha escrito ninguna reseña todavía." : r.ReviewTexto,
            RutaJuego = r.rutaJuego,
            esFavorito = r.Esfavorito
        })
        .ToList();

                reviewsObservable = new ObservableCollection<CargarReview>(reviewsCargadas);
                reviewsObservableFavoritas = new ObservableCollection<CargarReview>(reviewsCargadas.Where(r => r.esFavorito == true).ToList());
                dgReviews.ItemsSource = reviewsObservable;

            }
        }

        private void BtnJugar_Click(object sender, RoutedEventArgs e)
        {
            Button botonJugar = sender as Button;

            if (botonJugar != null && botonJugar.CommandParameter != null)
            {
                string ruta = botonJugar.CommandParameter.ToString();

                if (string.IsNullOrWhiteSpace(ruta))
                {
                    MessageBox.Show("Este juego no tiene una ruta de ejecución configurada.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    if (File.Exists(ruta))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = ruta,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró el archivo ejecutable en la ruta especificada:\n{ruta}", "Archivo no encontrado", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo iniciar el juego. Error: {ex.Message}", "Error de ejecución", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void chkFavoritos_Checked(object sender, RoutedEventArgs e)
        {
            dgReviews.ItemsSource = reviewsObservableFavoritas;

        }

        private void chkFavoritos_Unchecked(object sender, RoutedEventArgs e)
        {
            dgReviews.ItemsSource = reviewsObservable;
        }
    }

}
