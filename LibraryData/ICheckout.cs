using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface ICheckout
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int checkoutid);
        void Add(Checkout newCheckout);
        void CheckOutItem(int assetId, int LibraryCardId);
        void CheckInItem(int assetId, int LibrariCardId);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        string GetCurrentCheckoutPatron(int assetId);

        void PlaceHold(int asserId, int LibraryCardId);
        string GetCurrentHoldPatronName(int id);
        DateTime GetCurrentHoldPlaced(int id);
        bool isCheckedOut(int id);

        Checkout GetLatestCheckout(int assetId);
        IEnumerable<Holds> GetCurrentHolds(int id);

        void MarkLost(int asetId);
        void MarkFound(int assetId);
    }
}
