using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DAL.Migrations
{
    public partial class SeedChallenges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO public.\"Challenges\"(\"ChallengeId\", \"Name\", \"Content\", \"PositionId\") VALUES('" + Guid.NewGuid() + "', 'Recursion', 'Do something cool with recursion. Cal f(5)=x + 5', '3c40b469-6b3e-4b68-ac98-be9325fc2932')");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
