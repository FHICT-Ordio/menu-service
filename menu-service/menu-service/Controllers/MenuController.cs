using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

using AL;
using FL;
using DTO;
using MenuContext = DAL.MenuContext;
using System.Security.Claims;

namespace menu_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
#pragma warning disable CS8618
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
#pragma warning restore CS8618

        private readonly IMenuCollection _menuCollection;

        public MenuController(MenuContext context, IMenuCollection? menuCollection = null)
        {
            _menuCollection = menuCollection ?? IMenuCollectionFactory.Get(context);            
        }

        /// <summary>
        /// Add Menu [O]
        /// </summary>
        /// <remarks>
        /// Add a new Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menu">A menu object to be added. The description field is optional</param>
        /// <response code="200">The menu was added. The new menu's ID will be returned</response>
        /// <response code="400">The menu could not be added. More information will be given in the rensponse body</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult? AddMenu(Menu menu)
        {
            if (menu == null)
            {
                return BadRequest("No item was supplied");
            }

            int menuID = _menuCollection.Add(new MenuDTO { Title = menu.Title, RestaurantName = menu.RestaurantName, Description = menu.Description, Owner = HashManager.GetHash(GetRequestSub(Request)) });
            return Ok(menuID);
        }


        /// <summary>
        /// Get Menu
        /// </summary>
        /// <remarks>
        /// Get an existing Menu. 
        /// </remarks>
        /// <param name="menuID">The ID for the requested menu</param>
        /// <response code="200">The menu eixsts. The new menu object will be returned</response>
        /// <response code="400">The menu could not be found. More information will be given in the rensponse body</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Menu))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{menuID}")]
        public IActionResult? GetMenu(int menuID)
        {
            MenuDTO? menu = _menuCollection.Get(menuID);

            if (menu == null)
                return BadRequest("A menu with this ID could not be found");
            
            return Ok(menu);
        }

        /// <summary>
        /// Update Menu [O]
        /// </summary>
        /// <remarks>
        /// Edit an existing Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID for the menu to be edited</param>
        /// <param name="menu">The updated values to be edited. All field are optional, exluded fields will be retained</param>
        /// <response code="200">The menu was edited</response>
        /// <response code="400">The menu could not be updated. More information will be given in the response body</response>
        /// <response code="401">An authorization error occured. More information will be given in the response body</response>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("{menuID}")]
        public IActionResult? UpdateMenu(int menuID, MenuEdit menu)
        {            
            MenuDTO? menuDTO = _menuCollection.Get(menuID); 

            if (menuDTO == null)
                return BadRequest("Menu with given ID does not exist");

            string user = GetRequestSub(Request);
            if (menuDTO.Owner != user)
                return Unauthorized("A menu can only be edited by the menu owner");

            menuDTO.Title = menu.Title ?? menuDTO.Title;
            menuDTO.RestaurantName = menu.RestaurantName ?? menuDTO.RestaurantName;
            menuDTO.Description = menu.Description ?? menuDTO.Description;
            
            if (_menuCollection.Update(menuDTO))
                return Ok();
            else
                return BadRequest("An unexpected error occured");
        }


        /// <summary>
        /// Delte Menu [O]
        /// </summary>
        /// <remarks>
        /// Delete an existing Menu. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID for the menu to be deleted</param>        
        /// <response code="200">The menu was deleted</response>
        /// <response code="400">The menu could not be deleted. More information will be given in the response body</response>
        /// <response code="401">An authorization error occured. More information will be given in the response body</response>
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("{menuID}")]
        public IActionResult? DeleteMenu(int menuID)
        {
            MenuDTO? menuDTO = _menuCollection.Get(menuID);

            if (menuDTO == null)
                return BadRequest("A menu with given ID does not exist");

            string user = GetRequestSub(Request);
            if (!HashManager.CompareStringToHash(user, menuDTO.Owner))
                return Unauthorized("A menu can only be edited by the menu owner");

            bool state = _menuCollection.Delete(menuID);
            if (state)
                return Ok();
            else
                return BadRequest("An unexpected error occured");
        }

        /// <summary>
        /// Get User Menus [O]
        /// </summary>
        /// <remarks>
        /// Get all menus owner by a specific user. An authorization token should be provided through the authorization header to authorize and identify the user.
        /// </remarks>
        /// <response code="200">All user menus will be returned</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]        
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

            Claim? claim = token.Claims.FirstOrDefault(x => x.Type == "sub");

            if (claim == null)
                return "";

            return claim.Value;
        }
    }
}