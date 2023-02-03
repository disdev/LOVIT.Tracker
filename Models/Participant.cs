using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOVIT.Tracker.Models;

public enum Status
{
    Registered,
    DNS,
    Started,
    DNF,
    Finished
}

public enum Gender
{
    Male,
    Female
}

public class Participant
{
    public Guid Id { get; set; }
    public string Bib { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public Guid RaceId { get; set; }
    public Race? Race { get; set; }
    public Status Status { get; set; }
    public bool Linked { get; set; }
    public string PictureUrl { get; set; } = string.Empty;
    public float Rank { get; set; }
    public string UltraSignupEmail { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public Guid? LinkCode { get; set; }
    public List<Checkin> Checkins { get; set; } = new();
    public List<Watcher> Watchers { get; set; } = new();
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
    [NotMapped]
    public string Hometown => $"{City}, {Region}";

    public string BibOrId => string.IsNullOrWhiteSpace(Bib) ? Id.ToString() : Bib;

    public string ProfilePicture
    {
        get
        {
            if (String.IsNullOrEmpty(PictureUrl) || PictureUrl == "empty")
            {
                return $"https://ui-avatars.com/api/?name={FirstName?.Substring(0, 1)}+{LastName?.Substring(0, 1)}&rounded=true&size={128}";
            }
            else if (PictureUrl.Contains("graph.facebook.com"))
            {
                return $"{PictureUrl}";
            }
            else
            {
                return PictureUrl;
            }
        }
    }

    public string StatusClass
    {
        get
        {
            var result = "";

            switch (Status)
            {
                case Status.Registered:
                    result = "bg-secondary";
                    break;
                case Status.Started:
                    result = "bg-primary";
                    break;
                case Status.DNS:
                    result = "bg-warning";
                    break;
                case Status.DNF:
                    result = "bg-danger";
                    break;
                case Status.Finished:
                    result = "bg-success";
                    break;
                default:
                    result = "bg-light";
                    break;
            }

            return result;
        }
    }
}