namespace MinimalAPIPeliculas.Entidades
{
    public class GeneroPelicula
    {
        public int GeneroId { get; set; }
        public int PeliculaId{ get; set; }
        public Genero Genero { get; set; } = null!;
        public Pelicula Pelicula { get; set; } = null!;
    }
}
