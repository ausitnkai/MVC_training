// 建立控制器檔案時，命名時後面一定要加上Controller這樣程式才能識別這個檔案式控制器
using Azure.Core;
using Kaiweb.Models;
using Kaiweb.Models.ViewModels;
using KaiWeb.DataAccess.Data;
using KaiWeb.DataAccess.Repository.IRepository;
using KaiWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DiaSymReader;

// 這邊所使用的串接資料庫方式是 Entity Framework
namespace KaiWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    // 限制這個頁面只能是有 Admin 身分的使用者才能看到
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        // 這邊我們使用的是自製的 interface
        private readonly IProductsRepository _ProductsRepo;
        private readonly ICategoryRepository _categoryRepo;

        // ctor+tab 會自動產出控制器函式
        public ProductsController(IProductsRepository db, ICategoryRepository categoryRepo, IWebHostEnvironment webHostEnvironment)
        {
            _ProductsRepo = db;
            _categoryRepo = categoryRepo;
            _webHostEnvironment = webHostEnvironment;

		}
        public IActionResult Index()
        {
            // 會將 Products 資料庫的資料存放到一個名為 objProductsList 的清單
            List<Products> objProductsList = _ProductsRepo.GetAll(includeProperties: "myProperty").ToList();
            
            // 會將資料傳遞給 Products 中的 Index 檔做使用
            return View(objProductsList);
        }
		// 這邊的 Upsert 是用於顯示畫面用的
		public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _categoryRepo.GetAll().ToList().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
                Products = new Products()
            };
            if (id == null || id == 0) 
            {
                // create
                return View(productVM);
            }
            else
            {
                // update
                productVM.Products = _ProductsRepo.Get(u => u.Id == id);
                return View(productVM);

			}
        }
		//這邊的 Upsert 是實際用來建立新的資料以及編輯資料
		[HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // 將使用者所上傳的圖片存放到我們設定的資料夾中
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product"); 
                    // 為了讓使用者可以更新圖片，我們需要先判斷資料欄位是否為空
                    if(!string.IsNullOrEmpty(productVM.Products.ImageUrl))
                    {
                        // 刪除舊的圖片
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Products.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using(var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    // 將圖片路徑存放到我們資料庫中的 ImageUrl 欄位中
                    productVM.Products.ImageUrl = @"\images\product\" + fileName;
                }
                if(productVM.Products.Id == 0)
                {
					_ProductsRepo.Add(productVM.Products);
				}
                else
                {
					_ProductsRepo.Update(productVM.Products);
				}
                
                _ProductsRepo.Save();
                TempData["success"] = "資料創建成功!!";
                // 當添加完資料後會將頁面重新導向回主頁面
                return RedirectToAction("Index");
            }
            else
            {
				productVM.CategoryList = _categoryRepo.GetAll().ToList().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});
				// 如果添加資料失敗就停留在頁面顯示錯誤訊息
				return View(productVM);
			}
        }
       
        // 將資料轉換成JSON格式，方便後續套用版面
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Products> objProductsList = _ProductsRepo.GetAll(includeProperties: "myProperty").ToList();
            return Json(new { data = objProductsList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = _ProductsRepo.Get(u => u.Id == id);
            if (productToDelete == null)
            {
                return Json(new {success = false, message = "failed to delete" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _ProductsRepo.Delete(productToDelete);
            _ProductsRepo.Save();

            return Json(new { success = true, message = "資料刪除成功!!" });
        }
        #endregion
    }
}
