using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
        {
            var entities = await context.Set<TEntity>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entities);
            return dtos;

        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDTO paginationDTO) where TEntity : class
        {
            var queryable = context.Set<TEntity>().AsNoTracking().AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            var entities = await queryable.Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<TDTO>>(entities);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id) where TEntity : class, IId
        {
            var queryable = context.Set<TEntity>().AsQueryable();
            return await Get<TEntity, TDTO>(id, queryable);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id, IQueryable<TEntity> queryable) where TEntity : class, IId
        {
            var entity = await queryable.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) { return NotFound(); }

            return mapper.Map<TDTO>(entity);
        }

        protected async Task<ActionResult> Post<TCreation, TEntity, TRead>(TCreation creation,
                                                                           string routeName,
                                                                           IFileStorageService fileStorageService,
                                                                           string container)
        where TCreation: class, IPictureFormFile, IPictureCustom
        where TEntity: class, IId, IPicture
        {
            var entity = mapper.Map<TEntity>(creation);
            await SetPicture<TCreation, TEntity>(creation, entity, fileStorageService, container);

            return await Post<TCreation, TEntity, TRead>(creation, routeName);
        }

        protected async Task<ActionResult> Post<TCreation, TEntity, TRead>(TCreation creation, string routeName) 
            where TEntity : class, IId
        {
            var entity = mapper.Map<TEntity>(creation);

            if (entity is Movie) {  AnnotateMovieOrder(entity as Movie); }

            context.Add(entity);
            await context.SaveChangesAsync();
            var readDTO = mapper.Map<TRead>(entity);

            return new CreatedAtRouteResult(routeName, new { entity.Id }, readDTO);
        }

        protected async Task<ActionResult> Put<TCreation, TEntity>(int id, TCreation creation) where TEntity : class, IId
        {
            var entity = mapper.Map<TEntity>(creation);
            entity.Id = id;
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntity>(int id,
                                                        IFileStorageService fileStorageService,
                                                        string container) where TEntity : class, IId, new()
        {
            var tEntity = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            var picureEntity = tEntity as IPicture;
            await fileStorageService.DeleteFile(fileRoute: picureEntity.Picture, containerName: container);
            return await Delete<TEntity>(id);
        }

        protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId, new()
        {
            var exists = await context.Set<TEntity>().AnyAsync(x => x.Id == id);
            if (!exists) { return NotFound(); }

            context.Remove(new TEntity() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntity, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument) where TDTO : class
            where TEntity : class, IId
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDB = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = mapper.Map<TDTO>(entityFromDB);

            patchDocument.ApplyTo(entityDTO, ModelState);

            var isValid = TryValidateModel(entityDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entityDTO, entityFromDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

        private async Task SetPicture<TDTO, TEntity>(TDTO dto, TEntity entity, IFileStorageService fileStorageService, string container)
            where TDTO: class, IPictureFormFile, IPictureCustom
            where TEntity: class, IPicture
        {
            if (dto.Picture != null && fileStorageService != null)
            {
                using var memoryStream = new MemoryStream();
                dto.Picture.CopyTo(memoryStream);
                byte[] content = memoryStream.ToArray();
                string extension = Path.GetExtension(path: dto.Picture.FileName);
                if (dto.GetType().Name.ToLower().Contains("creation"))
                {
                    dto.PictureCustom =  await fileStorageService.SaveFile(content: content,
                                                                       extension: extension,
                                                                       containerName: container,
                                                                       contentType: dto.Picture.ContentType);
                }
                // else
                // {
                //     entity.Picture = await fileStorageService.EditFile(content: content,
                //                                                       extension: extension,
                //                                                       containerName: container,
                //                                                       fileRoute: entity.Picture,
                //                                                       contentType: dto.Picture.ContentType);
                // }
            }
        }

        
        private void AnnotateMovieOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }
    }
}
