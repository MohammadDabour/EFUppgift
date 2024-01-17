using Microsoft.EntityFrameworkCore;
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello,World!");
        using (var context = new EntityContext())
        {
            var users = ReadFiles("user.csv", parts => new User
            {
                Id = int.Parse(parts[0]),
                Username = parts[1],
                Password = parts[2],
                PostId = int.Parse(parts[3])
            });

            var posts = ReadFiles("post.csv", parts => new Post
            {
                Id = int.Parse(parts[0]),
                Title = parts[1],
                Content = parts[2],
                BlogId = int.Parse(parts[3]),
                UserId = int.Parse(parts[4])
            });

            var blogs = ReadFiles("blog.csv", parts => new Blog
            {
                Id = int.Parse(parts[0]),
                Url = parts[1],
                Name = parts[2],
                PostId = int.Parse(parts[3])
            });

            context.Users.AddRange(users);
            context.Posts.AddRange(posts);
            context.Blogs.AddRange(blogs);

            context.SaveChanges();
            DatabaseTree(context);

        }

    }

    static List<T> ReadFiles<T>(string filePath, Func<string[], T> convertFunc)
    {
        var result = new List<T>();

        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var values = line.Split(',');

            if (values.Length == typeof(T).GetProperties().Length)
            {
                result.Add(convertFunc(values));
            }
            else
            {
                Console.WriteLine($"{line}");
            }
        }

        return result;
    }

    static void DatabaseTree(EntityContext context)
    {
        var users = context.Users.ToList();

        foreach (var user in users)
        {
            Console.WriteLine($"User: {user.Username}");

            var posts = context.Posts
                .Where(p => p.UserId == user.Id)
                .ToList();

            foreach (var post in posts)
            {
                Console.WriteLine($"Post: {post.Title}");

                var blog = context.Blogs
                    .SingleOrDefault(b => b.Id == post.BlogId);

                if (blog != null)
                {
                    Console.WriteLine($"Blog: {blog.Name}");
                }
            }
        }
    }
}
