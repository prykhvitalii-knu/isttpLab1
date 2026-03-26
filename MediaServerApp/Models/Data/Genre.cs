using System;
using System.Collections.Generic;

namespace MediaServerApp.Models.Data;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
