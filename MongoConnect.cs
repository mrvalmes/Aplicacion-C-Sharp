using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MongoConnect
{
    private readonly IMongoDatabase _database;
    /// <summary>
    /// Conexion a la base de datos.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="databaseName"></param>
    public MongoConnect(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Smartphone> GetSmartphoneCollection()
    {
        return _database.GetCollection<Smartphone>("smartphones");
    }

    public IMongoCollection<Image> GetImageCollection()
    {
        return _database.GetCollection<Image>("images");
    }

    /// <summary>
    /// Metodo para inicializar la base de datos, y insertar invetario de prueba.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeDatabase()
    {
        try
        {
            var collectionNames = await _database.ListCollectionNamesAsync();
            var collectionsList = await collectionNames.ToListAsync();

            // Verificar y crear la colección "smartphones"
            var smartphoneCollection = GetSmartphoneCollection();
            if (!collectionsList.Contains("smartphones"))
            {
                Console.WriteLine("La colección 'smartphones' no existe. Creándola...");
                await InsertInitialSmartphones(smartphoneCollection);
            }
            else
            {
                long smartphoneCount = await smartphoneCollection.CountDocumentsAsync(Builders<Smartphone>.Filter.Empty);
                if (smartphoneCount == 0)
                {
                    Console.WriteLine("La colección 'smartphones' está vacía. Insertando datos iniciales...");
                    await InsertInitialSmartphones(smartphoneCollection);
                }
            }

            // Verificar y crear la colección "images"
            var imageCollection = GetImageCollection();
            if (!collectionsList.Contains("images"))
            {
                Console.WriteLine("La colección 'images' no existe. Creándola...");
                await InsertInitialImages(imageCollection, smartphoneCollection);
            }
            else
            {
                long imageCount = await imageCollection.CountDocumentsAsync(Builders<Image>.Filter.Empty);
                if (imageCount == 0)
                {
                    Console.WriteLine("La colección 'images' está vacía. Insertando datos iniciales...");
                    await InsertInitialImages(imageCollection, smartphoneCollection);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error durante la inicialización de la base de datos: {ex.Message}");
        }
    }

    /// <summary>
    /// Si no existe la coleccion en la base de datos, crea los datos iniciales, para pruebas
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    private async Task InsertInitialSmartphones(IMongoCollection<Smartphone> collection)
    {
        var initialData = new[]
        {
            new Smartphone
            {
                Brand = "Samsung",
                Model = "Galaxy S23",
                Ram = 8,
                Rom = 128,
                ScreenSize = 6.1,
                Battery = 3900,
                Price = 799.99,
                Color = "Phantom Black",
                CameraPixels = 50,
                Network = "5G",
                Availability = "Sí",
                Img = "https://fdn2.gsmarena.com/vv/pics/samsung/samsung-galaxy-s23-5g-1.jpg"
            },
            new Smartphone
            {
                Brand = "Apple",
                Model = "iPhone 14 Pro",
                Ram = 6,
                Rom = 256,
                ScreenSize = 6.1,
                Battery = 3200,
                Price = 999.99,
                Color = "Deep Purple",
                CameraPixels = 48,
                Network = "5G",
                Availability = "Sí",
                Img = "https://fdn2.gsmarena.com/vv/pics/apple/apple-iphone-14-pro-3.jpg"
            }
        };

        await collection.InsertManyAsync(initialData);
        Console.WriteLine("Datos iniciales insertados en 'smartphones'.");
    }

    /// <summary>
    /// Insertar la primera imagen
    /// </summary>
    /// <param name="imageCollection"></param>
    /// <param name="smartphoneCollection"></param>
    /// <returns></returns>
    private async Task InsertInitialImages(IMongoCollection<Image> imageCollection, IMongoCollection<Smartphone> smartphoneCollection)
    {
        var smartphones = await smartphoneCollection.Find(Builders<Smartphone>.Filter.Empty).ToListAsync();

        var initialData = new List<Image>();

        foreach (var smartphone in smartphones)
        {
            if (smartphone.Model == "Galaxy S23")
            {
                initialData.Add(new Image
                {
                    IdSmartphone = smartphone.Id.ToString(),
                    ImgPath = "https://fdn2.gsmarena.com/vv/pics/samsung/samsung-galaxy-s23-5g-1.jpg"
                });
            }
            else if (smartphone.Model == "iPhone 14 Pro")
            {
                initialData.Add(new Image
                {
                    IdSmartphone = smartphone.Id.ToString(),
                    ImgPath = "https://fdn2.gsmarena.com/vv/pics/apple/apple-iphone-14-pro-3.jpg"
                });
            }
        }

        if (initialData.Count > 0)
        {
            await imageCollection.InsertManyAsync(initialData);
            Console.WriteLine("Datos iniciales insertados en 'images'.");
        }
    }

    /// <summary>
    /// Metodo para actualziar los datos de un telefono existente
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedData"></param>
    /// <returns></returns>
    public async Task UpdateSmartphone(string id, Smartphone updatedData)
    {
        try
        {
            var collection = GetSmartphoneCollection();
            var objectId = new ObjectId(id);
            var filter = Builders<Smartphone>.Filter.Eq(s => s.Id, id);
            //var filter = Builders<Smartphone>.Filter.Eq(s => s.Id, objectId);

            var update = Builders<Smartphone>.Update
                .Set(s => s.Brand, updatedData.Brand)
                .Set(s => s.Model, updatedData.Model)
                .Set(s => s.Ram, updatedData.Ram)
                .Set(s => s.Rom, updatedData.Rom)
                .Set(s => s.ScreenSize, updatedData.ScreenSize)
                .Set(s => s.Battery, updatedData.Battery)
                .Set(s => s.Price, updatedData.Price)
                .Set(s => s.Color, updatedData.Color)
                .Set(s => s.CameraPixels, updatedData.CameraPixels)
                .Set(s => s.Network, updatedData.Network)
                .Set(s => s.Availability, updatedData.Availability)
                .Set(s => s.Img, updatedData.Img);

            var result = await collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount > 0)
            {
                Console.WriteLine("Smartphone actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("No se encontró el smartphone con el ID proporcionado.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar el smartphone: {ex.Message}");
        }
    }

    /// <summary>
    /// Método para insertar un nuevo telefono
    /// </summary>
    /// <param name="smartphone"></param>
    /// <returns></returns>
    public async Task InsertSmartphone(Smartphone smartphone)
    {
        var collection = GetSmartphoneCollection();
        await collection.InsertOneAsync(smartphone);
    }

    /// <summary>
    /// Método para consultar todos los telefono
    /// </summary>
    /// <returns></returns>
    public async Task<List<Smartphone>> GetAllSmartphones()
    {
        var collection = GetSmartphoneCollection();
        return await collection.Find(_ => true).ToListAsync();
    }
}
