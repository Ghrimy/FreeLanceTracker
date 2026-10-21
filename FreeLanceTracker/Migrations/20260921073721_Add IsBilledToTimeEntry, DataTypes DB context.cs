using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreeLanceTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBilledToTimeEntryDataTypesDBcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_UserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Clients_ClientId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItem_Invoice_InvoiceId",
                table: "InvoiceLineItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItem_Projects_ProjectId",
                table: "InvoiceLineItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItem_TimeEntry_TimeEntryId",
                table: "InvoiceLineItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Clients_ClientId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeEntry_Projects_ProjectId",
                table: "TimeEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeEntry",
                table: "TimeEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceLineItem",
                table: "InvoiceLineItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.RenameTable(
                name: "TimeEntry",
                newName: "TimeEntries");

            migrationBuilder.RenameTable(
                name: "InvoiceLineItem",
                newName: "InvoiceLineItems");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.RenameIndex(
                name: "IX_TimeEntry_ProjectId",
                table: "TimeEntries",
                newName: "IX_TimeEntries_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLineItem_TimeEntryId",
                table: "InvoiceLineItems",
                newName: "IX_InvoiceLineItems_TimeEntryId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLineItem_ProjectId",
                table: "InvoiceLineItems",
                newName: "IX_InvoiceLineItems_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLineItem_InvoiceId",
                table: "InvoiceLineItems",
                newName: "IX_InvoiceLineItems_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_ClientId",
                table: "Invoices",
                newName: "IX_Invoices_ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                table: "TimeEntries",
                type: "decimal(6,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<bool>(
                name: "IsBilled",
                table: "TimeEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "InvoiceLineItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeEntries",
                table: "TimeEntries",
                column: "TimeEntryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceLineItems",
                table: "InvoiceLineItems",
                column: "InvoiceLineItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AspNetUsers_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItems_Invoices_InvoiceId",
                table: "InvoiceLineItems",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItems_Projects_ProjectId",
                table: "InvoiceLineItems",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItems_TimeEntries_TimeEntryId",
                table: "InvoiceLineItems",
                column: "TimeEntryId",
                principalTable: "TimeEntries",
                principalColumn: "TimeEntryId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Clients_ClientId",
                table: "Invoices",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Clients_ClientId",
                table: "Projects",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeEntries_Projects_ProjectId",
                table: "TimeEntries",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_UserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItems_Invoices_InvoiceId",
                table: "InvoiceLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItems_Projects_ProjectId",
                table: "InvoiceLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItems_TimeEntries_TimeEntryId",
                table: "InvoiceLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Clients_ClientId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Clients_ClientId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeEntries_Projects_ProjectId",
                table: "TimeEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeEntries",
                table: "TimeEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceLineItems",
                table: "InvoiceLineItems");

            migrationBuilder.DropColumn(
                name: "IsBilled",
                table: "TimeEntries");

            migrationBuilder.RenameTable(
                name: "TimeEntries",
                newName: "TimeEntry");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.RenameTable(
                name: "InvoiceLineItems",
                newName: "InvoiceLineItem");

            migrationBuilder.RenameIndex(
                name: "IX_TimeEntries_ProjectId",
                table: "TimeEntry",
                newName: "IX_TimeEntry_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_ClientId",
                table: "Invoice",
                newName: "IX_Invoice_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLineItems_TimeEntryId",
                table: "InvoiceLineItem",
                newName: "IX_InvoiceLineItem_TimeEntryId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLineItems_ProjectId",
                table: "InvoiceLineItem",
                newName: "IX_InvoiceLineItem_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLineItems_InvoiceId",
                table: "InvoiceLineItem",
                newName: "IX_InvoiceLineItem_InvoiceId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                table: "TimeEntry",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "InvoiceLineItem",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeEntry",
                table: "TimeEntry",
                column: "TimeEntryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "InvoiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceLineItem",
                table: "InvoiceLineItem",
                column: "InvoiceLineItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AspNetUsers_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Clients_ClientId",
                table: "Invoice",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItem_Invoice_InvoiceId",
                table: "InvoiceLineItem",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItem_Projects_ProjectId",
                table: "InvoiceLineItem",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItem_TimeEntry_TimeEntryId",
                table: "InvoiceLineItem",
                column: "TimeEntryId",
                principalTable: "TimeEntry",
                principalColumn: "TimeEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Clients_ClientId",
                table: "Projects",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeEntry_Projects_ProjectId",
                table: "TimeEntry",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
