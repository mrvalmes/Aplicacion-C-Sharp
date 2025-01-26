using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Smartphone
{
	[BsonId] // Marca este campo como el ID único del documento
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; }
	public string Brand { get; set; }
	public string Model { get; set; }
	public int Ram { get; set; }
	public int Rom { get; set; }
	public double ScreenSize { get; set; }
	public int Battery { get; set; }
	public double Price { get; set; }
	public string Color { get; set; }
	public double CameraPixels { get; set; }
	public string Network { get; set; }
	public string Availability { get; set; }
    public string Img { get; set; }
}
