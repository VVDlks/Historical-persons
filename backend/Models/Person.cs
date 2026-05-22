namespace backend.Models;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Fact { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Period { get; set; }
    public int CategoryId { get; set; }
}