﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceSite.Server.Migrations
{
    public partial class ProductSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { 1, "Dune is a 1965 epic science fiction novel by American author Frank Herbert, originally published as two separate serials in Analog magazine. It tied with Roger Zelazny's This Immortal for the Hugo Award in 1966 and it won the inaugural Nebula Award for Best Novel. It is the first installment of the Dune saga. In 2003, it was described as the world's best-selling science fiction novel.", "https://upload.wikimedia.org/wikipedia/en/d/de/Dune-Frank_Herbert_%281965%29_First_edition.jpg", 14.99m, "Dune" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { 2, "Evil Dead II (also known in publicity materials as Evil Dead 2: Dead by Dawn) is a 1987 American horror comedy film directed by Sam Raimi, who co-wrote it with Scott Spiegel, and produced by Robert Tapert. The film is the second installment in the Evil Dead film series and is considered both a remake and sequel to The Evil Dead (1981).The film stars Bruce Campbell as Ash Williams, who vacations with his girlfriend to a remote cabin in the woods. He discovers an audio tape of recitations from a book of ancient texts, and when the recording is played, it unleashes a number of demons which possess and torment him.", "https://upload.wikimedia.org/wikipedia/en/6/6d/Evil_Dead_II_poster.jpg", 9.99m, "Evil Dead II" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { 3, "The themes include alienation and criticism of the music business. The bulk of the album is taken up by Shine On You Crazy Diamond, a nine-part tribute to founding member Syd Barrett, who left the band seven years earlier due to his deteriorating mental health. Barrett coincidentally visited during the album's production in 1975. Like their previous record, The Dark Side of the Moon (1973), Pink Floyd used studio effects and synthesisers. Guest singers included Roy Harper, who provided the lead vocals on Have a Cigar, and Venetta Fields, who added backing vocals to Shine On You Crazy Diamond. To promote the album, the band released the double A-side single Have a Cigar / Welcome to the Machine.", "https://upload.wikimedia.org/wikipedia/en/a/a4/Pink_Floyd%2C_Wish_You_Were_Here_%281975%29.png", 7.99m, "Wish You Were Here" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
