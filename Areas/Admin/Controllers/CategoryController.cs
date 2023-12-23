// 建立控制器檔案時，命名時後面一定要加上Controller這樣程式才能識別這個檔案式控制器
using Azure.Core;
using Kaiweb.Models;
using KaiWeb.DataAccess.Data;
using KaiWeb.DataAccess.Repository.IRepository;
using KaiWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// 這邊所使用的串接資料庫方式是 Entity Framework
namespace KaiWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    // 限制這個頁面只能是有 Admin 身分的使用者才能看到
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        // 這邊我們使用的是自製的 interface
        private readonly ICategoryRepository _categoryRepo;
        // ctor+tab 會自動產出控制器函式
        public CategoryController(ICategoryRepository db)
        {
            _categoryRepo = db;
        }
        public IActionResult Index()
        {
            // 會將 MyProperty 資料庫的資料存放到一個名為 objCategoryList 的清單
            List<Category> objCategoryList = _categoryRepo.GetAll().ToList();
            // 會將資料傳遞給 Category 中的 Index 檔做使用
            return View(objCategoryList);
        }
        // 這邊的 Create 是用於顯示畫面用的
        public IActionResult Create()
        {
            return View();
        }
        //這邊的 Create 是實際用來建立新的資料
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Category Name 不能與 Display Order相同");
            }
            // 檢查使用者的輸入是否符合
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(obj);
                _categoryRepo.Save();
                TempData["success"] = "資料創建成功!!";
                // 當添加完資料後會將頁面重新導向回主頁面
                return RedirectToAction("Index");
            }
            // 如果添加資料失敗就停留在頁面顯示錯誤訊息
            return View();
        }
        // 這邊的 Edit 是用於顯示畫面用的
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // 從資料庫中獲取資料的三種方法
            Category? categoryFromDb = _categoryRepo.Get(u => u.Id == id);
            // Category? categoryFromDb1 = _db.MyProperty.FirstOrDefault(u=>u.Id == id);
            // Category? categoryFromDb2 = _db.MyProperty.Where(u => u.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //這邊的 Edit 是實際用來編輯資料
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Update(obj);
                _categoryRepo.Save();
                TempData["success"] = "資料更新成功!!";
                return RedirectToAction("Index");
            }
            return View();
        }
        // 這邊的 Delete 是用於顯示畫面用的
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // 從資料庫中獲取資料的三種方法
            Category? categoryFromDb = _categoryRepo.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //這邊的 Edit 是實際用來編輯資料
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _categoryRepo.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _categoryRepo.Delete(obj);
            _categoryRepo.Save();
            TempData["success"] = "資料刪除成功!!";
            return RedirectToAction("Index");
        }
    }
}
