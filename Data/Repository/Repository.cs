using AspNetCoreBlog.Helpers;
using AspNetCoreBlog.Models;
using AspNetCoreBlog.Models.Comments;
using AspNetCoreBlog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreBlog.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _ctx;
        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public void AddContactRequest(Contact contact)
        {
            _ctx.Contacts.Add(contact);
        }

        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);
        }

        public void AddSubComment(SubComment comment)
        {
            _ctx.SubComments.Add(comment);
        }

        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }

        public List<Contact> GetAllContacts()
        {
            return _ctx.Contacts.ToList();
        }

        public IndexViewModel GetAllPosts(
            int pageNumber, 
            string category,
            string search)
        {
            Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

            int pageSize = 6;
            int skipAmount = pageSize * (pageNumber - 1);
            
            int capacity = skipAmount + pageSize;

            var query = _ctx.Posts.AsNoTracking().AsQueryable();

            if(!String.IsNullOrEmpty(category))
                query = query.Where(x => InCategory(x));

            if (!String.IsNullOrEmpty(search))
                query = query.Where(x => 
                                    EF.Functions.Like(x.Title, $"%{search}%") ||
                                    EF.Functions.Like(x.Body, $"%{search}%") ||
                                    EF.Functions.Like(x.Description, $"%{search}%"));

            int postsCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)postsCount / pageSize);

            if(pageNumber < postsCount)
            {
                return new IndexViewModel
                {
                    PageNumber = pageNumber,
                    PageCount = pageCount,
                    NextPage = postsCount > capacity,
                    Pages = PageHelper.PageNumbers(pageNumber, pageCount).ToList(),
                    Category = category,
                    Search = search,
                    Posts = query
                            .Skip(skipAmount)
                            .Take(pageSize)
                            .ToList()
                };
            }
            else
            {
                return new IndexViewModel
                {
                    PageNumber = 1,
                    PageCount = pageCount,
                    NextPage = postsCount > capacity,
                    Pages = PageHelper.PageNumbers(pageNumber, pageCount).ToList(),
                    Category = category,
                    Search = search,
                    Posts = query
                            .Skip(skipAmount)
                            .Take(pageSize)
                            .ToList()
                };
            }
            
        }

        public Contact GetContact(int id)
        {
            return _ctx.Contacts.FirstOrDefault(p => p.Id == id);
        }

        //public IndexViewModel GetAllPosts(int pageNumber)
        //{
        //    int pageSize = 6;
        //    int skipAmount = pageSize * (pageNumber - 1);
        //    int postsCount = _ctx.Posts.Count();
        //    int capacity = skipAmount + pageSize;

        //    return new IndexViewModel {
        //        PageNumber = pageNumber,
        //        NextPage = postsCount > capacity,
        //        Posts = _ctx.Posts
        //            .Skip(skipAmount)
        //            .Take(pageSize)
        //            .ToList()
        //    };
        //}

        public Post GetPost(int id)
        {
            return _ctx.Posts
                .Include(p => p.MainComments)
                .ThenInclude(mc => mc.SubComments)
                .FirstOrDefault(p => p.Id == id);
        }

        public void RemoveContactRequest(int id)
        {
            _ctx.Contacts.Remove(GetContact(id));
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }

        public async Task<bool> SaveChangesAsync()
        {
            if(await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }
    }
}
