using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DAL.Migrations
{
    public partial class Addchallenges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO public.\"Challenges\"(\"ChallengeId\", \"Name\", \"Content\", \"PositionId\") VALUES('" + Guid.NewGuid() + "', 'challenge1', 'something1', '578d5d9d-c856-4c8f-bb5e-864a442e99e0')");
            migrationBuilder.Sql("INSERT INTO public.\"Challenges\"(\"ChallengeId\", \"Name\", \"Content\", \"PositionId\") VALUES('" + Guid.NewGuid() + "', 'challenge2', 'something2', '578d5d9d-c856-4c8f-bb5e-864a442e99e0')");
            migrationBuilder.Sql("INSERT INTO public.\"Challenges\"(\"ChallengeId\", \"Name\", \"Content\", \"PositionId\") VALUES('" + Guid.NewGuid() + "', 'challenge3', 'something3', '578d5d9d-c856-4c8f-bb5e-864a442e99e0')");
            migrationBuilder.Sql("INSERT INTO public.\"Challenges\"(\"ChallengeId\", \"Name\", \"Content\", \"PositionId\") VALUES('" + Guid.NewGuid() + "', 'challenge4', 'something4', '14969a4d-a42e-46c7-808f-51792945df39')");
            migrationBuilder.Sql("INSERT INTO public.\"Challenges\"(\"ChallengeId\", \"Name\", \"Content\", \"PositionId\") VALUES('" + Guid.NewGuid() + "', 'challenge5', 'something5', '14969a4d-a42e-46c7-808f-51792945df39')");
            migrationBuilder.Sql("INSERT INTO public.\"Challenges\"(\"ChallengeId\", \"Name\", \"Content\", \"PositionId\") VALUES('" + Guid.NewGuid() + "', 'challenge6', 'something6', 'f077b7e0-cca5-4e67-9a1a-9e64d0974716')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
