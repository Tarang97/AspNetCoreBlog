using AspNetCoreBlog.Models;
using AspNetCoreBlog.Models.Comments;
using AspNetCoreBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreBlog.Data.Repository
{
    public interface IRepository
    {
        Post GetPost(int id);
        Contact GetContact(int id);
        List<Post> GetAllPosts();
        List<Contact> GetAllContacts();
        IndexViewModel GetAllPosts(int pageNumber, string category, string search);
        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(int id);

        void AddContactRequest(Contact contact);
        void RemoveContactRequest(int id);

        void AddSubComment(SubComment comment);
        Task<bool> SaveChangesAsync();
    }
}
