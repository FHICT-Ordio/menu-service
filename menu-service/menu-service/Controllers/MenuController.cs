using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using AL;
using FL;
using DTO;
using MenuContext = DAL.MenuContext;

namespace menu_service.Controllers
{
    public class GetOptions
    {
        public enum SortType
        {
            /// <summary>
            /// Alphabetical Ascending
            /// </summary>
            ALP_ASC,
            /// <summary>
            /// Alphabetical Descending
            /// </summary>
            ALP_DES,
            /// <summary>
            /// Price Ascending
            /// </summary>
            PRICE_ASC,
            /// <summary>
            /// Price Descending
            /// </summary>
            PRICE_DES
        }

        public enum FilterType
        {
            /// <summary>
            /// Dont apply filtering
            /// </summary>
            NONE,
            /// <summary>
            /// Apply Name filtering. A String object should be provided in FilterValue as filter condition
            /// </summary>
            /// <example>
            /// "Carpacio"
            /// </example>
            NAME,
            /// <summary>
            /// Apply Regular Expression filtering. A Regex expression should be provided in FilterValue
            /// </summary>
            /// <example>
            /// "([A-Z])\w+"
            /// </example>
            NAME_REGEX,
            /// <summary>
            /// Apply price range filtering. An integer range value should be supplied in FilterValue
            /// </summary>
            /// <example>
            /// { x, x }
            /// </example>
            PRICE_RANGE
        };
        
        /// <summary>
        /// Specify the type of sorting to apply to the returned values
        /// </summary>
        public SortType Sort { get; set; } = SortType.ALP_ASC;
        public FilterType Filter { get; set; } = FilterType.NONE;
        public object? FilterValue { get; set; } = null;
    }

    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
        public class Menu
        {
            public string Title { get; set; }
            public string RestaurantName { get; set; }
        }

        public class Category
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
        }

        private readonly IMenuCollection _menuCollection;
        private readonly ICategoryCollection _categoryCollection;

        public MenuController(MenuContext context, IMenuCollection? menuCollection = null, ICategoryCollection? categoryCollection = null)
        {
            _menuCollection = menuCollection ?? IMenuCollectionFactory.Get(context);
            _categoryCollection = categoryCollection ?? ICategoryCollectionFactory.Get(context);
        }

        [HttpPut]
        public IActionResult? AddMenu(Menu item)
        {
            if (item == null)
            {
                return BadRequest("No item was supplied");
            }

            _menuCollection.Add(new DTO.Menu { Title = item.Title, RestaurantName = item.RestaurantName });
            return Ok();
        }

        [HttpPut]
        [Route("Test")]
        public IActionResult? TEST(Menu menu)
        {
            _menuCollection.Add(new DTO.Menu { Title = menu.Title, RestaurantName = menu.RestaurantName });
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult? DeleteMenu(int id)
        {
            _menuCollection.Delete(id);
            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult? GetMenu(int id)
        {
            DTO.Menu menu = _menuCollection.Get(id);
                return Ok(menu);
        }
        

        [HttpGet]
        [Route("Items/{id}")]
        public IActionResult? GetAllItems(int id, GetOptions? options)
        {
            return null;
        }

        [HttpPut]
        [Route("TestPut/{id}")]
        public IActionResult? Test(int id, Category category)
        {
            _categoryCollection.Add(new DTO.Category { DisplayName = category.DisplayName, Name = category.Name }, id);
            return Ok();
        }
    }
}