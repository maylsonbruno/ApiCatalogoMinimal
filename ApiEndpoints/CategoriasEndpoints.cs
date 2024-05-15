﻿using ApiCatalogoMinimal.Context;
using ApiCatalogoMinimal.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoMinimal.ApiEndpoints
{
    public  static class CategoriasEndpoints
    {
        public static void MapCategoriasEndpoint(this WebApplication app)
        {
            // definir os endpoints Categoria

           // app.MapGet("/", () => "Catalogo de Produtos - 2022").ExcludeFromDescription();

            app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias
            .ToListAsync()).WithTags("Categorias").RequireAuthorization();


            app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db)
                =>
            {
                return await db.Categorias.FindAsync(id)
                is Categoria categoria
                ? Results.Ok(categoria)
                : Results.NotFound();
            });


            app.MapPost("/categorias", async (Categoria categoria, AppDbContext db)
             => {
                 db.Categorias.Add(categoria);
                 await db.SaveChangesAsync();

                 return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);


             });

            app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db)
                =>
            {
                if (categoria.CategoriaId != id)
                {
                    return Results.BadRequest();
                }

                var categoriaDB = await db.Categorias.FindAsync(id);
                if (categoriaDB is null) return Results.NotFound();

                categoriaDB.Nome = categoria.Nome;
                categoriaDB.Descricao = categoria.Descricao;

                await db.SaveChangesAsync();

                return Results.Ok(categoriaDB);
            });

            app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
            {
                var categoria = await db.Categorias.FindAsync(id);

                if (categoria is null)
                {
                    return Results.NotFound();
                }

                db.Categorias.Remove(categoria);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
