using MongoDB.Driver;
using System;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ParaleloEjercicio1
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configuración de la aplicación Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cadena de conexión y nombre de base de datos
            string connectionString = "mongodb+srv://hvalmes:EusUnLm1ak4T897B116@paralelos.kiyod.mongodb.net/?retryWrites=true&w=majority";
            
            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("heromovildb");

                // Probar la autenticación listando las colecciones
                var collections = database.ListCollectionNames().ToList();
                Console.WriteLine("Autenticación exitosa. Colecciones encontradas:");
                foreach (var collection in collections)
                {

                    Console.WriteLine(collection);                    

                }
            }
            catch (MongoDB.Driver.Core.Authentication.GssapiException ex)
            {
                Console.WriteLine("Error de autenticación: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
            }

            // Ejecutar la aplicación principal
            Application.Run(new Form1());
        }
    }
}
