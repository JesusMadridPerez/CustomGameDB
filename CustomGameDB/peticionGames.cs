using CustomGameDB.trailers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace CustomGameDB
{

    public class peticionGames
    {
        HttpClient client;
        int page_size = 20;
        String search = "";
        int page = 1;

        public peticionGames()
        {
            client = new HttpClient();
        }

        public async Task<GameResults> GetGames(int page_size, int page, string search)
        {
            GameResults gameResults = new GameResults();

            try
            {
                // 1. Limpiamos y preparamos el término de búsqueda
                string searchTerm = string.IsNullOrWhiteSpace(search) ? "" : Uri.EscapeDataString(search.Trim());

                // 2. Construimos la URL de forma dinámica
                // Usamos el page_size que viene por parámetro para ser consistentes
                string url = $"https://api.rawg.io/api/games?key=c5ebc9f26b6642c0a6b92e0fe19ebd11&page_size={page_size}&page={page}";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    url += $"&search={searchTerm}";
                }

                // 3. Petición
                HttpResponseMessage response = await client.GetAsync(url);


                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();

                var deserialized = System.Text.Json.JsonSerializer.Deserialize<GameResults>(responseJson);

                if (deserialized != null)
                    gameResults = deserialized;
            }
            catch (HttpRequestException ex)
            {
                // Error específico de red o HTTP (ej. 404 de página no encontrada)
                MessageBox.Show($"Error de red: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}");
            }

            gameResults.results ??= new ObservableCollection<Game>();
            return gameResults;
        }

        gameTrailers trailers = new gameTrailers();

        public async Task<gameTrailers> getTrailer(int id)
        {
            try
            {

                String url = $"https://api.rawg.io/api/games/{id}/movies?key=c5ebc9f26b6642c0a6b92e0fe19ebd11";

                HttpResponseMessage response = await client.GetAsync(url);


                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();
                
                var deserialized = System.Text.Json.JsonSerializer.Deserialize<gameTrailers>(responseJson);

                if (deserialized != null)
                    trailers = deserialized;

               
                //trailers.results ??= new ObservableCollection<GameTrailerResult>();
                return trailers;

            }
            catch (HttpRequestException ex)
            {
                // Error específico de red o HTTP (ej. 404 de página no encontrada)
                MessageBox.Show($"Error de red: {ex.Message}");
                return trailers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}");
                return trailers;


            }
        }

    }

}


