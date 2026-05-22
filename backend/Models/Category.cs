using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace backend.Models;

public class Category
{
    public int Id { get; set; }
    public string ObjectCategory { get; set; } = string.Empty;
}