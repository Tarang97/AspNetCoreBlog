using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreBlog.Models;
using AspNetCoreBlog.Data;
using AspNetCoreBlog.Data.Repository;
using AspNetCoreBlog.Data.FileManager;
using AspNetCoreBlog.Models.Comments;
using AspNetCoreBlog.ViewModels;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace AspNetCoreBlog.Controllers
{
    public class HomeController : Controller
    {
        private IRepository _repo;
        private IFileManager _fileManager;

        public HomeController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }
        
        public IActionResult Index(int pageNumber, string category, string search)
        {
            if (pageNumber < 1)
                return RedirectToAction("Index", new { pageNumber = 1, category });
            
            var vm = _repo.GetAllPosts(pageNumber, category, search);
            if(vm.PageNumber < pageNumber)
                return RedirectToAction("Index", new { pageNumber = 1, category });

            return View(vm);
        }

        public IActionResult Post(int id) =>
            View(_repo.GetPost(id));

        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Weekly")]
        public IActionResult Image(string image) =>
            new FileStreamResult(
                _fileManager.ImageStream(image), 
                $"image/{image.Substring(image.LastIndexOf('.') + 1)}"
                );

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Post", new { id = vm.PostId });

            var post = _repo.GetPost(vm.PostId);
            if(vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment 
                {
                    Message = vm.Message,
                    Created = DateTime.Now
                });
                _repo.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment 
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now
                };
                _repo.AddSubComment(comment);
            }

            await _repo.SaveChangesAsync();

            return RedirectToAction("Post", new { id = vm.PostId });
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> About(Contact vm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("About");
            }
            else
            { 
                var contact = new Contact
                {
                    FullName = vm.FullName,
                    Email = vm.Email,
                    Subject = vm.Subject,
                    Comment = vm.Comment
                };
                _repo.AddContactRequest(contact);
            }
            await _repo.SaveChangesAsync();

            return RedirectToAction("About");
        }

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult Index(string category)
        //{
        //    var posts =  string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category);
        //    return View(posts);
        //}

        //public IActionResult Post(int id)
        //{
        //    var post = _repo.GetPost(id);

        //    return View(post);
        //}

        //[HttpGet("/Image/{image}")]
        //public IActionResult Image(string image)
        //{
        //    var mime = image.Substring(image.LastIndexOf('.') + 1);
        //    return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        //}
    }
}
