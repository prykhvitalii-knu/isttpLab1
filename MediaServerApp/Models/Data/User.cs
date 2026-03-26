using System;
using System.Collections.Generic;

namespace MediaServerApp.Models.Data;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<SavedItem> SavedItems { get; set; } = new List<SavedItem>();

    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
}
