using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

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
        public Dictionary<string, string>? FilterValue { get; set; } = new();
    }

    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
        public class Menu
        {
            public string Title { get; set; }
            public string RestaurantName { get; set; }
            public string? Description { get; set; } = "";
        }

        public class MenuEdit
        {
            public string? Title { get; set; } = null;
            public string? RestaurantName { get; set; } = null;
            public string? Description { get; set; } = null;
        }

        private readonly IMenuCollection _menuCollection;

        public MenuController(MenuContext context, IMenuCollection? menuCollection = null)
        {
            _menuCollection = menuCollection ?? IMenuCollectionFactory.Get(context);            
        }

        [HttpPost]
        [Authorize]
        public IActionResult? AddMenu(Menu menu)
        {
            if (menu == null)
            {
                return BadRequest("No item was supplied");
            }

            int menuID = _menuCollection.Add(new MenuDTO { Title = menu.Title, RestaurantName = menu.RestaurantName, Description = menu.Description, Owner = HashManager.GetHash(GetRequestSub(Request)) });
            return Ok(menuID);
        }


        [HttpGet]
        [Authorize]
        [Route("{menuID}")]
        public IActionResult? GetMenu(int menuID)
        {
            MenuDTO? menu = _menuCollection.Get(menuID);

            if (menu == null)
                return BadRequest("The menu could not be found");
            
            return Ok(menu);
        }
        
        [HttpPut]
        [Route("{menuID}")]
        public IActionResult? UpdateMenu(int menuID, MenuEdit menu)
        {
            MenuDTO? menuDTO = _menuCollection.Get(menuID);

            if (menuDTO == null)
                return BadRequest("Menu with given ID does not exist");

            menuDTO.Title = menu.Title ?? menuDTO.Title;
            menuDTO.RestaurantName = menu.RestaurantName ?? menuDTO.RestaurantName;
            menuDTO.Description = menu.Description ?? menuDTO.Description;
            
            if (_menuCollection.Update(menuDTO))
                return Ok();
            else
                return BadRequest("An error occured");
        }

        [HttpDelete]
        [Route("{menuID}")]
        public IActionResult? DeleteMenu(int menuID)
        {
            _menuCollection.Delete(menuID);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public IActionResult? GetAllUserMenus()
        {
            string sub = GetRequestSub(Request);
            return Ok(_menuCollection.GetAll(HashManager.GetHash(sub)));
        }

        private string GetRequestSub(HttpRequest request)
        {
            string authHeader = request.Headers.Authorization;

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(authHeader.Split(' ')[1]);

            return token.Claims.FirstOrDefault(x => x.Type == "sub").Value;
        }
    }
}