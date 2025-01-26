using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParaleloEjercicio1
{
    /// <summary>
    /// clase principal, inicializa la aplicacion
    /// </summary>
    public partial class Form1 : Form
    {
        private readonly MongoConnect _dbConnection;
        public Form1()
        {
            InitializeComponent();            
            _dbConnection = new MongoConnect("mongodb+srv://hvalmes:EusUnLm1ak4T897B116@paralelos.kiyod.mongodb.net/?retryWrites=true&w=majority", "heromovildb");
                       
            LoadSmartphoneNames();
            TexBoxDesactivados();

            // Configuracion el contexto de licencia de EPPlus (versión >= 5)
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        private void TexBoxDesactivados()
        {
            EditBtn.Enabled = false;
            TextId.Enabled = false;
            txtBrand.Enabled = false;
            txtModel.Enabled = false;
            txtRam.Enabled = false;
            txtRom.Enabled = false;
            txtScreenSize.Enabled = false;
            txtBattery.Enabled = false;
            txtPrice.Enabled = false;
            txtColor.Enabled = false;
            txtCameraPixels.Enabled = false;
            txtNetwork.Enabled = false;
            txtAvailability.Enabled = false;
            ActualizarBtn.Enabled = false;
            InsertarBtn.Enabled = false;
            txtBoxImg.Enabled = false;
        }

        /// <summary>
        /// Cargar telefonos
        /// </summary>
        private async void LoadSmartphoneNames()
        {
            //Llenar el ComboBox de equipos.
            try
            {
                await _dbConnection.InitializeDatabase();
                Console.WriteLine("Inicialización completada.");

                var collection = _dbConnection.GetSmartphoneCollection();

                // Obtener todos los smartphones (solo los nombres de los modelos)
                var smartphones = await collection.Find(_ => true).ToListAsync();

                // Limpiar el ListBox antes de agregar los nuevos elementos
                CajaTelef.Items.Clear();

                // Agregar los nombres de los smartphones al ListBox
                foreach (var smartphone in smartphones)
                {
                    CajaTelef.Items.Add(smartphone.Model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los nombres: {ex.Message}");
            }
        }       

        private async void button1_Click(object sender, EventArgs e)
        {
            //Consultar btn            
            try
            {
                TexBoxDesactivados();
                // Obtener el modelo seleccionado
                string selectedModel = CajaTelef.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(selectedModel))
                {
                    var collection = _dbConnection.GetSmartphoneCollection();

                    // Buscar el smartphone por modelo
                    var smartphone = await collection.Find(x => x.Model == selectedModel).FirstOrDefaultAsync();

                    if (smartphone != null)
                    {
                        // Cargar los datos en los TextBox
                        
                        TextId.Text = smartphone.Id;
                        txtBrand.Text = smartphone.Brand;
                        txtModel.Text = smartphone.Model;
                        txtRam.Text = smartphone.Ram.ToString();
                        txtRom.Text = smartphone.Rom.ToString();
                        txtScreenSize.Text = smartphone.ScreenSize.ToString();
                        txtBattery.Text = smartphone.Battery.ToString();
                        txtPrice.Text = smartphone.Price.ToString("C");
                        txtColor.Text = smartphone.Color;
                        txtCameraPixels.Text = smartphone.CameraPixels.ToString();
                        txtNetwork.Text = smartphone.Network;
                        txtAvailability.Text = smartphone.Availability;
                        ImgBox.ImageLocation = smartphone.Img;
                        txtBoxImg.Text = smartphone.Img;                       

                        //habilitar btn editar
                        EditBtn.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron los detalles del teléfono.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los detalles: {ex.Message}");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //Insertar
                var smartphone = new Smartphone
                {
                    Brand = txtBrand.Text,
                    Model = txtModel.Text,
                    Ram = int.Parse(txtRam.Text),
                    Rom = int.Parse(txtRom.Text),
                    ScreenSize = double.Parse(txtScreenSize.Text),
                    Battery = int.Parse(txtBattery.Text),
                    Price = double.Parse(txtPrice.Text),
                    Color = txtColor.Text,
                    CameraPixels = double.Parse(txtCameraPixels.Text),
                    Network = txtNetwork.Text,
                    Availability = txtAvailability.Text,
                    Img = txtBoxImg.Text,
                };

                await _dbConnection.InsertSmartphone(smartphone);

                MessageBox.Show("Smartphone insertado correctamente.");
                Limpiar();
                LoadSmartphoneNames();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al Insertar datos: {ex.Message}");
            }

        }

        /// <summary>
        /// Actualizacion de un articulo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ActualizarBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar el ID
                string id = TextId.Text;
                if (string.IsNullOrWhiteSpace(id))
                {
                    MessageBox.Show("El campo ID no puede estar vacío.");
                    return;
                }

                // Validar y convertir campos
                if (!int.TryParse(txtRam.Text, out int ram))
                {
                    MessageBox.Show("RAM debe ser un número entero válido.");
                    return;
                }

                if (!int.TryParse(txtRom.Text, out int rom))
                {
                    MessageBox.Show("ROM debe ser un número entero válido.");
                    return;
                }

                if (!double.TryParse(txtScreenSize.Text, out double screenSize))
                {
                    MessageBox.Show("El tamaño de pantalla debe ser un número válido.");
                    return;
                }

                if (!int.TryParse(txtBattery.Text, out int battery))
                {
                    MessageBox.Show("La batería debe ser un número entero válido.");
                    return;
                }

                if (!double.TryParse(txtPrice.Text, out double price))
                {
                    MessageBox.Show("El precio debe ser un número válido. Elimine ($)");
                    return;
                }

                if (!double.TryParse(txtCameraPixels.Text, out double cameraPixels))
                {
                    MessageBox.Show("Los píxeles de la cámara deben ser un número válido.");
                    return;
                }

                // Crear el objeto Smartphone
                var smartphone = new Smartphone
                {
                    Brand = txtBrand.Text,
                    Model = txtModel.Text,
                    Ram = ram,
                    Rom = rom,
                    ScreenSize = screenSize,
                    Battery = battery,
                    Price = price,
                    Color = txtColor.Text,
                    CameraPixels = cameraPixels,
                    Network = txtNetwork.Text,
                    Availability = txtAvailability.Text,
                    Img = txtBoxImg.Text

                };

                // Actualizar en la base de datos
                await _dbConnection.UpdateSmartphone(id, smartphone);

                MessageBox.Show("Smartphone actualizado correctamente.");
                button1_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar datos: {ex.Message}");
            }
        }


        private void EditBtn_Click(object sender, EventArgs e)
        {
            txtBrand.Enabled = true;
            txtModel.Enabled = true;
            txtRam.Enabled = true;
            txtRom.Enabled = true;
            txtScreenSize.Enabled = true;
            txtBattery.Enabled = true;
            txtPrice.Enabled = true;
            txtColor.Enabled = true;
            txtCameraPixels.Enabled = true;
            txtNetwork.Enabled = true;
            txtAvailability.Enabled = true;
            InsertarBtn.Enabled = false;
            ActualizarBtn.Enabled = true;
            txtBoxImg.Enabled = true;

        }

        private void NuevoBtn_Click(object sender, EventArgs e)
        {
            //Habilitar los Campos para insertar un Nuevo Registro
            Limpiar();
            TextId.Text = "";
            txtBrand.Enabled = true;
            txtModel.Enabled = true;
            txtRam.Enabled = true;
            txtRom.Enabled = true;
            txtScreenSize.Enabled = true;
            txtBattery.Enabled = true;
            txtPrice.Enabled = true;
            txtColor.Enabled = true;
            txtCameraPixels.Enabled = true;
            txtNetwork.Enabled = true;
            txtAvailability.Enabled = true;
            EditBtn.Enabled = false;
            ActualizarBtn.Enabled = false;            
            InsertarBtn.Enabled = Enabled;
            txtBoxImg.Enabled = true;
        }

        private void LimpiarBtn_Click(object sender, EventArgs e)
        {
            //Limpia y desahabilita btn
            Limpiar();
            TexBoxDesactivados();
            MessageBox.Show("Completado.");
        }

        private void Limpiar()
        {
            TextId.Text = "";
            txtBrand.Text = "";
            txtModel.Text = "";
            txtRam.Text = "";
            txtRom.Text = "";
            txtScreenSize.Text = "";
            txtBattery.Text = "";
            txtPrice.Text = "";
            txtColor.Text = "";
            txtCameraPixels.Text = "";
            txtNetwork.Text = "";
            txtAvailability.Text = "";
            CajaTelef.Text = "";
            txtBoxImg.Text = "";
            ImgBox.Image = null;
            EditBtn.Enabled = false;
            InsertarBtn.Enabled = false;
            ActualizarBtn.Enabled =false;
        }

        /// <summary>
        /// Proceso para cargar los datos desde excel, en lotes paralelos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CargaBtn_Click(object sender, EventArgs e)
        {
            // Crear y mostrar el formulario de carga
            using (var pantallaCarga = new PantallaCarga())
            {
                
                pantallaCarga.progressBar.Style = ProgressBarStyle.Marquee;
                pantallaCarga.StartPosition = FormStartPosition.CenterParent;

                try
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Archivos de Excel (*.xlsx;*.xls)|*.xlsx;*.xls";
                        openFileDialog.Title = "Seleccionar archivo Excel con datos de smartphones";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            pantallaCarga.Show();
                            string filePath = openFileDialog.FileName;

                            // Leer datos desde el archivo Excel
                            var smartphones = await ReadSmartphoneDataFromExcelAsync(filePath);

                            // Insertar datos en la base de datos
                            await InsertDataInBatches(smartphones, _dbConnection.GetSmartphoneCollection(), pantallaCarga);
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    pantallaCarga.Close();
                    MessageBox.Show("Datos cargados exitosamente.", "Carga Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private async Task InsertDataInBatches(List<Smartphone> smartphones, IMongoCollection<Smartphone> collection, PantallaCarga loadingForm)
        {
            int batchSize = 100; // Tamaño del lote
            for (int i = 0; i < smartphones.Count; i += batchSize)
            {
                var batch = smartphones.Skip(i).Take(batchSize).ToList();
                await collection.InsertManyAsync(batch);

                // Actualizar el progreso
                loadingForm.Invoke((MethodInvoker)delegate {
                    loadingForm.progressBar.Value = Math.Min(loadingForm.progressBar.Maximum, loadingForm.progressBar.Value + 1);
                });
            }
            LoadSmartphoneNames();
        }       

        /// <summary>
        /// Leer datos desde el archivo Excel, usando task.run para optimizar el proceso
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<List<Smartphone>> ReadSmartphoneDataFromExcelAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                var smartphones = new List<Smartphone>();

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Leer desde la Primera hoja
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Empezar en la segunda fila (evitar encabezados)
                    {
                        var smartphone = new Smartphone
                        {
                            Brand = worksheet.Cells[row, 1].Text,
                            Model = worksheet.Cells[row, 2].Text,
                            Ram = int.Parse(worksheet.Cells[row, 3].Text),
                            Rom = int.Parse(worksheet.Cells[row, 4].Text),
                            ScreenSize = double.Parse(worksheet.Cells[row, 5].Text),
                            Battery = int.Parse(worksheet.Cells[row, 6].Text),
                            Price = double.Parse(worksheet.Cells[row, 7].Text),
                            Color = worksheet.Cells[row, 8].Text,
                            CameraPixels = int.Parse(worksheet.Cells[row, 9].Text),
                            Network = worksheet.Cells[row, 10].Text,
                            Availability = worksheet.Cells[row, 11].Text,
                            Img = worksheet.Cells[row, 12].Text
                        };

                        smartphones.Add(smartphone);
                    }
                }

                return smartphones;
            });
        }

        /// <summary>
        /// Inserta datos en MongoDB en lotes paralelos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="collection"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        
        /*
        private async Task InsertDataInBatches<T>(IEnumerable<T> data, IMongoCollection<T> collection, int batchSize = 100)
        {
            var dataBatches = data.Select((item, index) => new { item, index })
                                  .GroupBy(x => x.index / batchSize)
                                  .Select(g => g.Select(x => x.item).ToList());

            var tasks = dataBatches.Select(batch => collection.InsertManyAsync(batch));
            await Task.WhenAll(tasks);
        }*/

        /// <summary>
        /// Exportar datos a excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ExtrBtn_Click(object sender, EventArgs e)
        {
            var exportForm = new ExportFilterForm();
            if (exportForm.ShowDialog() == DialogResult.OK)
            {
                string queryType = exportForm.QueryType;
                string priceComparison = exportForm.PriceComparison;
                double? priceValue = exportForm.PriceValue;
                string brand = exportForm.Brand;
                string model = exportForm.Model;

                SaveFileDialog saveFileDialog1 = new SaveFileDialog
                {
                    Filter = "Archivos de Excel (*.xlsx)|*.xlsx",
                    Title = "Guardar archivo de exportación"
                };
                SaveFileDialog saveFileDialog = saveFileDialog1;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await ExportSmartphonesAsync(saveFileDialog.FileName, queryType, priceComparison, priceValue, brand, model);
                }
            }
        }
        /*
        private void MostrarPantallaCarga()
        {
            PantallaCarga pantallaCarga = new PantallaCarga();
            pantallaCarga.StartPosition = FormStartPosition.CenterParent; // O FormStartPosition.CenterScreen
            pantallaCarga.Show();           
        }*/ //Presenta error.

        /// <summary>
        /// metodo para exportar a excel el inventario
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="queryType"></param>
        /// <param name="priceComparison"></param>
        /// <param name="priceValue"></param>
        /// <param name="brand"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ExportSmartphonesAsync(string filePath, string queryType, string priceComparison = null, double? priceValue = null, string brand = null, string model = null)
    {
            var pantallaCarga = new PantallaCarga();
            pantallaCarga.progressBar.Style = ProgressBarStyle.Marquee;
            pantallaCarga.StartPosition = FormStartPosition.CenterParent;

            try
            {
                pantallaCarga.Show();   
            var collection = _dbConnection.GetSmartphoneCollection();                

                // Seleccionar la consulta en función del tipo
                List<Smartphone> smartphones;

                switch (queryType)
                {
                    case "all": // Todo el inventario
                        smartphones = await collection.Find(_ => true).ToListAsync();
                        break;

                    case "price": // Filtrar por precio
                        var filter = priceComparison == "greater"
                            ? Builders<Smartphone>.Filter.Gt(s => s.Price, priceValue)
                            : Builders<Smartphone>.Filter.Lt(s => s.Price, priceValue);
                        smartphones = await collection.Find(filter).ToListAsync();
                        break;

                    case "brand_or_model": // Filtrar por marca o modelo
                        var brandModelFilter = Builders<Smartphone>.Filter.Or(
                            Builders<Smartphone>.Filter.Eq(s => s.Brand, brand),
                            Builders<Smartphone>.Filter.Eq(s => s.Model, model)
                        );
                        smartphones = await collection.Find(brandModelFilter).ToListAsync();
                        break;

                    default: // Caso por defecto si no se inidica el tipo de consulta
                        smartphones = new List<Smartphone>();
                        break;
                }

            // Crear el archivo de Excel
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Smartphones");

            // Encabezados
            worksheet.Cells[1, 1].Value = "Marca";
            worksheet.Cells[1, 2].Value = "Modelo";
            worksheet.Cells[1, 3].Value = "RAM";
            worksheet.Cells[1, 4].Value = "ROM";
            worksheet.Cells[1, 5].Value = "Pantalla";
            worksheet.Cells[1, 6].Value = "Batería";
            worksheet.Cells[1, 7].Value = "Precio";
            worksheet.Cells[1, 8].Value = "Color";
            worksheet.Cells[1, 9].Value = "Cámara";
            worksheet.Cells[1, 10].Value = "Red";
            worksheet.Cells[1, 11].Value = "Disponibilidad";

            // Rellenar datos
            int row = 2;
            foreach (var smartphone in smartphones)
            {
                worksheet.Cells[row, 1].Value = smartphone.Brand;
                worksheet.Cells[row, 2].Value = smartphone.Model;
                worksheet.Cells[row, 3].Value = smartphone.Ram;
                worksheet.Cells[row, 4].Value = smartphone.Rom;
                worksheet.Cells[row, 5].Value = smartphone.ScreenSize;
                worksheet.Cells[row, 6].Value = smartphone.Battery;
                worksheet.Cells[row, 7].Value = smartphone.Price;
                worksheet.Cells[row, 8].Value = smartphone.Color;
                worksheet.Cells[row, 9].Value = smartphone.CameraPixels;
                worksheet.Cells[row, 10].Value = smartphone.Network;
                worksheet.Cells[row, 11].Value = smartphone.Availability;
                row++;
            }

            // Estilos básicos
            worksheet.Cells["A1:K1"].Style.Font.Bold = true;
            worksheet.Cells.AutoFitColumns();

            // Guardar el archivo
            package.SaveAs(new FileInfo(filePath));


            //Console.WriteLine("Datos exportados exitosamente.");
            
               MessageBox.Show($"Datos Exportados Existosamente en: {filePath.ToString()}");
                pantallaCarga.Close();
            }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error al exportar datos: {ex.Message}");
                MessageBox.Show($"Error en el proceso: {ex.Message}");
            }
    }

}

}

