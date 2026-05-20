using CustomGameDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Amigos : UserControl
    {
        public Amigos()
        {
            InitializeComponent();
            CargarAmigos();
        }

        public void CargarAmigos()
        {
            List<DatosAmigo> listaAmigos = new List<DatosAmigo>();
            int idLogeado = ((App)Application.Current).usuarioLogeado.Idusuario;
            using (var db = new NeondbContext())
            {

                listaAmigos = db.Amistades
                    
                    .Where(a => a.IdUsuario1 == idLogeado || a.IdUsuario2 == idLogeado)
                    .Select(a => new DatosAmigo
                    {
                        IdAmigo = a.IdUsuario1 == idLogeado ? a.IdUsuario2 : a.IdUsuario1,
                        NombreAmigo = a.IdUsuario1 == idLogeado ? db.Usuarios.FirstOrDefault(u => u.Idusuario == a.IdUsuario2).Username : db.Usuarios.FirstOrDefault(u => u.Idusuario == a.IdUsuario1).Username,
                        FechaAmistad = a.FechaAmistad


                    })
                    .ToList();
                
            }
            dgAmigos.ItemsSource = listaAmigos;
        }


        ObservableCollection<CargarReview> reviewsObservable = new ObservableCollection<CargarReview>();
        ObservableCollection<CargarReview> reviewsObservableFavoritas = new ObservableCollection<CargarReview>();
        private void BtnVerJuegos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Boton ver juego amigos");
            var boton = (Button)sender;
            DatosAmigo amigoSeleccionado = (DatosAmigo)boton.DataContext;

            using (var db = new NeondbContext())
            {
                List<CargarReview> reviewsCargadas = new List<CargarReview>();
                reviewsCargadas = db.ReviewsUsuarios
        .Where(r => r.IdUsuario == amigoSeleccionado.IdAmigo)
        .Select(r => new CargarReview
        {
            NombreUsuario = r.IdUsuarioNavigation.Username ?? "Usuario Desconocido",
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
        private void BtnChat_Click(object sender, RoutedEventArgs e)
        {
            var boton = (Button)sender;
            DatosAmigo amigoSeleccionado = (DatosAmigo)boton.DataContext;

            MessageBox.Show($"Abriendo chat privado con: {amigoSeleccionado.NombreAmigo}");

        }

    }
}
