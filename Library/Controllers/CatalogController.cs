using Library.Model.Catalog;
using Library.Model.Checkout;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{

    public class CatalogController : Controller
    {
        private ILibraryAsset _asset;
        private ICheckout _checkout;
        public CatalogController(ILibraryAsset asset, ICheckout checkout)
        {
            _asset = asset;
            _checkout = checkout;
        }
        public IActionResult Index()
        {
            var assetModels = _asset.GetAll();
            var ListingResult = assetModels.Select(i => new AssetIndexListingModel
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                Title = i.Title,
                AuthorDrDirector = _asset.GetAuthorOrDirector(i.Id),
                DeweyCallNumber = _asset.GetAuthorOrDirector(i.Id),
                Type = _asset.GetType(i.Id),
                //NumberOfCopies=i.NumberOfCopies

            });
            var model = new AssetIndexModel()
            {
                Assets = ListingResult
            };
            return View(model);
        }
        public IActionResult Detail(int id)
        {
            var asset = _asset.GetById(id);
            var currenHolds = _checkout.GetCurrentHolds(id)
                .Select(i => new AssetHoldModel
                {
                    HoldPlaced = _checkout.GetCurrentHoldPlaced(i.Id),
                    PatronName = _checkout.GetCurrentHoldPatronName(i.Id)
                });
            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _asset.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorDrDirector = _asset.GetAuthorOrDirector(id),
                CurrentLocation = _asset.GetCurrentLocation(id).Name,
                DeweyCallNumber = _asset.GetDeweyIndex(id),
                CheckoutHistories = _checkout.GetCheckoutHistory(id),
                ISBN = _asset.GetIsbn(id),
                LatestCheckout = _checkout.GetLatestCheckout(id),
                PatronName = _checkout.GetCurrentCheckoutPatron(id),
            };
            return View(model);
        }
        public IActionResult Checkout(int id)
        {
            var asset = _asset.GetById(id);
            var model = new CheckoutModel
            {

                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkout.isCheckedOut(id)

            };
            return View(model);
        }
        public IActionResult MarkLost(int assetid)
        {
            _checkout.MarkLost(assetid);
            return RedirectToAction("Detail", new { id = assetid });
        }
        public IActionResult MarkFound(int assetid)
        {
            _checkout.MarkFound(assetid);
            return RedirectToAction("Detail", new { id = assetid });
        }
        public IActionResult Hold(int id)
        {
            var asset = _asset.GetById(id);
            var model = new CheckoutModel
            {

                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkout.isCheckedOut(id),
                HoldCount = _checkout.GetCurrentHolds(id).Count()

            };
            return View(model);
        }
        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int LibraryCardId)
        {
           
            _checkout.CheckOutItem(assetId, LibraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
        [HttpPost]
        public IActionResult PlaceHold(int assetId, int librariCardId)
        {
            _checkout.PlaceHold(assetId, librariCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}




//var asset = _asset.GetById(id);
//var model = new AssetDetailModel
//{
//    AssetId = id,
//    Title = asset.Title,
//    Year = asset.Year,
//    Cost = asset.Cost,
//    Status = asset.Status.Name,
//    ImageUrl = asset.ImageUrl,
//    AuthorDrDirector = _asset.GetAuthorOrDirector(id),
//    CurrentLocation = _asset.GetCurrentLocation(id).Name,
//    DeweyCallNumber = _asset.GetDeweyIndex(id),
//    ISBN = _asset.GetIsbn(id),

//};
//            return View(model);