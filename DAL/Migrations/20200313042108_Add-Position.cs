using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DAL.Migrations
{
    public partial class AddPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO public.\"Positions\"(\"PositionId\", \"Name\") VALUES('" + Guid.NewGuid() + "', 'Junior')");
            migrationBuilder.Sql("INSERT INTO public.\"Positions\"(\"PositionId\", \"Name\") VALUES('" + Guid.NewGuid() + "', 'Mid-level')");
            migrationBuilder.Sql("INSERT INTO public.\"Positions\"(\"PositionId\", \"Name\") VALUES('" + Guid.NewGuid() + "', 'Senior')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
