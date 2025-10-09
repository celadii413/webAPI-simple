using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebAPI_simple.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    DateRead = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rate = table.Column<int>(type: "int", nullable: true),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoverUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublisherID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Publishers_PublisherID",
                        column: x => x.PublisherID,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Book_Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Book_Authors_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Book_Authors_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "FullName" },
                values: new object[,]
                {
                    { 1, "Nguyễn Nhật Ánh" },
                    { 2, "Tô Hoài" },
                    { 3, "Nam Cao" },
                    { 4, "Nguyễn Ngọc Tư" },
                    { 5, "Vũ Trọng Phụng" },
                    { 6, "Nguyễn Minh Châu" },
                    { 7, "Hemingway" },
                    { 8, "J.K. Rowling" },
                    { 9, "George Orwell" },
                    { 10, "Haruki Murakami" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "NXB Trẻ" },
                    { 2, "NXB Kim Đồng" },
                    { 3, "NXB Văn Học" },
                    { 4, "NXB Hội Nhà Văn" },
                    { 5, "NXB Phụ Nữ Việt Nam" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "CoverUrl", "DateAdded", "DateRead", "Description", "Genre", "IsRead", "PublisherID", "Rate", "Title" },
                values: new object[,]
                {
                    { 1, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Câu chuyện tuổi thơ hồn nhiên, cảm động của hai anh em Thiều và Tường.", "Tuổi thơ", true, 1, 5, "Tôi Thấy Hoa Vàng Trên Cỏ Xanh" },
                    { 2, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hành trình trưởng thành và khám phá thế giới của Dế Mèn.", "Thiếu nhi", true, 2, 4, "Dế Mèn Phiêu Lưu Ký" },
                    { 3, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bi kịch của người nông dân bị xã hội tha hoá trong thời kỳ phong kiến.", "Hiện thực phê phán", false, 3, null, "Chí Phèo" },
                    { 4, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Những phận người lầm lũi và khát vọng sống nơi miền sông nước.", "Tâm lý xã hội", true, 4, 5, "Cánh Đồng Bất Tận" },
                    { 5, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Câu chuyện về số phận con người và những giá trị bình dị của cuộc sống.", "Tâm lý xã hội", true, 3, 4, "Bến Quê" },
                    { 6, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Câu chuyện về nghị lực phi thường của con người trước thiên nhiên khắc nghiệt.", "Văn học nước ngoài", true, 4, 5, "Ông Già Và Biển Cả" },
                    { 7, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cuộc phiêu lưu đầu tiên của cậu bé phù thủy Harry Potter tại Hogwarts.", "Giả tưởng", true, 2, 5, "Harry Potter và Hòn Đá Phù Thủy" },
                    { 8, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Một xã hội toàn trị nơi mọi hành động đều bị giám sát.", "Chính trị - Xã hội", false, 5, null, "1984" },
                    { 9, "https://i.pinimg.com/474x/1c/71/55/1c7155f928b1b9dcf71f054be08c7f3c.jpg", new DateTime(2025, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hành trình tuổi trẻ đầy mất mát và khát vọng sống.", "Tâm lý - Lãng mạn", true, 1, 4, "Rừng Na Uy" }
                });

            migrationBuilder.InsertData(
                table: "Book_Authors",
                columns: new[] { "Id", "AuthorId", "BookId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 2 },
                    { 3, 3, 3 },
                    { 4, 4, 4 },
                    { 5, 6, 5 },
                    { 6, 7, 6 },
                    { 7, 8, 7 },
                    { 8, 9, 8 },
                    { 9, 10, 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Book_Authors_AuthorId",
                table: "Book_Authors",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Book_Authors_BookId",
                table: "Book_Authors",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherID",
                table: "Books",
                column: "PublisherID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Book_Authors");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Publishers");
        }
    }
}
