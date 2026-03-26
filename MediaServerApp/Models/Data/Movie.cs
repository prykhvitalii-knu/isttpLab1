using System;
using System.Collections.Generic;

namespace MediaServerApp.Models.Data;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string FilePath { get; set; } = null!;

    public int? Duration { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public virtual ICollection<SavedItem> SavedItems { get; set; } = new List<SavedItem>();

    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
