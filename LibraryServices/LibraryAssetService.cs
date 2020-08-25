using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _context;
        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets.Include(asset => asset.Status).Include(asset => asset.Location);
        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Where(i => i.Id == id).Any();
            var isVideo = _context.LibraryAssets.OfType<Video>().Where(i => i.Id == id).Any();
            return isBook ?
                _context.Books.FirstOrDefault(i => i.Id == id).Author :
                _context.Videos.FirstOrDefault(i => i.Id == id).Director
            ?? "Unkown";

        }

        public LibraryAsset GetById(int id)
        {
            return GetAll().FirstOrDefault(asset => asset.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
           return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(asset=> asset.Id==id))
            {
                return _context.Books.FirstOrDefault(Book => Book.Id == id).DeweyIndex;
            }
            else
            {
                return "";
            }
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(asset => asset.Id == id))
            {
               return _context.Books.FirstOrDefault(Books => Books.Id == id).ISBN;
            }
            else
            {
                return "";
            }
        }

        public string GetTitle(int id)
        {
            if (_context.Books.Any(asset=>asset.Id==id))
            {
                return  _context.Books.FirstOrDefault(books => books.Id == id).Title;
            }
            else
            {
                return "";
            }
        }

        public string GetType(int id)
        {
           var books =_context.LibraryAssets.OfType<Book>().Where(i => i.Id == id).Any();
            return books ? "Book" : "Video";
        }
    }
}
