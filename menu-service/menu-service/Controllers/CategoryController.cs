using Microsoft.AspNetCore.Mvc;

using AL;
using FL;
using DTO;
using MenuContext = DAL.MenuContext;
using Microsoft.AspNetCore.Authorization;

namespace menu_service.Controllers
{
    [ApiController]
    [Route("Menu/{menuID}/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryCollection _categoryCollection;
        private readonly IMenuCollection _menuCollection;
        
        public CategoryController(MenuContext context, IMenuCollection? menuCollection = null, ICategoryCollection? categoryCollection = null)
        {            
            _categoryCollection = categoryCollection ?? ICategoryCollectionFactory.Get(context);
            _menuCollection = menuCollection ?? IMenuCollectionFactory.Get(context);
        }


        /// <summary>
        /// Add Category [A]
        /// </summary>
        /// <remarks>
        /// Add a new Category to a menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID of the Menu for which to add a Category</param>
        /// <param name="category">A Category object. The description is an optional field</param>
        /// <response code="200">The category was added. The new Categorie's ID will be returned</response>
        /// <response code="400">The menu could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">The authorization token was invalid or not provided</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult? AddCategory(int menuID, Category category)
        {
            MenuDTO? menuDTO = _menuCollection.Get(menuID);
            if (menuDTO == null)
                return BadRequest("A menu with the given ID could not be found");

            int categoryID = _categoryCollection.Add(menuID, new DTO.CategoryDTO { Name = category.Name, Description = category.Description ?? "" });
            return Ok(categoryID);
        }



        /// <summary>
        /// Get Category [A]
        /// </summary>
        /// <remarks>
        /// Get a Category from a specific Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID of the Menu the Category belongs to</param>
        /// <param name="categoryID">The ID of the Category to be retreived</param>
        /// <response code="200">The category was found and will be returned as JSON object</response>
        /// <response code="400">The menu could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">The authorization token was invalid or not provided</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("{categoryID}")]
        public IActionResult? GetCategory(int menuID, int categoryID)
        {
            MenuDTO? menuDTO = _menuCollection.Get(menuID);
            if (menuDTO == null)
                return BadRequest("A menu with the given ID could not be found");

            CategoryDTO? category = _categoryCollection.Get(menuID, categoryID);
            if (category == null)
                return BadRequest("A category with the given ID could not be found");

            string user = AuthorizationHelper.GetRequestSub(Request);
            if (!HashManager.CompareStringToHash(user, menuDTO.Owner))
                return Unauthorized("A menu can only be fetched by the menu owner through this endpoint. If you want to get the menu as external user, use the public endpoint /Public/Menu");

            return Ok(category);
        }



        /// <summary>
        /// Update Category [A]
        /// </summary>
        /// <remarks>
        /// Update a Category from a specific Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID of the Menu the Category belongs to</param>
        /// <param name="categoryID">The ID of the Category to be updated</param>
        /// <param name="updateCategory">An object containing the updated values for the category. Fields that are left out will not be updated</param>
        /// <response code="200">The category was updated</response>
        /// <response code="400">The menu or category could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">The authorization token was invalid or not provided</response>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("{categoryID}")]
        public IActionResult? UpdateCategory(int menuID, int categoryID, CategoryEdit updateCategory)
        {
            MenuDTO? menuDTO = _menuCollection.Get(menuID);
            if (menuDTO == null)
                return BadRequest("A menu with the given ID could not be found");

            CategoryDTO? category = _categoryCollection.Get(menuID, categoryID);
            if (category == null)
                return BadRequest("A category with the given ID could not be found");

            string user = AuthorizationHelper.GetRequestSub(Request);
            if (!HashManager.CompareStringToHash(user, menuDTO.Owner))
                return Unauthorized("A category can only be fetched by the menu owner through this endpoint. If you want to get the category as external user, use the public endpoint /Public/Menu");

            category.Name = updateCategory.Name ?? category.Name;
            category.Description = updateCategory.Description ?? category.Description;

            _categoryCollection.Update(menuID, category);
            return Ok();
        }



        /// <summary>
        /// Delete Category [A]
        /// </summary>
        /// <remarks>
        /// Delete a Category from a specific Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID of the Menu the Category belongs to</param>
        /// <param name="categoryID">The ID of the Category to be deleted</param>
        /// <response code="200">The category was deleted</response>
        /// <response code="400">The menu or category could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">The authorization token was invalid or not provided</response>
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("{categoryID}")]
        public IActionResult? DeleteCategory(int menuID, int categoryID)
        {
            MenuDTO? menuDTO = _menuCollection.Get(menuID);
            if (menuDTO == null)
                return BadRequest("A menu with the given ID could not be found");

            CategoryDTO? category = _categoryCollection.Get(menuID, categoryID);
            if (category == null)
                return BadRequest("A category with the given ID could not be found");

            string user = AuthorizationHelper.GetRequestSub(Request);
            if (!HashManager.CompareStringToHash(user, menuDTO.Owner))
                return Unauthorized("A category can only be fetched by the menu owner through this endpoint. If you want to get the category as external user, use the public endpoint /Public/Menu");

            _categoryCollection.Delete(menuID, categoryID);
            return Ok();
        }
    }

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
#pragma warning disable CS8618
}
