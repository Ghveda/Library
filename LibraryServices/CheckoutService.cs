using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LibraryServices
{
    public class CheckoutService : ICheckout

    {
        private LibraryContext _context;
        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId, int LibrariCardId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets.FirstOrDefault(i => i.Id == assetId);

            RemoveExistingCheckouts(assetId);
            CloseExistingCheckoutHistory(assetId);
            var currentHolds = _context.Holds.Include(i => i.LibraryAsset).Include(i => i.LibraryCard).Where(i => i.LibraryAsset.Id == assetId);
            if (currentHolds.Any())
            {
                CheckoutEarliestHold(assetId, currentHolds);
            }
            UpdateAssetStatus(assetId, "Available");
            _context.SaveChanges();
        }

        private void CheckoutEarliestHold(int assetId, IQueryable<Holds> currentHolds)
        {
            var earliestHold = currentHolds.OrderBy(i => i.HoldPlaced).FirstOrDefault();
            var card = earliestHold.LibraryCard;
            _context.Remove(earliestHold);
            _context.SaveChanges();
            CheckOutItem(assetId, card.Id);
        }

        public void CheckOutItem(int assetId, int LibraryCardId)
        {
            if (isCheckedOut(assetId))
            {
                return;
            }
            var item = _context.LibraryAssets.FirstOrDefault(i => i.Id == assetId);
            UpdateAssetStatus(assetId, "Checked Out");
            var libraryCard = _context.LibraryCards.Include(i => i.Checkouts).FirstOrDefault(i => i.Id == LibraryCardId);
            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(DateTime.Now)
            };
            _context.Add(checkout);
            var checkOutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };
            _context.Add(checkOutHistory);
            _context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public bool isCheckedOut(int assetId)
        {
            var isCheckedOut = _context.Checkouts.Where(i => i.LibraryAsset.Id == assetId).Any();
            return isCheckedOut;
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutid)
        {
            return GetAll().FirstOrDefault(i => i.Id == checkoutid);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories.Include(i => i.LibraryAsset).Include(i => i.LibraryCard).Where(i => i.Id == id);
        }

        public string GetCurrentHoldPatronName(int id)
        {
            var hold = _context.Holds.Include(i => i.LibraryAsset).Include(i => i.LibraryCard).FirstOrDefault(i => i.Id == id);
            var cardId = hold?.LibraryCard.Id;
            var patron = _context.Patrons.Include(i => i.LibraryCard).FirstOrDefault(i => i.LibraryCard.Id == cardId);
            return patron?.Firstname + " " + patron?.Lastname;
        }

        public DateTime GetCurrentHoldPlaced(int id)
        {
            return _context.Holds.Include(i => i.LibraryAsset).Include(i => i.LibraryCard).FirstOrDefault(i => i.Id == id).HoldPlaced;

        }

        public IEnumerable<Holds> GetCurrentHolds(int id)
        {
            return _context.Holds.Include(i => i.LibraryAsset).Where(i => i.Id == id);
        }

        public Checkout GetLatestCheckout(int assetId)
        {
            return _context.Checkouts.Where(i => i.Id == assetId).OrderByDescending(i => i.Since).FirstOrDefault();
        }

        public void MarkFound(int asetId)
        {
            UpdateAssetStatus(asetId, "Available");
            RemoveExistingCheckouts(asetId);
            CloseExistingCheckoutHistory(asetId);


            _context.SaveChanges();
        }

        public void MarkLost(int asetId)
        {
            UpdateAssetStatus(asetId, "Lost");
            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int asetId, string newStatus)
        {
            var item = _context.LibraryAssets.FirstOrDefault(i => i.Id == asetId);
            _context.Update(item);
            item.Status = _context.Statuses.FirstOrDefault(i => i.Name == newStatus);
        }

        private void CloseExistingCheckoutHistory(int asetId)
        {
            var now = DateTime.Now;
            var History = _context.CheckoutHistories.FirstOrDefault(i => i.LibraryAsset.Id == asetId && i.CheckedIn == null);
            if (History != null)
            {
                _context.Update(History);
                History.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int asetId)
        {
            var checkout = _context.Checkouts.FirstOrDefault(i => i.LibraryAsset.Id == asetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void PlaceHold(int asserId, int LibraryCardId)
        {
            var now = DateTime.Now;
            var asset = _context.LibraryAssets.FirstOrDefault(i => i.Id == asserId);
            var card = _context.LibraryCards.FirstOrDefault(i => i.Id == LibraryCardId);
            if (asset.Status.Name == "Available")
            {
                UpdateAssetStatus(asserId, "On Hold");
            }
            var hold = new Holds
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };
            _context.Add(hold);
            _context.SaveChanges();

        }

        public string GetCurrentCheckoutPatron(int id)
        {

            //var hold = _context.Holds
            //    .Include(a => a.LibraryAsset)
            //    .Include(a => a.LibraryCard)
            //    .Where(v => v.Id == id);

            //var cardId = hold
            //    .Include(a => a.LibraryCard)
            //    .Select(a => a.LibraryCard.Id)
            //    .FirstOrDefault();

            //    var patron = _context.Patrons
            //        .Include(p => p.LibraryCard)
            //        .First(p => p.LibraryCard.Id == cardId);

            //    return patron.Firstname + " " + patron.Lastname;
            return "";
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(i => i.LibraryAsset)
                .Include(i => i.LibraryCard)
                .FirstOrDefault(i => i.LibraryAsset.Id == assetId);

        }

       
    }
}
