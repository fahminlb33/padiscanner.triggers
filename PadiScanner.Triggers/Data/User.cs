using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PadiScanner.Triggers.Data;

public class User
{
    [Key]
    public Ulid Id { get; set; }

    [MaxLength(100)]
    public string FullName { get; set; }
    [MaxLength(100)]
    public string Username { get; set; }
    [MaxLength(60)]
    public string Password { get; set; }
    public UserRole Role { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public List<PredictionHistory> Predictions { get; set; } = new();
}
