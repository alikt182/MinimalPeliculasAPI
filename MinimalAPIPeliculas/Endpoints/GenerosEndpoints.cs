using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Repositorios;

namespace MinimalAPIPeliculas.Endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros ( this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));

            group.MapGet("/{id:int}", ObtenerGeneroPorId);

            group.MapPost("/", CrearGenero);

            group.MapPut("/{id:int}", ActualizarGenero);

            group.MapDelete("/{id:int}", BorrarGenero);
            return group;
        }

        //Metodos
        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
        {
            //var generos = new List<Genero>
            //{
            //    new Genero{ Id = 1, Nombre = "Drama"},
            //    new Genero{ Id = 1, Nombre = "Acción"},
            //    new Genero{ Id = 1, Nombre = "Comedia"},
            //};
            var generos = await repositorio.ObtenerTodos();
            //sin automapper
            //var generosDTO = generos.Select(x => new GeneroDTO { Id = x.Id, Nombre = x.Nombre }).ToList();
            //con autommaper
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

            return TypedResults.Ok(generosDTO);
        }
        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            //Sin automapper
            //var generoDTO = new GeneroDTO
            //{
            //    Id = id,
            //    Nombre = genero.Nombre
            //};

            //Con automapper
            var generoDTO = mapper.Map<GeneroDTO>(genero);

            return TypedResults.Ok(generoDTO);

        }
        static async Task<Created<GeneroDTO>> CrearGenero(CrearGeneroDTO creargeneroDTO, 
            IRepositorioGeneros repositorioGeneros, 
            IOutputCacheStore outputCacheStore,
            IMapper mapper
            )
        {

            //Sin automapper
            //var genero = new Genero
            //{
            //    Nombre = creargeneroDTO.Nombre
            //};

            //Con automapper
            var genero = mapper.Map<Genero>(creargeneroDTO);

            var id = await repositorioGeneros.CrearGenero(genero);

            //limpiamos el caché porque la data cambió
            await outputCacheStore.EvictByTagAsync("generos-get", default);

            //Sin automapper
            //var generoDTO = new GeneroDTO
            //{
            //    Id = id,
            //    Nombre = genero.Nombre
            //};

            //Con automapper
            var generoDTO = mapper.Map<GeneroDTO>( genero );

            return TypedResults.Created("$/generos/{id}", generoDTO);
        }
        static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, CrearGeneroDTO creargeneroDTO, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            //sin automapper
            //var genero = new Genero
            //{
            //    Id = id,
            //    Nombre = creargeneroDTO.Nombre
            //};

            //con automapper
            var genero = mapper.Map<Genero>(creargeneroDTO);
            genero.Id = id;

            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();

        }
        static async Task<Results<NoContent, NotFound>> BorrarGenero(int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();

        }
    }
}
