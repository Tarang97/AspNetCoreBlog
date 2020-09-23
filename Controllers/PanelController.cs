using AspNetCoreBlog.Data.FileManager;
using AspNetCoreBlog.Data.Repository;
using AspNetCoreBlog.Models;
using AspNetCoreBlog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private IRepository _repo;
        private IFileManager _file;

        public PanelController(IRepository repo, IFileManager file)
        {
            _repo = repo;
            _file = file;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AllPosts()
        {
            var posts = _repo.GetAllPosts();
            return View(posts);
        }

        public IActionResult AllContactRequest()
        {
            var contacts = _repo.GetAllContacts();
            return View(contacts);
        }

        [HttpGet]
        public async Task<IActionResult> ContactDelete(int id)
        {
            _repo.RemoveContactRequest(id);
            await _repo.SaveChangesAsync();
            return RedirectToAction("AllContactRequest");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new PostViewModel());
            }
            else
            {
                var post = _repo.GetPost((int)id);
                return View(new PostViewModel 
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    CurrentImage = post.Image,
                    Description = post.Description,
                    Category = post.Category,
                    Tags = post.Tags
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            var post = new Post
            {
                Id = vm.Id,
                Title = vm.Title,
                Body = vm.Body,
                Description = vm.Description,
                Category = vm.Category,
                Tags = vm.Tags

            };

            if(vm.Image == null)
            {
                post.Image = vm.CurrentImage;
            }
            else
            {
                if (!string.IsNullOrEmpty(vm.CurrentImage))
                    _file.RemoveImage(vm.CurrentImage);
                post.Image = await _file.SaveImage(vm.Image);
            }

            if (post.Id > 0)
                _repo.UpdatePost(post);
            else
                _repo.AddPost(post);

            if (await _repo.SaveChangesAsync())
                return RedirectToAction("AllPosts");
            else
                return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {
            _repo.RemovePost(id);
            await _repo.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
