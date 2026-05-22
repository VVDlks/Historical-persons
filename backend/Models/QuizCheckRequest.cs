using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace backend.Models;

public class QuizCheckRequest
{
    public int CorrectObjectId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}