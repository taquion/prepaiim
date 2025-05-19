using Microsoft.AspNetCore.Mvc;

namespace PreparatoriaIIM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(T result, string notFoundMessage = null)
        {
            if (result == null) 
                return NotFound(new { Message = notFoundMessage ?? "Recurso no encontrado" });
                
            return Ok(result);
        }

        protected IActionResult HandlePagedResult<T>(IEnumerable<T> items, int pageNumber, int pageSize, int totalItems)
        {
            if (items == null || !items.Any())
                return Ok(new PagedResult<T>
                {
                    Items = Array.Empty<T>(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = 0,
                    TotalPages = 0
                });

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            return Ok(new PagedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            });
        }
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
