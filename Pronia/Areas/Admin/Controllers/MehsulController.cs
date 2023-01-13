using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilies.Extensions;
using Pronia.ViewModels.Mehsul;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MehsulController : Controller
    {

        readonly AppDbContext _context;
        readonly IWebHostEnvironment _env;

        public MehsulController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Mehsuls.Include(p => p.MehsulColors).ThenInclude(pc => pc.Color).Include(p => p.MehsulSize).ThenInclude(ps => ps.Size).Include(p => p.MehsulImage));
        }
        public IActionResult Create()
        {
            ViewBag.Colors = new SelectList(_context.Colors, "Id", "Name");
            ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateMehsulVM cp)
        {
            var coverImg = cp.CoverImage;
            var hoverImg = cp.HoverImage;
            var otherImgs = cp.OtherImages ?? new List<IFormFile>();
            string result = coverImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("CoverImage", result);
            }
            result = hoverImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("HoverImage", result);
            }
            foreach (IFormFile image in otherImgs)
            {
                result = image.CheckValidate("image/", 300);
                if (result?.Length > 0)
                {
                    ModelState.AddModelError("OtherImages", result);
                }
            }
            foreach (int colorId in (cp.ColorIds ?? new List<int>()))
            {
                if (!_context.Colors.Any(c => c.Id == colorId))
                {
                    ModelState.AddModelError("ColorIds", "Get tullan");
                    break;
                }
            }
            foreach (int sizeId in cp.SizeIds)
            {
                if (!_context.Sizes.Any(s => s.Id == sizeId))
                {
                    ModelState.AddModelError("SizeIds", "Yeniden daxil edin x2");
                    break;
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Colors = new SelectList(_context.Colors, "Id", "Name");
                ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
                return View();
            }
            var sizes = _context.Sizes.Where(s => cp.SizeIds.Contains(s.Id));
            var colors = _context.Colors.Where(c => cp.ColorIds.Contains(c.Id));
            Mehsul mehsul = new Mehsul
            {
                Name = cp.Name,
                CostPrice = cp.CostPrice,
                SellPrice = cp.SellPrice,
                Description = cp.Description,
                Discount = cp.Discount,
                IsDeleted = false,
                SKU = "1"
            };
            List<MehsulImage> images = new List<MehsulImage>();
            images.Add(new MehsulImage { ImageUrl = coverImg?.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = true, Mehsul = mehsul });
            if (hoverImg != null)
            {
                images.Add(new MehsulImage { ImageUrl = hoverImg.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = false, Mehsul = mehsul });
            }
            foreach (var item in otherImgs)
            {
                images.Add(new MehsulImage { ImageUrl = item?.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = null, Mehsul = mehsul });
            }
            mehsul.MehsulImage = images;
            _context.Mehsuls.Add(mehsul);
            foreach (var item in colors)
            {
                _context.MehsulColor.Add(new MehsulColor { Mehsul = mehsul, ProductId = item.Id });
            }
            foreach (var item in sizes)
            {
                _context.MehsulSize.Add(new MehsulSize { Mehsul = mehsul, SizeId = item.Id });
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        //public IActionResult UpdateMehsul(int? id)
        //{
        //    if (id == null) return BadRequest();
        //    Mehsul product = _context.Mehsuls.Include(p => p.MehsulColor).Include(p => p.ProductSizes).FirstOrDefault(p => p.Id == id);
        //    if (product == null) return NotFound();
        //    UpdateProductVM updateProduct = new UpdateProductVM
        //    {
        //        Id = product.Id,
        //        Name = product.Name,
        //        Description = product.Description,
        //        Discount = product.Discount,
        //        SellPrice = product.SellPrice,
        //        CostPrice = product.CostPrice,
        //        ColorIds = new List<int>(),
        //        SizeIds = new List<int>(),
        //    };
        //    foreach (ProductColor color in product.ProductColors)
        //    {
        //        updateProduct.ColorIds.Add(color.ColorId);
        //    }
        //    foreach (var size in product.ProductSizes)
        //    {
        //        updateProduct.SizeIds.Add(size.SizeId);
        //    }
        //    ViewBag.Colors = new SelectList(_context.Colors, "Id", "Name");
        //    ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
        //    return View(updateProduct);
        //}
        //[HttpPost]
        //public IActionResult UpdateProduct(int? id, UpdateProductVM updateProduct)
        //{
        //    if (id == null) return NotFound();
        //    foreach (int colorId in (updateProduct.ColorIds ?? new List<int>()))
        //    {
        //        if (!_context.Colors.Any(c => c.Id == colorId))
        //        {
        //            ModelState.AddModelError("ColorIds", "Get tullan");
        //            break;
        //        }
        //    }
        //    foreach (int sizeId in (updateProduct.SizeIds ?? new List<int>()))
        //    {
        //        if (!_context.Sizes.Any(s => s.Id == sizeId))
        //        {
        //            ModelState.AddModelError("SizeIds", "Get tullan x2");
        //            break;
        //        }
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Colors = new SelectList(_context.Colors, "Id", "Name");
        //        ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
        //        return View();
        //    }
        //    var prod = _context.Products.Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefault(p => p.Id == id);
        //    if (prod == null) return NotFound();
        //    foreach (var item in prod.ProductColors)
        //    {
        //        if (updateProduct.ColorIds.Contains(item.ColorId))
        //        {
        //            updateProduct.ColorIds.Remove(item.ColorId);
        //        }
        //        else
        //        {
        //            _context.ProductColors.Remove(item);
        //        }
        //    }
        //    foreach (var colorId in updateProduct.ColorIds)
        //    {
        //        _context.ProductColors.Add(new ProductColor { Product = prod, ColorId = colorId });
        //    }
        //    prod.Name = updateProduct.Name;
        //    prod.Discount = updateProduct.Discount;
        //    prod.CostPrice = updateProduct.CostPrice;
        //    prod.SellPrice = updateProduct.SellPrice;
        //    prod.Description = updateProduct.Description;
        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}
        //public IActionResult UpdateProductImage(int? id)
        //{
        //    if (id == null) return BadRequest();
        //    var prod = _context.Products.Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);
        //    if (prod == null) return NotFound();
        //    UpdateProductImageVM updateProductImage = new UpdateProductImageVM
        //    {
        //        ProductImages = prod.ProductImages.Where(pi => pi.IsCover == null)
        //    };
        //    return View(updateProductImage);
        //}
        //public IActionResult DeleteProductImage(int? id)
        //{
        //    if (id == null) return BadRequest();
        //    var productImage = _context.ProductImages.Find(id);
        //    if (productImage == null) return NotFound();
        //    _context.ProductImages.Remove(productImage);
        //    _context.SaveChanges();
        //    return Ok();
        //}
    }

}
