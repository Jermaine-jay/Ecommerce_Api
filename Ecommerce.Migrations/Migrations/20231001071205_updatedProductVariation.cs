using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class updatedProductVariation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductVariations_ProductVariationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductVariationId",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Carts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "CartItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "CartItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariations_ProductId",
                table: "ProductVariations",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariations_Products_ProductId",
                table: "ProductVariations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariations_Products_ProductId",
                table: "ProductVariations");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariations_ProductId",
                table: "ProductVariations");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "CartItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "CartItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductVariationId",
                table: "Products",
                column: "ProductVariationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductVariations_ProductVariationId",
                table: "Products",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
