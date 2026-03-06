using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    [DbContext(typeof(Airport_Managment_SYS.DataAccess.ApplicationDbcontext))]
    [Migration("20260228150000_SeedPlanesAndTrips")]
    public partial class SeedPlanesAndTrips : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // this migration originally seeded airplanes, seats and trips for testing
            // the application now performs seeding in DbInitializer instead and the
            // model has evolved (seat price moved to SeatClass).  Executing the
            // original InsertData calls during `database update` was causing the
            // runtime model validation exception reported by the user:
            // "There is no entity type mapped to table 'Airplanes' in the current model."
            //
            // Rather than trying to reconcile the old seed code with the current
            // model, we simply make this migration a no-op.  The schema produced by
            // earlier migrations already contains the necessary tables, and the
            // initializer will populate test data at application startup.
            //
            // If you need to re-enable the migration for some reason, move the
            // seeding logic back into DbInitializer and delete this file entirely
            // or recreate a fresh empty migration.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // nothing to un-do; seeding is handled outside of migrations now
        }
    }
}