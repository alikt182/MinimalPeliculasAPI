using AutoMapper;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Entidades;

namespace MinimalAPIPeliculas.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, GeneroDTO>();

            CreateMap<CrearActorDTO, Actor>()
                //para ignorar el tipo string en Actor y IFormFile en CrearActorDTO
                .ForMember(x => x.Foto, opciones => opciones.Ignore());
            CreateMap<Actor, ActorDTO>();

            CreateMap<PeliculaDTO, Pelicula>();

            //Mapeamos desde GenerosPeliculas a GeneroDTO
            // y de ActoresPeliculas a ActoresPeliculaDTO
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember( x => x.Generos, entidad => 
                entidad.MapFrom( p =>
                p.GenerosPeliculas.Select( gp =>
                    new GeneroDTO { Id = gp.GeneroId, Nombre = gp.Genero.Nombre })))
                .ForMember( x => x.Actores, entidad =>
                entidad.MapFrom( p => 
                p.ActoresPeliculas.Select( ap => new ActorPeliculaDTO { Id = ap.ActorId,
                Nombre = ap.Actor.Nombre, Personaje = ap.Personaje})));

            CreateMap<CrearPeliculaDTO, Pelicula>()
            //para ignorar el tipo string en Actor y IFormFile en CrearActorDTO
            .ForMember(x => x.Poster, opciones => opciones.Ignore());
                    CreateMap<Pelicula, PeliculaDTO>();

            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();

        }
    }
}
