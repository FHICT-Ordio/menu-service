using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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

        private readonly IItemCollection _itemCollection;
        private readonly ICategoryCollection _categoryCollection;

        public ItemController(MenuContext context, IItemCollection? itemCollection = null, ICategoryCollection? categoryCollection = null)
        {
            _itemCollection = _itemCollection ?? IItemCollectionFactory.Get(context);
            _categoryCollection = categoryCollection ?? ICategoryCollectionFactory.Get(context);
        }

        [HttpPost]
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


        [HttpGet]
        [Route("{itemID}")]
        public IActionResult? GetMenuItem(int menuID, int itemID)
        {
            ItemDTO? item = _itemCollection.Get(menuID, itemID);

            if (item == null)
                return BadRequest("The menu or item could not be found");

            return Ok(item);
        }

        [HttpPut]
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

        [HttpDelete]
        [Route("{itemID}")]
        public IActionResult? DeleteItem(int menuID, int itemID)
        {
            _itemCollection.Delete(menuID, itemID);
            return Ok();
        }

        [HttpGet]
        [Route("All")]
        public IActionResult? GetAllItems(int menuID, GetOptions.SortType sort = GetOptions.SortType.ALP_ASC, GetOptions.FilterType filter = GetOptions.FilterType.NONE, string? filterParam1 = null, string? filterParam2 = null)
        {
            List<ItemDTO> items = _itemCollection.GetAll(menuID);

            if (items == null)
                return BadRequest("The menu could not be found");

            switch(sort)
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

            switch(filter)
            {
                case GetOptions.FilterType.PRICE_RANGE:
                    try
                    {
                        items = items.FindAll(x => x.Price >= Convert.ToInt32(filterParam1));
                        if (filterParam2 != null)
                            items = items.FindAll(x => x.Price <= Convert.ToInt32(filterParam2));
                    } catch (FormatException ex)
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

            return Ok(items);
        }        
    }
}
