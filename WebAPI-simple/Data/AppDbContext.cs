using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using WebAPI_simple.Models.Domain;

namespace WebAPI_simple.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {
            //contructor
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book_Author>()
                .HasOne(b => b.Book)
                .WithMany(ba => ba.Book_Authors)
                .HasForeignKey(bi => bi.BookId);
            modelBuilder.Entity<Book_Author>()
                .HasOne(b => b.Author)
                .WithMany(ba => ba.Book_Authors)
                .HasForeignKey(bi => bi.AuthorId);

            base.OnModelCreating(modelBuilder);

            // Seed Publishers
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "NXB Trẻ" },
                new Publisher { Id = 2, Name = "NXB Kim Đồng" },
                new Publisher { Id = 3, Name = "NXB Văn Học" },
                new Publisher { Id = 4, Name = "NXB Hội Nhà Văn" },
                new Publisher { Id = 5, Name = "NXB Phụ Nữ Việt Nam" }
            );

            // Seed Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FullName = "Nguyễn Nhật Ánh" },
                new Author { Id = 2, FullName = "Tô Hoài" },
                new Author { Id = 3, FullName = "Nam Cao" },
                new Author { Id = 4, FullName = "Nguyễn Ngọc Tư" },
                new Author { Id = 5, FullName = "Vũ Trọng Phụng" },
                new Author { Id = 6, FullName = "Nguyễn Minh Châu" },
                new Author { Id = 7, FullName = "Hemingway" },
                new Author { Id = 8, FullName = "J.K. Rowling" },
                new Author { Id = 9, FullName = "George Orwell" },
                new Author { Id = 10, FullName = "Haruki Murakami" }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh",
                Description = "Câu chuyện tuổi thơ hồn nhiên, cảm động của hai anh em Thiều và Tường.",
                IsRead = true,
                DateRead = new DateTime(2025, 10, 7),
                Rate = 5,
                Genre = "Tuổi thơ",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 10, 7),
                PublisherID = 1
            },
            new Book
            {
                Id = 2,
                Title = "Dế Mèn Phiêu Lưu Ký",
                Description = "Hành trình trưởng thành và khám phá thế giới của Dế Mèn.",
                IsRead = true,
                DateRead = new DateTime(2025, 9, 20),
                Rate = 4,
                Genre = "Thiếu nhi",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 9, 20),
                PublisherID = 2
            },
            new Book
            {
                Id = 3,
                Title = "Chí Phèo",
                Description = "Bi kịch của người nông dân bị xã hội tha hoá trong thời kỳ phong kiến.",
                IsRead = false,
                DateRead = null,
                Rate = null,
                Genre = "Hiện thực phê phán",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 8, 15),
                PublisherID = 3
            },
            new Book
            {
                Id = 4,
                Title = "Cánh Đồng Bất Tận",
                Description = "Những phận người lầm lũi và khát vọng sống nơi miền sông nước.",
                IsRead = true,
                DateRead = new DateTime(2025, 8, 10),
                Rate = 5,
                Genre = "Tâm lý xã hội",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 8, 10),
                PublisherID = 4
            },
            new Book
            {
                Id = 5,
                Title = "Bến Quê",
                Description = "Câu chuyện về số phận con người và những giá trị bình dị của cuộc sống.",
                IsRead = true,
                DateRead = new DateTime(2025, 7, 15),
                Rate = 4,
                Genre = "Tâm lý xã hội",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 7, 15),
                PublisherID = 3
            },
            new Book
            {
                Id = 6,
                Title = "Ông Già Và Biển Cả",
                Description = "Câu chuyện về nghị lực phi thường của con người trước thiên nhiên khắc nghiệt.",
                IsRead = true,
                DateRead = new DateTime(2025, 6, 10),
                Rate = 5,
                Genre = "Văn học nước ngoài",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 6, 10),
                PublisherID = 4
            },
            new Book
            {
                Id = 7,
                Title = "Harry Potter và Hòn Đá Phù Thủy",
                Description = "Cuộc phiêu lưu đầu tiên của cậu bé phù thủy Harry Potter tại Hogwarts.",
                IsRead = true,
                DateRead = new DateTime(2025, 5, 1),
                Rate = 5,
                Genre = "Giả tưởng",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 5, 1),
                PublisherID = 2
            },
            new Book
            {
                Id = 8,
                Title = "1984",
                Description = "Một xã hội toàn trị nơi mọi hành động đều bị giám sát.",
                IsRead = false,
                DateRead = null,
                Rate = null,
                Genre = "Chính trị - Xã hội",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 4, 20),
                PublisherID = 5
            },
            new Book
            {
                Id = 9,
                Title = "Rừng Na Uy",
                Description = "Hành trình tuổi trẻ đầy mất mát và khát vọng sống.",
                IsRead = true,
                DateRead = new DateTime(2025, 3, 18),
                Rate = 4,
                Genre = "Tâm lý - Lãng mạn",
                CoverUrl = "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg",
                DateAdded = new DateTime(2025, 3, 18),
                PublisherID = 1
            }
        );


            // Seed Book_Author relationships
            modelBuilder.Entity<Book_Author>().HasData(
                new Book_Author { Id = 1, BookId = 1, AuthorId = 1 },
                new Book_Author { Id = 2, BookId = 2, AuthorId = 2 },
                new Book_Author { Id = 3, BookId = 3, AuthorId = 3 },
                new Book_Author { Id = 4, BookId = 4, AuthorId = 4 },
                new Book_Author { Id = 5, BookId = 5, AuthorId = 6 },
                new Book_Author { Id = 6, BookId = 6, AuthorId = 7 },
                new Book_Author { Id = 7, BookId = 7, AuthorId = 8 },
                new Book_Author { Id = 8, BookId = 8, AuthorId = 9 },
                new Book_Author { Id = 9, BookId = 9, AuthorId = 10 }
            );
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book_Author> Book_Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Image> Images { get; set; }

        }
}
