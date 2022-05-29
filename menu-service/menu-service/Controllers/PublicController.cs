﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

using AL;
using FL;
using DTO;
using MenuContext = DAL.MenuContext;
using JWT.Exceptions;

namespace menu_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublicController : ControllerBase
    {

        private readonly IMenuCollection _menuCollection;
        private readonly IItemCollection _itemCollection;
        private readonly JWTManager _jwtManager;

        public PublicController(MenuContext context, IMenuCollection? menuCollection = null, IItemCollection? itemCollection = null)
        {
            _menuCollection = menuCollection ?? IMenuCollectionFactory.Get(context);
            _itemCollection = itemCollection ?? IItemCollectionFactory.Get(context);
            _jwtManager = new("IsMUbet9tSM0O2Hf7DO92eg2l8vto74S9Tk19u558w6bJ2M3j75XM5s3oKqmCWAv");
        }

        /// <summary>
        /// Generate public menu token [A]
        /// </summary>
        /// <remarks>
        /// Generate an access token that can be used to retreive non-sensitive menu data through the public endpoint. An authorization token should be provided through the authorization header to authorize and identify the menu-owner.
        /// </remarks>
        /// <param name="menuID">The ID for which to generate a token</param>
        /// <response code="200">The menu exists, a Json Web Token that can be used for identification will be returned</response>
        /// <response code="400">The menu with given ID does not exist</response>
        /// <response code="401">The provided authorization token does not have the right authority to perform this action</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("Token/{menuID}")]
        public IActionResult? GenerateMenuToken(int menuID)
        {
            MenuDTO? menuDTO = _menuCollection.Get(menuID);

            if (menuDTO == null)
                return BadRequest("A menu with given ID does not exist");

            string user = AuthorizationHelper.GetRequestSub(Request);
            if (!HashManager.CompareStringToHash(user, menuDTO.Owner))
                return Unauthorized("A menu token can only be generated by the menu owner");

            Dictionary<string, object> json = new();
            json.Add("MenuID", menuDTO.ID);
            json.Add("Owner", menuDTO.Owner);

            string jwt = _jwtManager.Create(json);
            return Ok(jwt);
        }


        /// <summary>
        /// Get Menu
        /// </summary>
        /// <remarks>
        /// Get an existing Menu using the Menus identification token. This token can be aquired through the Ordio Admin Tool
        /// </remarks>
        /// <param name="token">The Menu token associated with the menu you want to retreive</param>
        /// <response code="200">The menu eixsts. The new menu object will be returned</response>
        /// <response code="400">The menu could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">An error occured reading the token or the provided token or its signature was invalid. More information will be given in the rensponse body</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PublicMenu))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("GetMenu")]
        public IActionResult? GetMenu(string token)
        {
            try
            {
                Dictionary<string, object> json = _jwtManager.Decode(token, true);

                if (!json.ContainsKey("MenuID"))
                    throw new FormatException();

                MenuDTO? menu = _menuCollection.Get(Convert.ToInt32(json["MenuID"]));
                if (menu == null)
                    return BadRequest("A menu with this ID could not be found");

                return Ok(new PublicMenu(menu));

            } catch (TokenExpiredException)
            {
                return Unauthorized("The provided token has expired! Generate a new token using the admin tool");
            } catch (SignatureVerificationException)
            {
                return Unauthorized("The token's signature is invalid! Generate a new token using the admin tool");
            } catch (FormatException)
            {
                return Unauthorized("Something went wrong parsing the Menu token, please try again or generate a new token using the admin tool");
            }
        }

        /// <summary>
        /// Get All Menu Items
        /// </summary>
        /// <remarks>
        /// Get all of a specific Menus items using the Menus public identification token. This token can be aquired through the Ordio Admin Tool. Different kinds of filtering and sorting can be applied.
        /// </remarks>
        /// <param name="token">The Menu token associated with the menu you want to retreive</param>
        /// <param name="sort">The sort type to apply to the returned items. The default sort is alphabetically ascending</param>
        /// <param name="filter">The type of filter to apply.</param>
        /// <param name="filterParam1">The first argument to apply to the filtering. For name filtering supply a string that the items name has to contain. For Regex filtering supply a Regex string. For price range filtering supply a lower bound.</param>
        /// <param name="filterParam2">The second argument to apply to the filtering. For name and Regex filtering this field is not required. For price filtering, supply an upper bound</param>
        /// <response code="200">A list of items with the specified filtering and sorting will be returned</response>
        /// <response code="400">The menu could not be found. More information will be given in the rensponse body</response>
        /// <response code="401">An error occured reading the token or the provided token or its signature was invalid. More information will be given in the rensponse body</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublicItem>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("GetMenuItems")]
        public IActionResult? GetMenuItems(string token, GetOptions.SortType sort = GetOptions.SortType.ALP_ASC, GetOptions.FilterType filter = GetOptions.FilterType.NONE, string? filterParam1 = null, string? filterParam2 = null)
        {
            try
            {
                Dictionary<string, object> json = _jwtManager.Decode(token, true);

                if (!json.ContainsKey("MenuID"))
                    throw new FormatException();


                List<ItemDTO> items = _itemCollection.GetAll(Convert.ToInt32(json["MenuID"]));


                if (items == null)
                    return BadRequest("The menu could not be found");

                switch (sort)
                {
                    case GetOptions.SortType.ALP_ASC:
                        items = items.OrderBy(x => x.Name).ToList();
                        break;

                    case GetOptions.SortType.ALP_DES:
                        items = items.OrderBy(x => x.Name).ToList();
                        items.Reverse();
                        break;

                    case GetOptions.SortType.PRICE_ASC:
                        items = items.OrderBy(x => x.Price).ToList();
                        break;

                    case GetOptions.SortType.PRICE_DES:
                        items = items.OrderBy(x => x.Price).ToList();
                        items.Reverse();
                        break;
                }

                switch (filter)
                {
                    case GetOptions.FilterType.PRICE_RANGE:
                        try
                        {
                            items = items.FindAll(x => x.Price >= Convert.ToInt32(filterParam1));
                            if (filterParam2 != null)
                                items = items.FindAll(x => x.Price <= Convert.ToInt32(filterParam2));
                        }
                        catch (FormatException ex)
                        {
                            return BadRequest(ex.Message);
                        }
                        break;

                    case GetOptions.FilterType.NAME:
                        items = items.FindAll(x => x.Name.ToLower().Contains((filterParam1 ?? "").ToLower()));
                        break;

                    case GetOptions.FilterType.NAME_REGEX:
                        string safeRegex = Regex.Escape(filterParam1 ?? "");
                        items = items.FindAll(x => new Regex(safeRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(2000)).IsMatch(x.Name));
                        break;
                }

                List<PublicItem> _publicItems = new();
                foreach(ItemDTO item in items)
                {
                    _publicItems.Add(new PublicItem(item));
                }

                return Ok(_publicItems);

            }
            catch (TokenExpiredException)
            {
                return Unauthorized("The provided token has expired! Generate a new token using the admin tool");
            }
            catch (SignatureVerificationException)
            {
                return Unauthorized("The token's signature is invalid! Generate a new token using the admin tool");
            }
            catch (FormatException)
            {
                return Unauthorized("Something went wrong parsing the Menu token, please try again or generate a new token using the admin tool");
            }
        }
    }

    public class PublicMenu
    {
        public PublicMenu(MenuDTO menuDTO)
        {
            Title = menuDTO.Title;
            RestaurantName = menuDTO.RestaurantName;
            Description = menuDTO.Description;

            Items = new();
            foreach (ItemDTO itemDTO in menuDTO.Items)
            {
                Items.Add(new PublicItem(itemDTO));
            }

            Categories = new();
            foreach (CategoryDTO categoryDTO in menuDTO.Categories)
            {
                Categories.Add(new PublicCategory(categoryDTO));
            }
        }

        public string Title { get; set; }
        public string RestaurantName { get; set; }
        public string? Description { get; set; }


        public List<PublicItem> Items { get; set; }
        public List<PublicCategory> Categories { get; set; }
    }

    public class PublicCategory
    {
        public PublicCategory(CategoryDTO category)
        {
            Name = category.Name;
            Description = category.Description;

            Items = new();
            foreach (ItemDTO itemDTO in category.Items)
            {
                Items.Add(new PublicItem(itemDTO));
            }
        }
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<PublicItem> Items { get; set; }
    }

    public class PublicItem
    {
        public PublicItem(ItemDTO itemDTO)
        {
            Name = itemDTO.Name;
            Description = itemDTO.Description;
            Price = itemDTO.Price;
            Tags = itemDTO.Tags;

            Categories = new();
            foreach (CategoryDTO categoryDTO in itemDTO.Categories)
            {
                Categories.Add(new PublicCategory(categoryDTO));
            }
        }

        public string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public List<string> Tags { get; set; }

        public List<PublicCategory> Categories { get; set; }
    }
}