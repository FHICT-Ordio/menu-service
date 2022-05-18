using Microsoft.AspNetCore.Mvc;

using AL;
using FL;
using DTO;
using MenuContext = DAL.MenuContext;

namespace menu_service.Controllers
{
    [ApiController]
    [Route("Menu/{menuID}/[controller]")]
    public class CategoryController : ControllerBase
    {
#pragma warning disable CS8618
        public class Category
        {
            public string Name { get; set; }
            public string? Description { get; set; } = null;
        }

        public class CategoryEdit
        {
            public string? Name { get; set; } = null;
            public string? Description { get; set; } = null;
        }
#pragma warning restore CS8618

        private readonly ICategoryCollection _categoryCollection;
        
        public CategoryController(MenuContext context, ICategoryCollection? categoryCollection = null)
        {            
            _categoryCollection = categoryCollection ?? ICategoryCollectionFactory.Get(context);
        }        

        [HttpPost]
        public IActionResult? AddCategory(int menuID, Category category)
        {
            _categoryCollection.Add(menuID, new DTO.CategoryDTO { Name = category.Name, Description = category.Description ?? "" });
            return Ok();
        }

        [HttpGet]
        [Route("{categoryID}")]
        public IActionResult? GetCategory(int menuID, int categoryID)
        {
            CategoryDTO? category = _categoryCollection.Get(menuID, categoryID);

            if (category == null)
                return BadRequest("The category or menu could not be found");

            return Ok(category);
        }

        [HttpPut]
        [Route("{categoryID}")]
        public IActionResult? UpdateCategory(int menuID, int categoryID, CategoryEdit updateCategory)
        {
            CategoryDTO? category = _categoryCollection.Get(menuID, categoryID);
            
            if (category == null)
                return BadRequest("The category of menu could not be found");

            category.Name = updateCategory.Name ?? category.Name;
            category.Description = updateCategory.Description ?? category.Description;

            _categoryCollection.Update(menuID, category);
            return Ok();
        }

        [HttpDelete]
        [Route("{categoryID}")]
        public IActionResult? DeleteCategory(int menuID, int categoryID)
        {
            _categoryCollection.Delete(menuID, categoryID);
            return Ok();
        }
    }
}
