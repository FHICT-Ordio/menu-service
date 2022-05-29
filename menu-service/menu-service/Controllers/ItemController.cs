using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AL;
using FL;
using DTO;
using MenuContext = DAL.MenuContext;


namespace menu_service.Controllers
{
    [ApiController]
    [Route("Menu/{menuID}/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemCollection _itemCollection;
        private readonly ICategoryCollection _categoryCollection;

        public ItemController(MenuContext context, IItemCollection? itemCollection = null, ICategoryCollection? categoryCollection = null)
        {
            _itemCollection = _itemCollection ?? IItemCollectionFactory.Get(context);
            _categoryCollection = categoryCollection ?? ICategoryCollectionFactory.Get(context);
        }

        /// <summary>
        /// Add Item [A]
        /// </summary>
        /// <remarks>
        /// Add a new Item to a menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID of the Menu for which to add a Category</param>
        /// <param name="item">An Item object. The description, tags and categories are optional fields</param>
        /// <response code="200">The Item was added. The new Items's ID will be returned</response>
        /// <response code="400">The menu could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">The authorization token was invalid or not provided</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult? AddMenuItem(int menuID, Item item)
        {

            List<CategoryDTO> _categories = new();
            if (item.Categories != null)
            {                
                foreach (int categoryID in item.Categories)
                {
                    if (_categoryCollection.Get(menuID, categoryID) != null)
                        _categories.Add(new CategoryDTO() { ID = categoryID });
                }
            }


            int id = _itemCollection.Add(menuID, new ItemDTO { Name = item.Name, Description = item.Description ?? "", Price = item.Price, Tags = item.Tags ?? new(), Categories = _categories ?? new() });
            return Ok(id);
        }

        /// <summary>
        /// Get Item [A]
        /// </summary>
        /// <remarks>
        /// Get an Item from a specific Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID of the Menu the Category belongs to</param>
        /// <param name="itemID">The ID of the Item to be retreived</param>
        /// <response code="200">The Item was found and an will be returned as JSON object</response>
        /// <response code="400">The menu could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">The authorization token was invalid or not provided</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("{itemID}")]
        public IActionResult? GetMenuItem(int menuID, int itemID)
        {
            ItemDTO? item = _itemCollection.Get(menuID, itemID);

            if (item == null)
                return BadRequest("The menu or item could not be found");

            return Ok(item);
        }

        /// <summary>
        /// Update Item [A]
        /// </summary>
        /// <remarks>
        /// Update an Item from a specific Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID of the Menu the Item belongs to</param>
        /// <param name="itemID">The ID of the Item to be updated</param>
        /// <param name="updateItem">An object containing the updated values for the Item. Fields that are left out will not be updated</param>
        /// <response code="200">The Item was updated</response>
        /// <response code="400">The menu or Item could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">The authorization token was invalid or not provided</response>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("{itemID}")]
        public IActionResult? UpdateMenuItem(int menuID, int itemID, ItemEdit updateItem)
        {
            ItemDTO? item = _itemCollection.Get(menuID, itemID);

            if (item == null)
                return BadRequest("The menu or item could not be found");

            List<CategoryDTO> _categories = null;
            if(updateItem.Categories != null)
            {
                _categories = new();
                foreach (int categoryID in updateItem.Categories)
                {
                    if (_categoryCollection.Get(menuID, categoryID) != null)
                        _categories.Add(new CategoryDTO() { ID = categoryID });
                }
            }

            item.Name = updateItem.Name ?? item.Name;
            item.Description = updateItem.Description ?? item.Description;
            item.Price = updateItem.Price ?? item.Price;
            item.Tags = updateItem.Tags ?? item.Tags;
            item.Categories = _categories ?? item.Categories;

            _itemCollection.Update(menuID, item);
            return Ok();
        }

        /// <summary>
        /// Delete Item [A]
        /// </summary>
        /// <remarks>
        /// Delete an Item from a specific Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID of the Menu the Item belongs to</param>
        /// <param name="itemID">The ID of the Item to be deleted</param>
        /// <response code="200">The Item was deleted</response>
        /// <response code="400">The Menu or Item could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">The authorization token was invalid or not provided</response>
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("{itemID}")]
        public IActionResult? DeleteItem(int menuID, int itemID)
        {
            _itemCollection.Archive(menuID, itemID);
            return Ok();
        }
    }

#pragma warning disable CS8618
    public class Item
    {
        public string Name { get; set; }
        public string? Description { get; set; } = null;
        public float Price { get; set; }

        public List<string>? Tags { get; set; } = null;
        public List<int>? Categories { get; set; } = null;
    }

    public class ItemEdit
    {
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public float? Price { get; set; } = null;
        public List<string>? Tags { get; set; } = null;
        public List<int>? Categories { get; set; } = null;
    }
#pragma warning restore CS8618
}
